using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryDataProducer
{
    public class Generator
    {
        private RNGCryptoServiceProvider _rnd = new RNGCryptoServiceProvider();
        private Random _random = new Random(Environment.TickCount);
        private List<Trackable> _trackeables = new List<Trackable>();
        private TimeSpan _interval;
        private Action<string> _report;
        private Timer _timer;
        private IProducer<Null, string> _kafkaProducer;

        public Generator(int numberOfDeliveryTrucks, int numberOfPackagesPerTruck, string interval, Action<string> report)
        {
            _report = report;
            _interval = TimeSpan.Parse(interval);
            for (int i = 0; i < numberOfDeliveryTrucks; i++)
            {
                var deliveryTruck = new DeliveryTruck($"TRUCK{i:0000}");
                _trackeables.Add(deliveryTruck);
                for (int j = 0; j < numberOfPackagesPerTruck; j++)
                {
                    var package = new Package($"{deliveryTruck.Id}PACKAGE{j:0000}", deliveryTruck);
                    _trackeables.Add(package);
                }
            }
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            //_kafkaProducer = new ProducerBuilder<Null, string>(config).Build();
        }

        public void StartNotifyLocationOfAllDevices()
        {
            var trackeables = GetRandomTrackeables();
            var wait = _interval / trackeables.Count();
            int i = 0;
            _timer = new Timer(_ => 
            {
                if (i >= trackeables.Count())
                {
                    i = 0;
                    trackeables = GetRandomTrackeables();
                }
                var trackeable = trackeables[i];
                lock (trackeable)
                {
                    trackeable.Move();
                    string json = JsonSerializer.Serialize(trackeable);
                    _report.Invoke(json);
                    //_kafkaProducer.Produce(trackeable.GetTrackableType(), new Message<Null, string> { Value = json });
                }
                i++;
            }, null, TimeSpan.FromSeconds(1), wait);
        }

        public void StopNotifyLocationOfAllDevices()
        {
            if(_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private Trackable[] GetRandomTrackeables()
        {
            return _trackeables.OrderBy(x => GetNextInt32()).ToArray();
        }

        private int GetNextInt32()
        {
            return _random.Next();
        }

        private double GetNextDouble(double min, double max)
        {
            return min + (_random.NextDouble() * (max - min));
        }

        public string[] SearchPackagesToBeLost(string pattern)
        {
            var trackeablesToBeLost = _trackeables.Where(t => t.Id.Contains(pattern));
            return trackeablesToBeLost.Take(11).Select(t => t.Id).ToArray();
        }

        public int LostPackages(string pattern)
        {
            int count = 0;
            var packages = _trackeables.Where(t => t.GetType() == typeof(Package));
            foreach(var package in packages.Cast<Package>())
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    package.Lost = false;
                }
                else
                {
                    package.Lost = package.Id.Contains(pattern);
                    if (package.Lost)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}

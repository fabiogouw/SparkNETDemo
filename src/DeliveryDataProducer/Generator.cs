using Confluent.Kafka;
using DeliveryDataProducer.Entities;
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
        private Random _random = new Random(Environment.TickCount);
        private List<CreditCardSimulator> _simulators = new List<CreditCardSimulator>();
        private TimeSpan _interval;
        private Action<string> _report;
        private Timer _timer;
        private IProducer<Null, string> _kafkaProducer;

        public Generator(int numberOfCreditCards, string interval, Action<string> report)
        {
            _report = report;
            _interval = TimeSpan.Parse(interval);
            for (int i = 0; i < Math.Min(numberOfCreditCards, 9999); i++)
            {
                var simulator = new CreditCardSimulator()
                {
                    Number = $"{i:0000}-0000-0000-0000",
                    Lat = _random.NextDouble(-45, +45),
                    Lng = _random.NextDouble(-90, +90)
            };
                _simulators.Add(simulator);
            }
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            _kafkaProducer = new ProducerBuilder<Null, string>(config).Build();
        }

        public void SendCommands(string[] commands)
        {
            foreach(var command in commands)
            {
                _kafkaProducer.Produce("transactions", new Message<Null, string> { Value = command });
                _report(command);
            }
        }

        public void StartNotifyLocationOfAllDevices()
        {
            var simulators = GetRandomTrackeables();
            var wait = _interval / simulators.Count();
            int i = 0;
            _timer = new Timer(_ => 
            {
                if (i >= simulators.Count())
                {
                    i = 0;
                    simulators = GetRandomTrackeables();
                }
                var simulator = simulators[i];
                lock (simulator)
                {
                    simulator.GenerateNormalTransaction(wait);
                    string json = JsonSerializer.Serialize((object)simulator);
                    _report.Invoke(json);
                    _kafkaProducer.Produce("transactions", new Message<Null, string> { Value = json });
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

        private CreditCardSimulator[] GetRandomTrackeables()
        {
            return _simulators.OrderBy(x => _random.NextDouble()).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliveryDataProducer
{
    public class DeliveryTruck : Trackable
    {
        private double _directionLat = 0;
        private double _directionLng = 0;
        private static Random _random = new Random(Environment.TickCount);
        public DeliveryTruck(string id)
            : base(id)
        {
            Lat = _random.NextDouble(-90, +90);
            Lng = _random.NextDouble(-180, +180);
            _directionLat = _random.NextDouble(-1, +1) / 10000;
            _directionLng = _random.NextDouble(-1, +1) / 10000;
        }
        [JsonIgnore]
        public List<Package> Packages { get; private set; }

        public override void Move()
        {
            Lat += _directionLat;
            if(Lat >= 90 || Lat <= -90)
            {
                _directionLat *= -1;
            }
            Lng += _directionLng;
            if (Lng >= 180 || Lng <= -180)
            {
                _directionLng *= -1;
            }
            EventTime = DateTime.Now;
        }

        public override string GetTrackableType()
        {
            return "trucks";
        }
    }
}
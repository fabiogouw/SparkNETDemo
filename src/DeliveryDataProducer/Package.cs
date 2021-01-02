using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliveryDataProducer
{
    public class Package : Trackable
    {
        private DeliveryTruck _deliveryTruck;
        private static Random _random = new Random(Environment.TickCount);
        public Package(string id, DeliveryTruck deliveryTruck)
            : base(id)
        {
            _deliveryTruck = deliveryTruck;
        }
        [JsonIgnore]
        public bool Lost { get; set; }
        [JsonPropertyName("idTruck")]
        public string IdTruck
        {
            get { return _deliveryTruck.Id; }
            set { }
        }

        public override void Move()
        {
            if (!Lost)
            {
                Lat = _deliveryTruck.Lat; // + _random.NextDouble(-1, +1) / 100000;
                Lng = _deliveryTruck.Lng; // + _random.NextDouble(-1, +1) / 100000;
            }
            EventTime = DateTime.Now;
        }

        public override string GetTrackableType()
        {
            return "packages";
        }
    }
}

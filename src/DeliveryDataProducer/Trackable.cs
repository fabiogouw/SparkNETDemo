using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliveryDataProducer
{
    public abstract class Trackable
    {
        public Trackable(string id)
        {
            Id = id;
        }
        [JsonPropertyName("id")]
        public string Id { get; private set; }
        [JsonPropertyName("lat")]
        [JsonConverter(typeof(DoubleJsonConverter))]
        public double Lat { get; set; }
        [JsonPropertyName("lng")]
        [JsonConverter(typeof(DoubleJsonConverter))]
        public double Lng { get; set; }
        [JsonPropertyName("lastInfo")]
        public DateTime LastInfo { get; set; }
        public abstract void Move();
        public abstract string GetTrackableType();
    }
}

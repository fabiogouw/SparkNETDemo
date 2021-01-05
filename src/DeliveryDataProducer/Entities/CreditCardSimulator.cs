using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeliveryDataProducer.Entities
{
    public class CreditCardSimulator
    {
        private static Random _random = new Random(Environment.TickCount);
        public static readonly string[] CATEGORIES = new[] { "alimentação", "vestuário", "lazer", "educação", "saúde" };
        private static int _count = 1;
        
        [JsonPropertyName("transaction")]
        public string Transaction { get; set; }
        [JsonPropertyName("number")]
        public string Number { get; set; }
        [JsonPropertyName("lat")]
        [JsonConverter(typeof(DoubleJsonConverter))]
        public double Lat { get; set; }
        [JsonPropertyName("lng")]
        [JsonConverter(typeof(DoubleJsonConverter))]
        public double Lng { get; set; }
        [JsonPropertyName("amount")]
        [JsonConverter(typeof(DoubleJsonConverter))]
        public double Amount { get; set; }
        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("eventTime")]
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime EventTime { get; set; }
        public void GenerateNormalTransaction(TimeSpan interval)
        {
            Transaction = _count++.ToString();
            double totalKm = interval.TotalSeconds / 45;
            Lat += totalKm * 0.0064 + _random.NextDouble(-0.0001, 0.0001);
            Lng += totalKm * 0.0063 + _random.NextDouble(-0.0001, 0.0001);
            Amount = _random.NextDouble(0.1, 100);
            Category = CATEGORIES[_random.Next(0, CATEGORIES.Length - 1)];
            EventTime = DateTime.Now;
        }

        public void FraudData()
        {
            Lat += _random.NextDouble(-0.1, 0.1);
            Lng += _random.NextDouble(-0.1, 0.1);
        }
    }
}

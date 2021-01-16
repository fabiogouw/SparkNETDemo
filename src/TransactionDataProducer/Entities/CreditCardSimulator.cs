using DeliveryDataProducer.Utils;
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
        public static readonly double LAT_LEFT_LIMIT = -23.53000;
        public static readonly double LAT_RIGTH_LIMIT = -23.51000;
        public static readonly double LNG_UPPER_LIMIT = -46.64000;
        public static readonly double LNG_LOWER_LIMIT = -46.68000;
        public static readonly string[] CATEGORIES = new[] { "health", "clothing", "sport", "garden", "pets", "office", "books", "auto", "furniture" };
        private static int _count = 1;

        private int _latDirection = 1;
        private int _lngDirection = 1;

        public CreditCardSimulator()
        {
            _latDirection = _random.Next(-1, 1) >= 0 ? 1 : -1;
            _lngDirection = _random.Next(-1, 1) >= 0 ? 1 : -1;
            EventTime = DateTime.Now;
        }

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
        public void GenerateNormalTransaction()
        {
            /* Mover 0.001 em latitude e longitude dá mais ou menos 150 metros (valor preciso é 150.85087509685803)
             * Considerando uma velocidade de 100 km/h e convertendo para 28 m/s, 
             * significa que para andar os 150 metrous, vou precisar de 5,36 segundos
             * Como eu gero a transação de tempos em tempos, eu preciso saber quanto eu andaria em metros nesse intervalo
             */
            var now = DateTime.Now;
            Transaction = _count++.ToString();
            double razao = now.Subtract(EventTime).TotalMilliseconds / 5360;
            // a cada 
            Lat += razao * 0.001 * _latDirection;
            Lng += razao * 0.001 * _lngDirection;
            Amount = _random.NextDouble(0.1, 100);
            Category = CATEGORIES[_random.Next(0, CATEGORIES.Length - 1)];
            EventTime = now;
            if(Lat < LAT_LEFT_LIMIT || Lat > LAT_RIGTH_LIMIT)
            {
                _latDirection *= -1;
            }
            if (Lng < LNG_LOWER_LIMIT || Lng > LNG_UPPER_LIMIT)
            {
                _lngDirection *= -1;
            }
        }

        public void FraudData()
        {
            Lat += _random.NextDouble(-0.1, 0.1);
            Lng += _random.NextDouble(-0.1, 0.1);
        }
    }
}

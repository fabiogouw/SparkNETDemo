using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaProducer
{
    class Program
    {
        private static Random _rnd = new Random(Environment.TickCount);
        public static async Task Main(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = args.Length > 0 ? args[0] : "localhost:9092" };
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        try
                        {
                            string msg = $"{{\"cliente\": \"{NAMES.GetRandom()}\",\"produto\": \"{PRODUCTS.GetRandom()}\",\"opiniao\": \"{OPINIONS.GetRandom()}\"}}";
                            var dr = await producer.ProduceAsync("test", new Message<Null, string> { Value = msg });
                            Console.WriteLine($"=> '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                        }
                        catch (ProduceException<Null, string> e)
                        {
                            Console.WriteLine($"ERRO: {e.Error.Reason}");
                        }
                        Random _rnd = new Random(Environment.TickCount);
                        Thread.Sleep(_rnd.Next(250, 750));
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
        }

        private static string[] NAMES = new[]
        {
            "Maria", 
            "Ana",
            "Francisca",
            "Fernanda",
            "Adriana",
            "Jose",
            "Joao",
            "Antonio",
            "Carlos",
            "Paulo"
        };

        private static string[] PRODUCTS = new[]
{
            "Guarda-chuva",
            "Tenis",
            "Calça jeans",
            "Mouse",
            "Violao",
            "Copo",
            "Banco",
            "Lousa",
            "Mala",
            "Almofada"
        };

        private static string[] OPINIONS = new[]
{
            "Não gostei deste produto, é horrível",
            "Muito excelente a qualidade, perfeito!",
            "Não quero nem ver isso perto de mim",
            "Gastei meu dinheiro de uma forma ótima",
            "Comprarei outros da próxima vez com total certeza",
            "A loja estava muito suja",
            "Palmeiras nao tem mundial",
            "Deve vender demais isso, a qualidade é ótima",
            "Bom pra caramba"
        };
    }

    public static class IntExtensions
    {
        public static string GetRandom(this string[] items)
        {
            Random _rnd = new Random(Environment.TickCount);
            int index = _rnd.Next(0, items.Length);
            return items[index];
        }
    }
}

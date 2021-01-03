using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;
using Microsoft.Spark.Sql.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Spark.Sql.Expressions;

namespace StreamingDemo
{
    public class StatefulStreamingDemo : IDemo
    {
        public void Run(string[] args)
        {
            string servidoresKafka = args[0];

            // Obtém a referência ao contexto de execução do Spark
            SparkSession spark = SparkSession
                .Builder()
                .AppName("Exemplo Streaming com Kafka")
                .GetOrCreate();

            spark.Conf().Set("spark.sql.shuffle.partitions", "1");  // sem essa configuração, cada stage ficou com 200 tasks, o que levou uns 4 minutos pra cada batch executar

            // Criando um dataframe pra receber dados do Kafka
            DataFrame dfPackages = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", servidoresKafka)
                .Option("subscribe", "packages")
                .Load()
                .SelectExpr("CAST(value AS STRING)");

            /* Criando schema pra validar o JSON que virá nas mensagens do Kafka
             * Exemplo do JSON: 
             * {
             *      "cliente": "Fulano", 
             *      "produto": "Mochila", 
             *      "opiniao": "Muito boa!"
             * }
             */
            var schemaPackages = new StructType(new[]
{
                            new StructField("id", new StringType()),
                            new StructField("idTruck", new StringType()),
                            new StructField("lat", new DoubleType()),
                            new StructField("lng", new DoubleType()),
                            new StructField("eventTime", new TimestampType())
                        });

            // Fazendo o parse do JSON pra um array ...

            dfPackages = dfPackages.WithColumn("json", Functions.FromJson(
                                            dfPackages.Col("value"),
                                            schemaPackages.SimpleString)
                                        )
                .Select("json.*");  // ... e retornando todas as colunas do array como um novo dataframe

            DataFrame dfTrucks = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", servidoresKafka)
                .Option("subscribe", "trucks")
                .Load()
                .SelectExpr("CAST(value AS STRING)");

            var schemaTrucks = new StructType(new[]
{
                            new StructField("id", new StringType()),
                            new StructField("lat", new DoubleType()),
                            new StructField("lng", new DoubleType()),
                            new StructField("eventTime", new TimestampType())
                        });

            dfTrucks = dfTrucks.WithColumn("json", Functions.FromJson(
                                            dfTrucks.Col("value"),
                                            schemaTrucks.SimpleString)
                                        )
                .Select("json.*");  // ... e retornando todas as colunas do array como um novo dataframe
            dfTrucks = dfTrucks.WithColumnRenamed("id", "truck_id")
                .WithColumnRenamed("lat", "truck_lat")
                .WithColumnRenamed("lng", "truck_lng")
                .WithColumnRenamed("eventTime", "truck_eventTime");

            dfPackages = dfPackages
                .WithColumn("package_window", Functions.Window(Functions.Col("eventTime"), "10 seconds").As("package_window"));

            DataFrame df = dfPackages
                .WithWatermark("eventTime", "10 seconds")
                .GroupBy(Functions.Window(Functions.Col("eventTime"), "10 seconds"), Functions.Col("id")).Count();

            dfTrucks = dfTrucks.WithWatermark("truck_eventTime", "10 seconds")
                .WithColumn("", Functions.Window(Functions.Col("truck_eventTime"), "10 seconds").As("truck_window"));

            //DataFrame df = dfPackages.Join(dfTrucks, Functions.Col("truck_window").Gt(Functions.Col("package_window")));
            //DataFrame df = dfPackages.Join(dfTrucks, Functions.Col("idTruck").EqualTo(Functions.Col("truck_id"))
                //.And(Functions.Col("truck_window").EqualTo(Functions.Col("package_window")))
            //    );
            //df.PrintSchema();

            //Registrando uma função personalizada pra ser usada no dataframe
            //spark.Udf().Register<double, double, double, double, double>("CalcularDistancia", (lat1, lng1, lat2, lng2) => CalcularDistancia(lat1, lng1, lat2, lng2));
            // Criando nova coluna nota com o resultado da análise de sentimento
            //df = df.WithColumn("dist", Functions.CallUDF("CalcularDistancia", df.Col("lat"), df.Col("lng"), df.Col("truck_lat"), df.Col("truck_lng")));

            // Colocando o streaming pra funcionar

            //df = df.Where(df.Col("dist").Gt(20));
            StreamingQuery query = df
                .WriteStream()
                .Format("console")
                .Option("truncate", "false")
                .OutputMode(OutputMode.Update)
                .Start();
            Console.ReadLine();
            Console.WriteLine("Parando query...");
            query.Stop();
            //query.AwaitTermination();   // Necessário pra deixar a aplcação no ar para processar os dados

        }

        public static double CalcularDistancia(double lat1, double lng1, double lat2, double lng2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lng1 - lng2;
            double rtheta = Math.PI * theta / 180;
            double dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist * 1609.344; // em metros
        }
    }
}

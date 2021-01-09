using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;
using Microsoft.Spark.Sql.Types;
using System;
using static Microsoft.Spark.Sql.Functions;

namespace StreamingDemo
{
    public class JoinStreamingDemo : IDemo
    {
        public void Run(string[] args)
        {
            string kafkaBrokers = args[0];
            double maxSpeed = double.Parse(args[1]);

            // Obtém a referência ao contexto de execução do Spark
            SparkSession spark = SparkSession
                .Builder()
                .AppName("Credit Card Fraud")
                .GetOrCreate();

            spark.Conf().Set("spark.sql.shuffle.partitions", "1");  // sem essa configuração, cada stage ficou com 200 tasks, o que levou uns 4 minutos pra cada batch executar

            // Criando um dataframe pra receber dados do Kafka
            DataFrame df = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", kafkaBrokers)
                .Option("subscribe", "transactions")
                .Load()
                .SelectExpr("CAST(value AS STRING)");

            /* Criando schema pra validar o JSON que virá nas mensagens do Kafka
             * Exemplo do JSON: 
             * {
             *      "transaction":"431",
             *      "number":"0015-0000-0000-0000",
             *      "lat":-23.1618,
             *      "lng":-46.47201,
             *      "amount":91.01487,
             *      "category":"pets",
             *      "eventTime":"2021-01-05T19:07:19.3888"
             * }
             */
            var schema = new StructType(new[]
{
                            new StructField("transaction", new StringType()),
                            new StructField("number", new StringType()),
                            new StructField("lat", new DoubleType()),
                            new StructField("lng", new DoubleType()),
                            new StructField("amount", new DoubleType()),
                            new StructField("category", new StringType()),
                            new StructField("eventTime", new TimestampType())
                        });

            // Fazendo o parse do JSON pra um array ...
            df = df.WithColumn("json", FromJson(
                                        df.Col("value"),
                                        schema.SimpleString)
                                    )
                .Select("json.*");  // ... e retornando todas as colunas do array como um novo dataframe

            // Gerando dois dataframes distintos para poder fazer o join e analisar a correção entre as transações
            DataFrame df1 = df
                .WithWatermark("eventTime", "7 minutes");
            DataFrame df2 = df
                .WithColumnRenamed("transaction", "transaction2")
                .WithColumnRenamed("lat", "lat2")
                .WithColumnRenamed("lng", "lng2")
                .WithColumnRenamed("eventTime", "eventTime2")
                .WithWatermark("eventTime2", "7 minutes");

            // Efetuando o join para verificar a correlação de transações dos cartões de crédito
            DataFrame dfJoin = df1.Join(df2, 
                df1.Col("number").EqualTo(df2.Col("number"))
                .And(Col("transaction").NotEqual(Col("transaction2")))
                .And(Col("eventTime2").Between(Col("eventTime"), Col("eventTime") + Expr("interval 5 minutes")))
                );

            //Registrando uma função personalizada pra ser usada no dataframe
            spark.Udf().Register<double, double, double, double, double>("CalculateDistance", (lat1, lng1, lat2, lng2) => CalculateDistance(lat1, lng1, lat2, lng2));
            spark.Udf().Register<double, Timestamp, Timestamp, double>("CalculateSpeed", (dist, eventTime, eventTime2) => CalculateSpeed(dist, eventTime, eventTime2));

            // Criando novas colunas para armazenar a execução do código da UDF
            dfJoin = dfJoin.WithColumn("dist", CallUDF("CalculateDistance", Col("lat"), Col("lng"), Col("lat2"), Col("lng2")));
            dfJoin = dfJoin.WithColumn("speed", CallUDF("CalculateSpeed", Col("dist"), Col("eventTime"), Col("eventTime2")));

            // Filtrando as transações que tiverem a velocidade acima do esperado (parâmetro "maxSpeed")
            dfJoin = dfJoin.Where(Col("speed").Gt(maxSpeed));

            // Colocando o streaming pra funcionar

            StreamingQuery query = dfJoin
                .WriteStream()
                .Format("console")
                .Option("truncate", "false")
                .OutputMode(OutputMode.Append)
                .Start();

            query.AwaitTermination();
        }

        public static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
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

        public static double CalculateSpeed(double dist, Timestamp eventTime1, Timestamp eventTime2)
        {
            TimeSpan time = eventTime2.ToDateTime().Subtract(eventTime1.ToDateTime());
            double distKm = dist / 1000;
            return distKm / time.TotalHours;
        }
    }
}

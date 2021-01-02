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

            //dfPackages = dfPackages.WithWatermark("eventTime", "2 seconds");
            //dfTrucks = dfTrucks.WithWatermark("eventTime", "2 seconds");

            DataFrame df = dfPackages.Join(dfTrucks, dfPackages.Col("idTruck").EqualTo(dfTrucks.Col("id")));

            //Registrando uma função personalizada pra ser usada no dataframe
            //spark.Udf().Register<string, float>("AnaliseDeSentimento", (texto) => AnalisarSentimento(texto, modelo));
            // Criando nova coluna nota com o resultado da análise de sentimento
            //df = df.WithColumn("nota", Functions.CallUDF("AnaliseDeSentimento", df.Col("opiniao")));

            //DataFrame impressions = spark
            //    .ReadStream().Format("rate").Option("rowsPerSecond", "5").Option("numPartitions", "1").Load()
            //    .Select(Functions.Col("value").As("adId"), Functions.Col("timestamp").As("impressionTime"));
            //DataFrame clicks = spark
            //      .ReadStream().Format("rate").Option("rowsPerSecond", "5").Option("numPartitions", "1").Load()
            //      .Where((Functions.Rand() * 100).Cast("integer") < 10)       // 10 out of every 100 impressions result in a click
            //      .Select(Functions.Col("value").Minus(50).As("adId"), Functions.Col("timestamp").As("clickTime"))   // -100 so that a click with same id as impression is generated much later.
            //      .Where("adId > 0");
            //DataFrame df = impressions.Join(clicks, "adId");
            //StreamingQuery query = df
            //    .WriteStream()
            //    .Format("console")
            //    .Start();

            // Colocando o streaming pra funcionar
            StreamingQuery query = df
                .WriteStream()
                //.OutputMode(OutputMode.Append)
                .Format("console")
                //.Trigger(Trigger.Continuous(2000))
                //.Foreach(new RedisForeachWriter())
                .Start();
            /*StreamingQuery query2 = dfTrucks
                .WriteStream()
                .Format("console")
                .Start();
            query2.AwaitTermination();*/
            query.AwaitTermination();   // Necessário pra deixar a aplcação no ar para processar os dados

        }

        /*
        public static float AnalisarSentimento(string texto, string caminhoDoModelo)
        {
            var contexto = new MLContext();
            ITransformer modelo = contexto.Model.Load(caminhoDoModelo, out var modelInputSchema);
            PredictionEngine<Avaliacao, ResultadoPredicao> predEngine = contexto.Model.CreatePredictionEngine<Avaliacao, ResultadoPredicao>(modelo);
            ResultadoPredicao resultado = predEngine.Predict(new Avaliacao { TextoAvaliacao = texto });
            return resultado.Nota;
        }
        */
    }
}

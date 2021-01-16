using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;
using Microsoft.Spark.Sql.Types;
using static Microsoft.Spark.Sql.Functions;

namespace StreamingDemo
{
    public class WindowStreamingDemo : IDemo
    {
        public void Run(string[] args)
        {
            string servidoresKafka = args[0];
            string connectionString = args.Length > 1 ? args[1] : string.Empty;

            // Obtém a referência ao contexto de execução do Spark
            SparkSession spark = SparkSession
                .Builder()
                .AppName("Credit Card Category")
                .GetOrCreate();

            spark.Conf().Set("spark.sql.shuffle.partitions", "1");  // sem essa configuração, cada stage ficou com 200 tasks, o que levou uns 4 minutos pra cada batch executar

            // Criando um dataframe pra receber dados do Kafka
            DataFrame df = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", servidoresKafka)
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

            // Colocando um limite de 7 minutos para receber os eventos atrasados
            df = df.WithWatermark("eventTime", "7 minutes");

            // Somando os valores gastos, agrupando por categoria e por janelas de 4 minutos que se iniciam a cada 2 minutos
            df = df.GroupBy(Window(Col("eventTime"), "2 minutes", "1 minutes"), Col("category"))
                .Sum("amount").WithColumnRenamed("sum(amount)", "total")
                .Select(Col("window.start"), Col("window.end"), Col("category"), Col("total"));

            // Colocando o streaming pra funcionar e gravando os dados retornados
            StreamingQuery query = df
                .WriteStream()
                .Format("console")
                .OutputMode(OutputMode.Update)
                //.Foreach(new MySQLForeachWriter(connectionString))    // Descomentar pra gravar em banco de dados
                .Start();

            query.AwaitTermination();
        }
    }
}

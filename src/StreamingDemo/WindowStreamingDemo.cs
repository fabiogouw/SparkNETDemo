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
                .Load();

            /*  Exemplo de saída do dataframe:
             *  +------+--------------------+--------------+-----------+--------+-------------------------+---------------+
             *  | key  | value              | topic        | partition | offset | timestamp               | timestampType |
             *  +------+--------------------+--------------+-----------+--------+-------------------------+---------------+
             *  | null |[7B 22 74 72 61 6...| transactions |         0 |  18081 | 2021 - 01 - 20 19:45:...|             0 |
             *  +------+--------------------+--------------+-----------+--------+-------------------------+---------------+
             */

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
                                        Col("value").Cast("STRING"),
                                        schema.SimpleString)
                                    );

            /* Exemplo de saída do dataframe:
             * +----+--------------------+------------+---------+------+--------------------+-------------+--------------------+
             * | key|               value|       topic|partition|offset|           timestamp|timestampType|                json|
             * +----+--------------------+------------+---------+------+--------------------+-------------+--------------------+
             * |null|[7B 22 74 72 61 6...|transactions|        0| 18083|2021-01-20 19:49:...|            0|[6, 1949-0000-000...|
             * +----+--------------------+------------+---------+------+--------------------+-------------+--------------------+
             */

            df = df.Select("json.*");  // ... e retornando todas as colunas do array como um novo dataframe

            /* Exemplo de saída do dataframe:
             * +-----------+-------------------+---------+---------+-------+--------+--------------------+
             * |transaction|             number|      lat|      lng| amount|category|           eventTime|
             * +-----------+-------------------+---------+---------+-------+--------+--------------------+
             * |          8|1951-0000-0000-0001|-23.51418|-46.64638|71.1473|    pets|2021-01-20 19:51:...|
             * +-----------+-------------------+---------+---------+-------+--------+--------------------+
             */

            // Colocando um limite de 7 minutos para receber os eventos atrasados
            df = df.WithWatermark("eventTime", "7 minutes");

            // Somando os valores gastos, agrupando por categoria e por janelas de 2 minutos que se iniciam a cada 1 minuto
            df = df.GroupBy(Window(Col("eventTime"), "2 minutes", "1 minutes"), Col("category"))
                .Sum("amount").WithColumnRenamed("sum(amount)", "total")
                .Select(Col("window.start"), Col("window.end"), Col("category"), Col("total"));

            /* Exemplo de saída do dataframe:
             * +-------------------+-------------------+--------+--------+
             * |              start|                end|category|   total|
             * +-------------------+-------------------+--------+--------+
             * |2021-01-20 19:52:00|2021-01-20 19:54:00|   sport|83.88499|
             * |2021-01-20 19:53:00|2021-01-20 19:55:00|   sport|83.88499|
             * +-------------------+-------------------+--------+--------+
             */

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

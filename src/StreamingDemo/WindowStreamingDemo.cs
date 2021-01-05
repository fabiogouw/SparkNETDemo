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
    public class WindowStreamingDemo : IDemo
    {
        public void Run(string[] args)
        {
            string servidoresKafka = args[0];

            // Obtém a referência ao contexto de execução do Spark
            SparkSession spark = SparkSession
                .Builder()
                .AppName("Credit Card Category")
                .GetOrCreate();

            spark.Conf().Set("spark.sql.shuffle.partitions", "1");  // sem essa configuração, cada stage ficou com 200 tasks, o que levou uns 4 minutos pra cada batch executar

            // Criando um dataframe pra receber dados do Kafka
            DataFrame dfTransactions = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", servidoresKafka)
                .Option("subscribe", "transactions")
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
                            new StructField("transaction", new StringType()),
                            new StructField("number", new StringType()),
                            new StructField("lat", new DoubleType()),
                            new StructField("lng", new DoubleType()),
                            new StructField("amount", new DoubleType()),
                            new StructField("category", new StringType()),
                            new StructField("eventTime", new TimestampType())
                        });

            // Fazendo o parse do JSON pra um array ...
            dfTransactions = dfTransactions.WithColumn("json", Functions.FromJson(
                                            dfTransactions.Col("value"),
                                            schemaPackages.SimpleString)
                                        )
                .Select("json.*");  // ... e retornando todas as colunas do array como um novo dataframe

            // Gerando dois dataframes distintos para poder fazer o join e analisar a correção entre as transações
            DataFrame df1 = dfTransactions
                .WithWatermark("eventTime", "7 minutes");

            // Efetuando o join para verificar a correlação de transações dos cartões de crédito
            DataFrame df = df1.GroupBy(Functions.Window(Functions.Col("eventTime"), "4 minutes", "2 minutes"), Functions.Col("category"))
                .Sum("amount").WithColumnRenamed("sum(amount)", "total");

            // Colocando o streaming pra funcionar
            var writer = new MySQLForeachWriter();
            StreamingQuery query = df
                .WriteStream()
                .Format("console")
                .OutputMode(OutputMode.Update)
                .Foreach(writer)
                .Start();
            Console.ReadLine();
            Console.WriteLine("Parando query...");
            query.Stop();
        }
    }
}

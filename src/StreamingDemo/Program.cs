using Microsoft.Spark.Sql;
using static Microsoft.Spark.Sql.Functions;
using System;
using Microsoft.Spark.Sql.Streaming;
using Microsoft.Spark.Sql.Types;
using Microsoft.ML.Data;
using Microsoft.ML;

namespace StreamingDemo
{
    class Program
    {
        /* Copiar jars para a pasta do Hadoop spark-sql-kafka-0-10_2.11-2.4.5.jar e kafka-clients-2.4.0.jar
         * ou --packages org.apache.spark:spark-sql-kafka-0-10_2.12:2.4.5
         * %SPARK_HOME%\bin\spark-submit --class org.apache.spark.deploy.dotnet.DotnetRunner \
         * --master local bin\Debug\netcoreapp3.1\microsoft-spark-2.4.x-0.9.0.jar dotnet bin\Debug\netcoreapp3.1\StreamingDemo.dll
         */
        static void Main(string[] args)
        {
            string bootstrapServers = "localhost:9092";    //args[0];
            string topics = "test"; //args[1];
            string model = @"data\MLModel.zip";

            SparkSession spark = SparkSession
                .Builder()
                .AppName("StructuredKafkaWordCount")
                .GetOrCreate();

            spark.Udf().Register<string, bool>("MLudf", (text) => Sentiment(text, model));

            DataFrame lines = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", bootstrapServers)
                .Option("subscribe", topics)
                .Load()
                .SelectExpr("CAST(value AS STRING)");

            //lines = lines.WithColumn("RES", Functions.CallUDF("MLudf", lines.Col("value")));


            StructType schema = new StructType(new[]
{
                new StructField("cliente", new StringType()),
                new StructField("produto", new StringType()),
                new StructField("opiniao", new StringType())
            }); // struct<cliente:string,produto:string,valor_total:float>

            lines = lines.WithColumn("json", Functions.FromJson(lines.Col("value"), schema.SimpleString)).Select("json.*");
            lines = lines.WithColumn("Score", Functions.CallUDF("MLudf", lines.Col("opiniao")));

            StreamingQuery query = lines
                .WriteStream()
                .OutputMode(OutputMode.Update)
                .Format("console")
                //.Trigger(Trigger.Continuous(3000))
                //.Foreach(new RedisForeachWriter())
                .Start();

            query.AwaitTermination();
        }
        // Method to call ML.NET code for sentiment analysis
        // Code primarily comes from ML.NET Model Builder
        public static bool Sentiment(string text, string modelPath)
        {
            Console.WriteLine(text);
            var mlContext = new MLContext();

            ITransformer mlModel = mlContext
                .Model
                .Load(modelPath, out var modelInputSchema);

            PredictionEngine<Review, ReviewPrediction> predEngine = mlContext
                .Model
                .CreatePredictionEngine<Review, ReviewPrediction>(mlModel);

            ReviewPrediction result = predEngine.Predict(
                new Review { ReviewText = text });

            // Returns true for positive, false for negative
            return result.Prediction;
        }

        // Class to represent each review
        public class Review
        {
            // Column name must match input file
            [LoadColumn(0)]
            public string ReviewText;
        }

        // Class resulting from ML.NET code including predictions about review
        public class ReviewPrediction : Review
        {
            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }

            public float Probability { get; set; }

            public float Score { get; set; }
        }
    }
}
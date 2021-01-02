using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Streaming;
using Microsoft.Spark.Sql.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamingDemo
{
    class Program
    {
        /* Copiar jars para a pasta do Hadoop spark-sql-kafka-0-10_2.11-2.4.5.jar e kafka-clients-2.4.0.jar
         * ou --packages org.apache.spark:spark-sql-kafka-0-10_2.12:2.4.5
         * 
         * %SPARK_HOME%\bin\spark-submit 
         * --master local 
         * --class org.apache.spark.deploy.dotnet.DotnetRunner 
         * bin\Debug\net5.0\microsoft-spark-2-4_2.11-1.0.0.jar 
         * dotnet 
         * bin\Debug\net5.0\StreamingDemo.dll 
         * MLNETStreamingDemo 
         * localhost:9092 test 
         * data\MLModel.zip
         * 
         * %SPARK_HOME%\bin\spark-submit 
         * --master local 
         * --class org.apache.spark.deploy.dotnet.DotnetRunner 
         * bin\Debug\net5.0\microsoft-spark-2-4_2.11-1.0.0.jar 
         * dotnet 
         * bin\Debug\net5.0\StreamingDemo.dll 
         * StatefulStreamingDemo 
         * localhost:9092
         */
        static void Main(string[] args)
        {
            IDemo demo = _demos.Single(i => i.Key.ToUpper() == args[0].ToUpper()).Value();
            demo.Run(args.Skip(1).ToArray());
        }

        private static Dictionary<string, Func<IDemo>> _demos = new Dictionary<string, Func<IDemo>>()
        {
            ["MLNETStreamingDemo"] = () => new MLNETStreamingDemo(),
            ["StatefulStreamingDemo"] = () => new StatefulStreamingDemo()
        };
    }
}
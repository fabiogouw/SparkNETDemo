using Microsoft.Spark.Sql;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingDemo
{
    [Serializable]
    public class RedisForeachWriter : IForeachWriter
    {
        public void Close(Exception errorOrNull)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine(errorOrNull);
            Console.ResetColor();
        }

        public bool Open(long partitionId, long epochId)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Open: {partitionId} - {epochId}");
            Console.ResetColor();
            return true;
        }

        public void Process(Row row)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Value: {row.Get(0)}");
            Console.ResetColor();
        }
    }
}

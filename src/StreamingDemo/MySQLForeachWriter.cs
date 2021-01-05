using Microsoft.Spark.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StreamingDemo
{
    [Serializable]
    public class MySQLForeachWriter : IForeachWriter
    {
        [NonSerialized]
        private MySqlConnection _connection;
        public void Close(Exception errorOrNull)
        {
            Console.WriteLine($"==> Close");
            _connection.Close();
        }

        public bool Open(long partitionId, long epochId)
        {
            Console.WriteLine($"==> Open");
            _connection = new MySqlConnection("server=localhost; database=teste_spark; uid=spark_user; pwd=my-secret-password;");
            _connection.Open();
            return true;
        }

        public void Process(Row row)
        {
            Console.WriteLine($"==> {row.Get("window")} - {row.Get("category")} - {row.Get("total")}");
            var cmd = _connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = "REPLACE INTO totais_transactions (window_id, category, total) VALUES (@window, @category, @total);";
            cmd.Parameters.AddWithValue("@window", row.Get("window").ToString());
            cmd.Parameters.AddWithValue("@category", row.Get("category"));
            cmd.Parameters.AddWithValue("@total", row.Get("total"));
            var recs = cmd.ExecuteNonQuery();            
        }
    }
}

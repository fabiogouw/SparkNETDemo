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
        private string _connectionString;
        [NonSerialized]
        private MySqlConnection _connection;

        public MySQLForeachWriter()
        {

        }

        public MySQLForeachWriter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Close(Exception errorOrNull)
        {
            Console.WriteLine($"==> Close");
            _connection.Close();
        }

        public bool Open(long partitionId, long epochId)
        {
            Console.WriteLine($"==> Open");
            _connection = new MySqlConnection(_connectionString);
            _connection.Open();
            return true;
        }

        public void Process(Row row)
        {
            Console.WriteLine($"==> {row.Get("start")} - {row.Get("category")} - {row.Get("total")}");
            var cmd = _connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = "REPLACE INTO total_transactions (window_start, window_end, category, total) VALUES (@window_start, @window_end, @category, @total);";
            cmd.Parameters.AddWithValue("@window_start", row.Get("start"));
            cmd.Parameters.AddWithValue("@window_end", row.Get("end"));
            cmd.Parameters.AddWithValue("@category", row.Get("category"));
            cmd.Parameters.AddWithValue("@total", row.Get("total"));
            var recs = cmd.ExecuteNonQuery();            
        }
    }
}

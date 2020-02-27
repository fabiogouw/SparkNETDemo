using Microsoft.Spark.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StreamingDemo
{
    [Serializable]
    public class RedisForeachWriter : IForeachWriter
    {
        //private MySqlConnection _connection;
        public void Close(Exception errorOrNull)
        {
            //_connection.Close();
        }

        public bool Open(long partitionId, long epochId)
        {
            //_connection = new MySqlConnection("server=localhost; database=db_streaming; uid=spark_user; pwd=my-secret-password;");
            //_connection.Open();
            return true;
        }

        public void Process(Row row)
        {
            Console.WriteLine($"==> {row.Get("cliente")} - {row.Get("produto")} - {row.Get("opiniao")} - {row.Get("nota")}");
            //var cmd = _connection.CreateCommand() as MySqlCommand;
            //cmd.CommandText = @"INSERT INTO avaliacoes (cliente, produto, opiniao, nota) VALUES (@cliente, @produto, @opiniao, @nota);";
            //cmd.Parameters.AddWithValue("@cliente", row.Get("cliente"));
            //cmd.Parameters.AddWithValue("@produto", row.Get("produto"));
            //cmd.Parameters.AddWithValue("@opiniao", row.Get("opiniao"));
            //cmd.Parameters.AddWithValue("@nota", row.Get("nota"));
            //var recs = cmd.ExecuteNonQuery();            
        }
    }
}

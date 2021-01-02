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
    public class MLNETStreamingDemo : IDemo
    {
        public void Run(string[] args)
        {
            string servidoresKafka = args[0];
            string topico = args[1];
            string modelo = args[2];

            // Obtém a referência ao contexto de execução do Spark
            SparkSession spark = SparkSession
                .Builder()
                .AppName("Exemplo Streaming com Kafka")
                .GetOrCreate();

            // Criando um dataframe pra receber dados do Kafka
            DataFrame df = spark
                .ReadStream()
                .Format("kafka")
                .Option("kafka.bootstrap.servers", servidoresKafka)
                .Option("subscribe", topico)
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
            var schema = new StructType(new[]
{
                new StructField("cliente", new StringType()),
                new StructField("produto", new StringType()),
                new StructField("opiniao", new StringType())
            }); // struct<cliente:string,produto:string,valor_total:float>

            // Fazendo o parse do JSON pra um array ...
            df = df.WithColumn("json", Functions.FromJson(
                                            df.Col("value"),
                                            schema.SimpleString)
                                        )
                .Select("json.*");  // ... e retornando todas as colunas do array como um novo dataframe

            //Registrando uma função personalizada pra ser usada no dataframe
            spark.Udf().Register<string, float>("AnaliseDeSentimento",
                (texto) => AnalisarSentimento(texto, modelo));
            // Criando nova coluna nota com o resultado da análise de sentimento
            df = df.WithColumn("nota", Functions.CallUDF("AnaliseDeSentimento", df.Col("opiniao")));

            // Colocando o streaming pra funcionar
            StreamingQuery query = df
                .WriteStream()
                .OutputMode(OutputMode.Append)
                .Format("console")
                //.Trigger(Trigger.Continuous(2000))
                //.Foreach(new RedisForeachWriter())
                .Start();

            query.AwaitTermination();   // Necessário pra deixar a aplcação no ar para processar os dados

        }

        public static float AnalisarSentimento(string texto, string caminhoDoModelo)
        {
            var contexto = new MLContext();
            ITransformer modelo = contexto.Model.Load(caminhoDoModelo, out var modelInputSchema);
            PredictionEngine<Avaliacao, ResultadoPredicao> predEngine = contexto.Model.CreatePredictionEngine<Avaliacao, ResultadoPredicao>(modelo);
            ResultadoPredicao resultado = predEngine.Predict(new Avaliacao { TextoAvaliacao = texto });
            return resultado.Nota;
        }

        public class Avaliacao
        {
            [ColumnName("Avaliacao"), LoadColumn(0)]
            public string TextoAvaliacao { get; set; }

            [ColumnName("Sentimento"), LoadColumn(1)]
            public bool Sentimento { get; set; }
        }

        public class ResultadoPredicao
        {
            [ColumnName("PredictedLabel")]
            public bool Predicao { get; set; }

            [ColumnName("Score")]
            public float Nota { get; set; }
        }

    }
}

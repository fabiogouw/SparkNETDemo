using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Types;
using System;
using System.Collections.Generic;
using static Microsoft.Spark.Sql.Functions; // Para acessar as funções estáticas Col, RegexpReplace, etc.

namespace BatchDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Copiar mysql-connector-java-8.0.19.jar para pasta do Spark / Hadoop
             * Rodar o comando abaixo a partir da pasta inicial deste projeto:
             *   %SPARK_HOME%\bin\spark-submit 
             *   --master local 
             *   --class org.apache.spark.deploy.dotnet.DotnetRunner 
             *   bin\Debug\netcoreapp3.1\microsoft-spark-2-4_2.11-1.0.0.jar 
             *   dotnet 
             *   bin\Debug\netcoreapp3.1\BatchDemo.dll 
             *   data\amostra.csv 
             *   jdbc:mysql://localhost:3306/teste_spark beneficios spark_user my-secret-password
             */

            if (args.Length == 0)
            {
                throw new ArgumentException("Informar os caminhos onde encontrar os arquivos CSV");
            }

            string arquivoEntrada = args[0];

            // Obtém a referência ao contexto de execução do Spark
            SparkSession spark = SparkSession
                .Builder()
                .AppName("Exemplo Batch")
                .GetOrCreate();

            // Definindo um schema fixo, com os nomes de coluna que eu quero e seus tipos
            StructType schema = new StructType(new[]
            {
                new StructField("MES_REFERENCIA", new StringType()),
                new StructField("MES_COMPETENCIA", new StringType()),
                new StructField("UF", new StringType()),
                new StructField("CODIGO_MUNICIPIO", new IntegerType()),
                new StructField("MUNICIPIO", new StringType()),
                new StructField("CODIGO_FAVORECIDO", new StringType()),
                new StructField("NOME", new StringType()),
                new StructField("DATA_SAQUE", new DateType()),
                new StructField("VALOR_TEXTO", new StringType())
            });

            // Leitura dos dados em disco para dentro do Spark
            DataFrame df = spark.Read()
                .Format("csv")
                .Schema(schema)
                .Option("sep", ";")
                .Option("header", true)
                .Option("dateFormat", "dd/MM/yyyy")
                .Load(arquivoEntrada);
            df.PrintSchema();
            df.Show(5, 10);
            
            // Removendo colunas que não precisamos mais
            df = df.Drop("MES_REFERENCIA")
                .Drop("MES_COMPETENCIA")
                .Drop("CODIGO_MUNICIPIO")
                .Drop("CODIGO_FAVORECIDO");
            df.Show(5, 10);

            // Convertendo a coluna VALOR de string para decimal, considerando que o padrão brasileiro é diferente do americano
            df = df.WithColumn("VALOR", RegexpReplace(
                                            RegexpReplace(
                                                df.Col("VALOR_TEXTO")
                                            , "\\.", "")
                                        , ",", ".")
                                    .Cast("decimal(10,2)"))
                .Drop("VALOR_TEXTO");
            df.PrintSchema();
            df.Show(5, 10);

            // Efetuando um filtro em cima dos dados
            df = df.Where(df.Col("UF").NotEqual("AC"));
            //df = df.Where("UF <> 'AC'");  // passar uma expressão WHERE também funciona como filtro
            df.Show(5, 10);

            spark.Udf().Register<string, string, string>("ConcatenarMunicipio",
                (uf, municipio) => ConcatenarMunicipio(uf, municipio));

            // Criando uma nova coluna a partir de uma concatenação e removendo colunas antigas e que não precisamos mais
            df = df.WithColumn("MUNICIPIO",
                CallUDF("ConcatenarMunicipio", df.Col("UF"), df.Col("MUNICIPIO")))
                .Drop("UF");
            // Efetuando uma agregação
            DataFrame somatorio = df.GroupBy("MUNICIPIO")
                .Sum("VALOR")
                .WithColumnRenamed("sum(VALOR)", "SOMA_BENEFICIOS");
            somatorio
                .OrderBy(somatorio.Col("SOMA_BENEFICIOS").Desc())
                .Show(15, 40);

            if (args.Length >= 2)
            {
                string urlJdbc = args[1];   // jdbc:mysql://localhost:3306/teste_spark
                string tabela = args[2];    // beneficios
                string usuario = args[3];   // spark_user
                string senha = args[4];     // my-secret-password
                
                // Salvando em banco de dados com funcionalidade nativa do Spark
                somatorio
                    .Write()
                    .Format("jdbc")
                    .Option("driver", "com.mysql.cj.jdbc.Driver")
                    .Option("url", "jdbc:mysql://localhost:3306/teste_spark")
                    .Option("dbtable", "beneficios")
                    .Option("user", "spark_user")
                    .Option("password", "my-secret-password")
                    .Mode(SaveMode.Overwrite)
                    .Save();
            }
            spark.Stop();
        }

        public static string ConcatenarMunicipio(string uf, string municipio)
        {
            return $"{uf} - {municipio}";
        }
    }
}

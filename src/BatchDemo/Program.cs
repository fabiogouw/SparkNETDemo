using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Types;
using System;
using System.Collections.Generic;

namespace BatchDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Copiar mysql-connector-java-8.0.19.jar para pasta do Spark / Hadoop
             * Rodar o comando abaixo a partir da pasta inicial deste projeto:
             *   %SPARK_HOME%\bin\spark-submit --class org.apache.spark.deploy.dotnet.DotnetRunner \
                 --master local \
                 bin\Debug\netcoreapp3.1\microsoft-spark-2.4.x-0.10.0.jar dotnet bin\Debug\netcoreapp3.1\BatchDemo.dll \
                 data\amostra.csv \
                 jdbc:mysql://localhost:3306/db_streaming beneficios spark_user my-secret-password
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
                new StructField("VALOR", new StringType())
            });

            // Leitura dos dados em disco para dentro do Spark
            DataFrame df = spark.Read()
                .Format("csv")
                .Schema(schema)
                //.Option("inferSchema", true)
                .Option("delimiter", ";")
                .Option("header", true)
                .Option("dateFormat", "dd/MM/yyyy")
                .Load(arquivoEntrada);

            // Convertendo a coluna VALOR de string para decimal, considerando que o padrão brasileiro é diferente do americano
            df = df.WithColumn("VALOR", Functions.RegexpReplace(
                                            Functions.RegexpReplace(Functions.Col("VALOR"), "\\.", "")
                                            , ",", ".").Cast("decimal(10,2)"));
            df.PrintSchema();
            df.Show(10, 100);

            // Efetuando um filtro em cima dos dados
            df = df.Where(Functions.Col("UF").NotEqual("AC"));
            //df = df.Where("UF <> 'AC'");  // passar uma expressão WHERE também funciona como filtro
            df.Show(10, 100);

            // Executando uma "query SQL" em cima dos dados 
            df.CreateOrReplaceTempView("filtrados");
            spark.Sql("SELECT NOME, MUNICIPIO, VALOR FROM filtrados WHERE UF = 'SP' AND VALOR >= 200")
                .Show(10, 100);

            // Criando um novo dataframe na mão
            StructType schemaEstados = new StructType(new[]
            {
                new StructField("UF", new StringType()),
                new StructField("NOME_UF", new StringType())
            });
            DataFrame estados = spark.CreateDataFrame(new[]
            {
                new GenericRow(new object[] { "AC", "ACRE" }),
                new GenericRow(new object[] { "AL", "ALAGOAS" }),
                new GenericRow(new object[] { "AP", "AMAPA" }),
                new GenericRow(new object[] { "AM", "AMAZONAS" }),
                new GenericRow(new object[] { "BA", "BAHIA" }),
                new GenericRow(new object[] { "CE", "CEARA" }),
                new GenericRow(new object[] { "DF", "DISTRITO FEDERAL" }),
                new GenericRow(new object[] { "ES", "ESPÍRITO SANTO" }),
                new GenericRow(new object[] { "GO", "GOIAS" }),
                new GenericRow(new object[] { "MA", "MARANHAO" }),
                new GenericRow(new object[] { "MT", "MATO GROSSO" }),
                new GenericRow(new object[] { "MS", "MATO GROSSO DO SUL" }),
                new GenericRow(new object[] { "MG", "MINAS GERAIS" }),
                new GenericRow(new object[] { "PA", "PARA" }),
                new GenericRow(new object[] { "PB", "PARAÍBA" }),
                new GenericRow(new object[] { "PR", "PARANA" }),
                new GenericRow(new object[] { "PE", "PERNAMBUCO" }),
                new GenericRow(new object[] { "PI", "PIAUI" }),
                new GenericRow(new object[] { "RJ", "RIO DE JANEIRO" }),
                new GenericRow(new object[] { "RN", "RIO GRANDE DO NORTE" }),
                new GenericRow(new object[] { "RS", "RIO GRANDE DO SUL" }),
                new GenericRow(new object[] { "RO", "RONDÔNIA" }),
                new GenericRow(new object[] { "RR", "RORAIMA" }),
                new GenericRow(new object[] { "SC", "SANTA CATARINA" }),
                new GenericRow(new object[] { "SP", "SAO PAULO" }),
                new GenericRow(new object[] { "SE", "SERGIPE" }),
                new GenericRow(new object[] { "TO", "TOCANTINS" })
            }, schemaEstados);
            // Efetuando o join em cima da coluna UF, que é a chave dos dois dataframes
            df = df.Join(estados, "UF");
            df.PrintSchema();
            df.Show(10, 100);

            // Criando uma nova coluna a partir de uma concatenação, e removendo as antigas
            df = df.WithColumn("CIDADE", Functions.Concat(df.Col("NOME_UF"), Functions.Lit(" - "), df.Col("MUNICIPIO")))
                .Drop("UF")
                .Drop("NOME_UF")
                .Drop("CODIGO_MUNICIPIO")
                .Drop("MUNICIPIO");
            df.PrintSchema();
            df.Show(10, 100);

            // Efetuando uma agregação
            DataFrame somatorio = df.GroupBy("CIDADE")
                .Sum("VALOR")
                .WithColumnRenamed("sum(VALOR)", "SOMA_BENEFICIOS")
                .OrderBy(Functions.Col("SOMA_BENEFICIOS").Desc());
            somatorio.PrintSchema();
            somatorio.Show(15, 100);

            if (args.Length >= 2)
            {
                string urlJdbc = args[1];   // jdbc:mysql://localhost:3306/db_streaming
                string tabela = args[2];    // beneficios
                string usuario = args[3];   // spark_user
                string senha = args[4];     // my-secret-password

                var propriedades = new Dictionary<string, string>()
                {
                    { "user", usuario },
                    { "password", senha }
                };
                // Salvando em banco de dados com funcionalidade nativa do Spark
                somatorio
                    .Write()
                    .Mode(SaveMode.Overwrite)
                    .Option("driver", "com.mysql.cj.jdbc.Driver")
                    .Jdbc(urlJdbc, tabela, propriedades);
            }
            spark.Stop();
        }
    }
}

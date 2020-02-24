using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Types;
using System;

namespace BatchDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Informar os caminhos onde encontrar os arquivos CSV", ConsoleColor.Red);
                return;
            }

            string input = args[0];
            string output = args.Length >= 2 ? args[1] : string.Empty;

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
                .Load(input);

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

            // Efetuando uma agregação
            DataFrame summary = df.GroupBy("DATA_SAQUE")
                .Sum("VALOR")
                .WithColumnRenamed("sum(VALOR)", "SOMA_BENEFICIOS_DIA")
                .OrderBy(Functions.Col("DATA_SAQUE").Asc());
            summary.Show(100);

            // Salvando os resultados em disco
            if(!string.IsNullOrEmpty(output))
            {
                summary
                    .Coalesce(1)
                    .Write()
                    .Mode(SaveMode.Overwrite)
                    .Option("header", true)
                    .Format("csv")
                    .Save(output);
            }

            spark.Stop();
        }
    }
}

# Usando .NET com Spark

Olá! Este é um projeto de demonstração em como utilizar o C# com o Apache Spark.

Aqui estou utilizando o componente .NET for Apache Spark, disponível para download no Nuget (pacote Microsoft.Spark). Maiores detalhes podem ser encontrados no site oficial da biblioteca (https://dotnet.microsoft.com/apps/data/spark) e no seu repositório do GitHub (https://github.com/dotnet/spark).

## Instalação

As instruções de instalação e configuração do Apache Spark e do .NET for Apache Spark se encontram neste link: https://dotnet.microsoft.com/learn/data/spark-tutorial/intro

Além disso, para essa demonstração em específico, são necessários alguns serviços rodando. O exemplo de batch precisa de um servidor MySQL e o exemplo de streaming precisa do Kafka rodando (que por sua vez necessita de uma instância do Zookeeper). Optei por subir esses serviços como contêineres no Docker, para facilitar a disponibilização do ambiente. Abaixo estão os comandos necessários para configurar esses contêineres.

````
docker run -d --name zookeeper -p 2181:2181 confluent/zookeeper

docker run -d --name kafka -p 9092:9092 --link zookeeper:zookeeper --env KAFKA_ADVERTISED_HOST_NAME=localhost confluent/kafka

docker run --name mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=my-secret -d mysql:latest
````

### "Pegadinhas"

Para poder executar corretamente, esses exemplos precisam de algumas bibliotecas para acesso ao MySQL e Kafka. Além disso, pra conseguirmos rodar o exemplo de Streaming que usa o ML.NET, vamos precisar deixar um dos seus componentes disponíveis na pasta que está apontada a variável de ambiente DOTNET_WORKER_DIR. 

- Copiar mysql-connector-java-8.0.19.jar para pasta do Spark / Hadoop
- Copiar jars para a pasta do Hadoop spark-sql-kafka-0-10_2.11-2.4.5.jar e kafka-clients-2.4.0.jar
- Copiar a dll Microsoft.ML.DataView.dll para a pasta DOTNET_WORKER_DIR

Os jars podem ser encontrados e baixados no site do Maven. A dll pode ser encontrada na pasta de binários que é gerada durante a compilação do projeto.

Obs. Não vou mentir que essa é uma solução boa, mas funciona para fins de exemplo, ok?

## Execução

Ambos exemplos são executados através de um terminal, por linha de comando. Para funcionar, é importante deixar o terminal na mesma pasta raiz do projeto.

### Batch

````
   %SPARK_HOME%\bin\spark-submit \
   --master local \
   --class org.apache.spark.deploy.dotnet.DotnetRunner \
   bin\Debug\net5.0\microsoft-spark-2-4_2.11-1.0.0.jar \
   dotnet \
   bin\Debug\net5.0\BatchDemo.dll \
   data\amostra.csv \
   jdbc:mysql://localhost:3306/teste_spark beneficios spark_user my-secret-password
````

### Streaming

````
	%SPARK_HOME%\bin\spark-submit \
	--master local \
	--class org.apache.spark.deploy.dotnet.DotnetRunner \
	bin\Debug\net5.0\microsoft-spark-2-4_2.11-1.0.0.jar \
	dotnet \
	bin\Debug\net5.0\StreamingDemo.dll \
	localhost:9092 test \
	data\MLModel.zip
````
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

### Banco de Dados

Alguns exemplos consideram a gravação dos resultados de processamento em banco de dados MySQL. Abaixo estão os scripts para criar as duas tabelas necessárias.

````
CREATE TABLE `beneficios` (
  `MUNICIPIO` text,
  `SOMA_BENEFICIOS` decimal(20,2) DEFAULT NULL
);

CREATE TABLE `total_transactions` (
  `window_start` datetime NOT NULL,
  `window_end` datetime NOT NULL,
  `category` varchar(45) NOT NULL,
  `total` decimal(15,4) DEFAULT NULL,
  PRIMARY KEY (`window_start`,`window_end`,`category`)
);
````

## Execução

Ambos exemplos são executados através de um terminal, por linha de comando. Para funcionar, é importante deixar o terminal na mesma pasta raiz do projeto.

### Batch

Este é um exemplo que carrega um arquivo CSV contendo dados de pagamento de benefício do Bolsa Familia e executa algumas operações em cima do DataFrame, como agregações.

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

#### ML.NET

Este é um exemplo que "ouve" mensagens de reviews de clientes para determinados produtos e, de acordo com um modelo treinado com ML.NET, efetua a análise de sentimento do texto para verificar se é uma reação positiva ou negativa.

````
%SPARK_HOME%\bin\spark-submit \
--master local \
--class org.apache.spark.deploy.dotnet.DotnetRunner \
bin\Debug\net5.0\microsoft-spark-2-4_2.11-1.0.0.jar \
dotnet \
bin\Debug\net5.0\StreamingDemo.dll \
MLNETStreamingDemo \
localhost:9092 test \
data\MLModel.zip
````

#### Window

Este exemplo utiliza um gerador de transações de cartão de crédito para agrupar a soma das transações pela categoria da compra e pelo momento que ela ocorreu (em janelas de tempo).

````
%SPARK_HOME%\bin\spark-submit \
--master local \
--class org.apache.spark.deploy.dotnet.DotnetRunner \
bin\Debug\net5.0\microsoft-spark-2-4_2.11-1.0.0.jar \
dotnet \
bin\Debug\net5.0\StreamingDemo.dll \
WindowStreamingDemo \
localhost:9092 \
"server=localhost; database=teste_spark; uid=spark_user; pwd=my-secret-password;"
````

#### Join

Este exemplo também trata um stream de transações de cartão de crédito, mas neste caso ele avalia se uma transação tem indícios de fraude, que no caso é dado um mesmo cartão, uma nova transação é efetuada dentro de um intervalo de tempo, mas em uma localidade bem distante. É baseado [neste exemplo](https://www.confluent.io/blog/atm-fraud-detection-apache-kafka-ksql/) de análise de fraudes com KSQL.

````
%SPARK_HOME%\bin\spark-submit \
--master local \
--class org.apache.spark.deploy.dotnet.DotnetRunner \
bin\Debug\net5.0\microsoft-spark-2-4_2.11-1.0.0.jar \
dotnet \
bin\Debug\net5.0\StreamingDemo.dll \
JoinStreamingDemo \
localhost:9092 200
````
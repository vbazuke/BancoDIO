# BancoDIO

Projeto desenvolvido para o bootcamp .NET Fundamentals da DIO. Este é o repo com a aplicação original criada por Eliézer Zarpelão https://github.com/elizarp/dio-dotnet-poo-lab-1.

Trata-se de uma aplicação de console simulando um sistema de gerenciamento bancário simples, com suporte a saque, depósitos e transferência entre contas. Mais informações estão na página do repositório original.

O projeto teve as seguintes alterações como forma de aprendizado:

1.

A forma de acesso e manipulação dos dados foi mudada de uma simples lista alocada temporariamente em memória, para a utilização de um banco embutido em arquivo, utilizando SqLite.
Assim, é possível persistir e preservar os dados entre execuções, na versão original não havia forma de persistir os dados por um prazo maior do que a execução do programa. Foi utilizado a ORM Entity Framework para implementar o acesso e manipulação do banco dentro do ambiente .NET.

2.

Foi adicionada uma feature para consulta de valor atual para compra de dólar, consultando/consumindo uma API pública que expõe dados do câmbio sempre atualizados. O programa também mostra a quantidade máxima de dólares que poderão ser comprados com o saldo atual da conta-corrente do cliente.

Requisitos:

.NET Core 3.1 +

Para rodar em linha de comando:

`dotnet run`

Para buildar o projeto:

`dotnet build`

Caso opte por rodar o projeto dentro do Visual Studio, utilize o caminho absoluto do projeto ao passar a string com o caminho do banco de dados, ao invés de apontar para o arquivo diretamente, (Data Source). Essa mudança deve ser feita no arquivo [a link](https://github.com/vbazuke/BancoDIO/blob/main/BancoContext.cs)

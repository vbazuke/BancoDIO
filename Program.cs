using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace BancoDIO
{
	class Program
	{
		static BancoContext context = new BancoContext();
		static List<Conta> listContas;

		static void Main(string[] args)
		{
			string opcaoUsuario = ObterOpcaoUsuario();

			while (opcaoUsuario.ToUpper() != "X")
			{
				switch (opcaoUsuario)
				{
					case "1":
						ListarContas();
						break;
					case "2":
						InserirConta();
						break;
					case "3":
						Transferir();
						break;
					case "4":
						Sacar();
						break;
					case "5":
						Depositar();
						break;
					case "6":
						ConsultarDolar();
						break;
					case "C":
						Console.Clear();
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}

				opcaoUsuario = ObterOpcaoUsuario();
			}

			Console.WriteLine("Obrigado por utilizar nossos serviços.");
			Console.ReadLine();
		}

		private static void Depositar()
		{

			listContas = context.Contas.ToList();

			Console.Write("Digite o número Id da conta: ");
			int indiceConta = int.Parse(Console.ReadLine());

			Console.Write("Digite o valor a ser depositado: ");
			double valorDeposito = double.Parse(Console.ReadLine());

			var conta = context.Contas.Find(indiceConta);

			var indiceDestino = listContas.Select((s, i) => new { i, s })
			.Where(t => t.s.Id == indiceConta)
			.Select(t => t.i)
			.ToList()[0];

			listContas[indiceDestino].Depositar(valorDeposito);

			var contaDestino = context.Contas.Find(listContas[indiceDestino].Id);
			context.SaveChanges();

		}

		private static void Sacar()
		{
			listContas = context.Contas.ToList();

			Console.Write("Digite o número Id da conta: ");
			int idConta = int.Parse(Console.ReadLine());

			var conta = context.Contas.Find(idConta);

			var indiceList = listContas.Select((s, i) => new { i, s })
			.Where(t => t.s.Id == idConta)
			.Select(t => t.i)
			.ToList()[0];

			Console.Write("Digite o valor a ser sacado: ");
			double valorSaque = double.Parse(Console.ReadLine());

			if (listContas[indiceList].Sacar(valorSaque))
			{
				var contaDestino = context.Contas.Find(listContas[indiceList].Id);				
				context.SaveChanges();
			}

			else 
			{
				Console.WriteLine("Não foi possível realizar o saque.");
			}

		}

		private static void ConsultarDolar()
		{			

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://economia.awesomeapi.com.br/");				
				var responseTask = client.GetAsync("json/last/USD-BRL"); ;
				responseTask.Wait();
				var result = responseTask.Result;

				if (result.IsSuccessStatusCode)
				{
					var readTask = result.Content.ReadAsStringAsync();
					readTask.Wait();
					var students = readTask.Result;
					var coin = JsonSerializer.Deserialize<MoedaModel>(students);

					var formatado = coin.USDBRL.ask.Replace('.', ',');

					Console.WriteLine("No momento um dólar está valendo R$ {0}", formatado);

					Console.Write("Digite o número Id da conta compradora interessada:");
					int indiceContaDestino = int.Parse(Console.ReadLine());

					var conta = context.Contas.Find(indiceContaDestino);

					Console.WriteLine("cotacao é {0} ", formatado);

					double qtdDolares = (int) (conta.Saldo/double.Parse(formatado));

					string formatadoFinal = String.Format("{0:n0}",qtdDolares);

					Console.WriteLine("Seu saldo é de {0}. O máximo de dólares que você poderá comprar é {1}",conta.Saldo , formatadoFinal);
				}
			}
		}

		private static void Transferir()
		{

			listContas = context.Contas.ToList();

			Console.Write("Digite o Id da conta de origem: ");
			int indiceContaOrigem = int.Parse(Console.ReadLine());

			Console.Write("Digite o Id da conta de destino: ");
			int indiceContaDestino = int.Parse(Console.ReadLine());

			Console.Write("Digite o valor a ser transferido: ");
			double valorTransferencia = double.Parse(Console.ReadLine());

			var indiceOrigem = listContas.Select((s, i) => new { i, s })
			.Where(t => t.s.Id == indiceContaOrigem)
			.Select(t => t.i)
			.ToList()[0];

			var indiceDestino = listContas.Select((s, i) => new { i, s })
			.Where(t => t.s.Id == indiceContaDestino)
			.Select(t => t.i)
			.ToList()[0];

			if (indiceOrigem > -1 && indiceDestino > -1)
			{

				if (listContas[indiceOrigem].Transferir(valorTransferencia, listContas[indiceDestino]))
				{					
					var contaOrigem = context.Contas.Find(listContas[indiceOrigem].Id);
					var contaDestino = context.Contas.Find(listContas[indiceDestino].Id);					

					contaOrigem.Saldo = listContas[indiceOrigem].Saldo;
					contaDestino.Saldo = listContas[indiceDestino].Saldo;

					context.SaveChanges();
				}

				else 
				{
					Console.WriteLine("Não foi possível realizar transferência");
				}

			}
		}

		private static void InserirConta()
		{
			Console.WriteLine("Inserir nova conta");

			Console.Write("Digite 1 para Conta Fisica ou 2 para Juridica: ");
			int entradaTipoConta = int.Parse(Console.ReadLine());

			Console.Write("Digite o Nome do Cliente: ");
			string entradaNome = Console.ReadLine();

			Console.Write("Digite o saldo inicial: ");
			double entradaSaldo = double.Parse(Console.ReadLine());

			Console.Write("Digite o crédito: ");
			double entradaCredito = double.Parse(Console.ReadLine());

			var novaConta = new Conta();		

			novaConta.TipoConta = (TipoConta)entradaTipoConta;
			novaConta.Saldo = entradaSaldo;
			novaConta.Credito = entradaCredito;
			novaConta.Nome = entradaNome;

			context.Contas.Add(novaConta);
			context.SaveChanges();
		}

		private static void ListarContas()
		{
			listContas = context.Contas.ToList();

			Console.WriteLine("Listar contas");

			if (listContas.Count == 0)
			{
				Console.WriteLine("Nenhuma conta cadastrada.");
				return;
			}

			for (int i = 0; i < listContas.Count; i++)
			{
				Conta conta = listContas[i];
				Console.Write("#{0} - ", i);
				Console.WriteLine(conta);
			}
		}

		private static string ObterOpcaoUsuario()
		{
			Console.WriteLine();
			Console.WriteLine("DIO Bank a seu dispor!!!");
			Console.WriteLine("Informe a opção desejada:");

			Console.WriteLine("1- Listar contas");
			Console.WriteLine("2- Inserir nova conta");
			Console.WriteLine("3- Transferir");
			Console.WriteLine("4- Sacar");
			Console.WriteLine("5- Depositar");
			Console.WriteLine("6- Consultar dólar");
			Console.WriteLine("C- Limpar Tela");
			Console.WriteLine("X- Sair");
			Console.WriteLine();

			string opcaoUsuario = Console.ReadLine().ToUpper();
			Console.WriteLine();
			return opcaoUsuario;
		}
	}
}

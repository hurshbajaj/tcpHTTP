using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using parser;

namespace HTTPbuilder
{
	internal class MainHoldr
	{
		static void Main(){}
	}

	public class HTTPbuild
	{
		private int PORT;
		private IPAddress address;
		private TcpListener server;
		private int currentI;

		private List<TcpClient> clients;

		public struct buildARG
		{
			public int Port;
			public IPAddress Address;

			public buildARG(int port, IPAddress address)
			{
				this.Port = port;
				this.Address = address != null? address:IPAddress.Parse("127.0.0.1");
			}
		}
		
		// public struct resObj
		// {
		// 	public string body {get;set;}
		// 	public string status {get;set;}
		// }
		
		public delegate void CHandlerD(Dictionary<string, string> parsedReq, out Dictionary<string, string> res);
		
		public HTTPbuild(buildARG src)
		{
			this.currentI = 0;
			this.PORT = src.Port;
			this.address = src.Address;
			this.server = new TcpListener(this.address, this.PORT);
		
			this.clients = new List<TcpClient>();
			
			server.Start();
		}
		public async Task listen()
		{
			Task.Run(async () =>
			{
				TcpClient client = await this.server.AcceptTcpClientAsync();
				Console.WriteLine("Listening on port " + this.PORT);
				this.clients.Add(client);
				this.listen();

			});

		}

		public async Task handle(string method, string url, CHandlerD CHandler)
		{
			
			while (true)
			{
				if(this.currentI < this.clients.Count)
				{
					//Console.WriteLine("FLUX");
					_ = Task.Run(async () => await this.handler(this.clients[this.clients.Count-1], method, url, CHandler));
					this.currentI++;
				}
			}
			
		}

		private async Task handler(TcpClient client, string method, string url,CHandlerD CHandler)
		{
			Console.WriteLine("Handling " + method);
			
			using NetworkStream stream = client.GetStream();
			using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
			using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8){AutoFlush = true};

			string req;

			Task.Run(async () =>
			{
				while ((req = await reader.ReadLineAsync()) != null)
				{
					try
					{
						Dictionary<string, string> parsedReq = Parser.parse(req);

						Console.WriteLine("FINALLY GOOD LORD");
						if (parsedReq["method"] == method && parsedReq["url"] == url)
						{
							Console.WriteLine("COUGHT");
							Dictionary<string, string> res = newRes();
							CHandler(parsedReq, out res);
							
							res["status"] = res["status"] == null ? "200 OK" : res["status"];
							
							string Response = $"{parsedReq["httpV"]} {res["status"]} \n";
							string resBody = res["body"];
							foreach (var property in res)
							{
								if (property.Key != "status" && property.Key != "body")
								{
									Response += $"{property.Value}\n";
								}
							}

							if (resBody != null)
							{
								Response += $"\n\n{resBody}";
							}

							Console.WriteLine(Response);
							writer.WriteLineAsync(Response);
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("Expected HTTP/ Bad Request... " + e);
						throw e;
					}
					

				}
			});
			while(true){}
			
		}

		public static Dictionary<string, string> newRes()
		{
			return new Dictionary<string, string>
			{
				{"body", null},
				{"status", null},
				{"Content-Type", "text/plain"}
			};
		}
		
	}
}	

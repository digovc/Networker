using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Networker.Client;
using Networker.Extensions.Json;
using Networker.Server;

namespace Demo.PacketModule
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new ServerBuilder()
				.UseTcp(1000)
				.UseUdp(5000)
				.UseJson()
				.ConfigureLogging(loggingBuilder =>
				{
					loggingBuilder.AddConsole();
					loggingBuilder.SetMinimumLevel(
						LogLevel.Debug);
				})
				.RegisterModule<BasicServerPacketModule>()
				.Build();

			server.Start();

			try
			{
				var client = new ClientBuilder()
					.UseIp("127.0.0.1")
					.UseTcp(1000)
					.UseUdp(5000)
					.UseJson()
					.Build();

				client.Connect();

                for (var i = 0; i < 4; i++)
                {
                    Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            client.Send(new BasicPacket
                            {
                                StringData = DateTime.UtcNow.ToString()
                            });

                            Thread.Sleep(1);
                        }
                    });
                }
            }
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.ReadLine();
			Console.ReadLine();
		}
	}
}

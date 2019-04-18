using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Demo.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Networker.Common;
using Networker.Common.Abstractions;
using Networker.Extensions.Json;
using Networker.Server;
using Networker.Server.Abstractions;

namespace Networker.Tests.ServerPerformance
{
	public class Program
	{

		[CoreJob]
		[MemoryDiagnoser]
		//[InliningDiagnoser]
		public class ServerPacketProcessTest
		{
			[Params(1000)] public int N;
			private IServerPacketProcessor _packetProcessor;
			private byte[] _serialised;
			private TcpSender _sender;

			[GlobalSetup]
			public void Setup()
			{
				var server = new ServerBuilder().UseTcp(1000)
					.UseUdp(5000)
					.ConfigureLogging(loggingBuilder =>
					{
						loggingBuilder.AddConsole();
						loggingBuilder.SetMinimumLevel(
							LogLevel.Debug);
					})
					.RegisterTypes(collection => { collection.AddSingleton<IPacketSerialiser, DoNothingSerialiser>(); })
					.Build();

				server.Start();

				_packetProcessor = server.ServiceProvider.GetService<IServerPacketProcessor>();
				var serialiser = server.ServiceProvider.GetService<IPacketSerialiser>();

				_serialised = serialiser.Serialise(new PerformancePacket
				{
					SomeData = Guid.NewGuid().ToString()
				});
				_sender = new TcpSender(serialiser);
			}

			[Benchmark]
			public void ProcessPacket()
			{
				_packetProcessor.ProcessFromBuffer(_sender, _serialised);
			}
		}

		public static void Main(string[] args)
		{
			//var test = new ServerPacketProcessTest();
			//test.Setup();
			//test.ProcessPacket();
			
			var summary = BenchmarkRunner.Run<ServerPacketProcessTest>();

			Console.ReadKey();
			Console.ReadKey();
		}
	}
}

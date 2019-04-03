﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Networker.Common.Abstractions
{
	public interface IBuilder<TBuilder, TResult>
	{
		//Build
		TResult Build();
		TBuilder ConfigureLogging(Action<ILoggingBuilder> loggingBuilder);

		//Service Collection
		IServiceCollection GetServiceCollection();

		IServiceProvider GetServiceProvider();

		TBuilder RegisterMiddleware<T>()
			where T : class, IMiddlewareHandler;

		//Packet Handler
		TBuilder RegisterPacketHandler<TPacket, TPacketHandler>()
			where TPacket : class where TPacketHandler : IPacketHandler;

		[Obsolete("Please use the new IPacketModule")]
		TBuilder RegisterPacketHandlerModule(IPacketHandlerModule packetHandlerModule);
		
		[Obsolete("Please use the new IPacketModule")]
		TBuilder RegisterPacketHandlerModule<T>()
			where T : IPacketHandlerModule;
		
		TBuilder RegisterModule(IPacketModule packetHandlerModule);

		TBuilder RegisterModule<T>()
			where T : IPacketModule;

		TBuilder RegisterTypes(Action<IServiceCollection> serviceCollection);

		//Info
		TBuilder SetPacketBufferSize(int size);

		TBuilder SetServiceCollection(IServiceCollection serviceCollection,
			Func<IServiceProvider> serviceProviderFactory = null);

		TBuilder UseConfiguration(IConfiguration configuration);

		TBuilder UseConfiguration<T>(IConfiguration configuration)
			where T : class;

		//Tcp
		TBuilder UseTcp(int port);

		//Udp
		TBuilder UseUdp(int port);
	}
}
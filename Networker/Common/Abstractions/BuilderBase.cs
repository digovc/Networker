using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Networker.Common.Abstractions
{
	public abstract class BuilderBase<TBuilder, TResult, TBuilderOptions> : IBuilder<TBuilder, TResult>
		where TBuilder : class, IBuilder<TBuilder, TResult> where TBuilderOptions : class, IBuilderOptions
	{
		private IConfiguration configuration;

		private Action<ILoggingBuilder> loggingBuilder;
        
		//Builder Options
		protected TBuilderOptions options;

        protected IPacketModuleBuilder packetModuleBuilder;

		//Service Collection
		protected IServiceCollection serviceCollection;
		private Action<IServiceCollection> serviceCollectionFactory;
		protected Func<IServiceProvider> serviceProviderFactory;

		public BuilderBase()
		{
			options = Activator.CreateInstance<TBuilderOptions>();
			serviceCollection = new ServiceCollection();
            this.packetModuleBuilder = new PacketModuleBuilder();
		}

		public abstract TResult Build();

		public TBuilder ConfigureLogging(Action<ILoggingBuilder> loggingBuilder)
		{
			this.loggingBuilder = loggingBuilder;
			return this as TBuilder;
		}

		public IServiceCollection GetServiceCollection()
		{
			return serviceCollection;
		}

		public IServiceProvider GetServiceProvider()
		{
			var serviceProvider = serviceProviderFactory != null
				? serviceProviderFactory.Invoke()
				: serviceCollection.BuildServiceProvider();
			try
			{
				PacketSerialiserProvider.PacketSerialiser = serviceProvider.GetService<IPacketSerialiser>();
			}
			catch (Exception ex)
			{
				throw new Exception("No packet serialiser has been configured for Networker");
			}

			if (PacketSerialiserProvider.PacketSerialiser == null)
				throw new Exception("No packet serialiser has been configured for Networker");

			return serviceProvider;
		}

		public TBuilder RegisterMiddleware<T>()
			where T : class, IMiddlewareHandler
		{
			serviceCollection.AddSingleton<IMiddlewareHandler, T>();
			return this as TBuilder;
		}
        
		public TBuilder RegisterModule(IPacketModule packetHandlerModule)
        {
            packetHandlerModule.Register(packetModuleBuilder);
            return this as TBuilder;
        }

		public TBuilder RegisterModule<T>() where T : class, IPacketModule
        {
            RegisterModule(Activator.CreateInstance<T>());

            return this as TBuilder;
        }

		public TBuilder RegisterTypes(Action<IServiceCollection> serviceCollection)
		{
			serviceCollectionFactory = serviceCollection;
			return this as TBuilder;
		}

		public TBuilder SetPacketBufferSize(int size)
		{
			options.PacketSizeBuffer = size;
			return this as TBuilder;
		}

		public TBuilder SetServiceCollection(IServiceCollection serviceCollection,
			Func<IServiceProvider> serviceProviderFactory = null)
		{
			this.serviceCollection = serviceCollection;
			this.serviceProviderFactory = serviceProviderFactory;
			return this as TBuilder;
		}

		public TBuilder UseConfiguration(IConfiguration configuration)
		{
			this.configuration = configuration;
			serviceCollection.AddSingleton(configuration);
			return this as TBuilder;
		}

		public TBuilder UseConfiguration<T>(IConfiguration configuration)
			where T : class
		{
			this.configuration = configuration;
			serviceCollection.AddSingleton(configuration);
			serviceCollection.Configure<T>(configuration);
			return this as TBuilder;
		}

		public TBuilder UseTcp(int port)
		{
			options.TcpPort = port;
			return this as TBuilder;
		}

		public TBuilder UseUdp(int port)
		{
			options.UdpPort = port;
			return this as TBuilder;
		}

		protected void SetupSharedDependencies()
		{
			serviceCollection.AddSingleton(options);

			if (loggingBuilder == null) loggingBuilder = loggerBuilderFactory => { };

			serviceCollection.AddLogging(loggingBuilder);
			serviceCollectionFactory?.Invoke(serviceCollection);
            serviceCollection.AddSingleton<IPacketModuleBuilder>(packetModuleBuilder);

            foreach (var packetRegistry in packetModuleBuilder.GetPacketRegistries())
            {
                serviceCollection.AddSingleton(packetRegistry.PacketHandlerType);
            }

		}
	}
}
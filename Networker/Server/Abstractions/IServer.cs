﻿using System;
using Networker.Common.Abstractions;

namespace Networker.Server.Abstractions
{
    public interface IServer
    {
        IServerInformation Information { get; }
        IServiceProvider ServiceProvider { get; }
        ITcpConnections GetConnections();

        EventHandler<ServerInformationEventArgs> ServerInformationUpdated { get; set; }
        EventHandler<TcpConnectionConnectedEventArgs> ClientConnected { get; set; }
        EventHandler<TcpConnectionDisconnectedEventArgs> ClientDisconnected { get; set; }

        void Broadcast<T>(T packet) where T : PacketBase;
        // void Broadcast(byte[] packet);

        void Start();
        void Stop();
    }
}

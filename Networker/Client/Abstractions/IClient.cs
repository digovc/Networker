using System;
using System.Net.Sockets;

namespace Networker.Client.Abstractions
{
    public interface IClient
    {
        EventHandler<Socket> Connected { get; set; }
        EventHandler<Socket> Disconnected { get; set; }

        void Send<T>(T packet) where T : class;
        // void Send(byte[] packet);

        void SendUdp<T>(T packet) where T : class;
        void SendUdp(byte[] packet);

        long Ping(int timeout = 10000);

        ConnectResult Connect();

        void Stop();
    }
}

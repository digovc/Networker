using System;
using Networker.Common.Abstractions;

namespace PacketPool
{
    public static class PacketPool
    {
        public static T Rent<T>() where T: PacketBase
        {
            return default(T);
        }

        public static void Release<T>(T packet) where T : PacketBase
        {

        }
    }
}

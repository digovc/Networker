using Networker.Common.Abstractions;
using ProtoBuf;

namespace Tutorial.Common
{
	[ProtoContract]
	public class ChatPacket : PacketBase
    {
		[ProtoMember(1)] 
		public virtual string Name { get; set; }

		[ProtoMember(2)] 
		public virtual string Message { get; set; }
	}
}

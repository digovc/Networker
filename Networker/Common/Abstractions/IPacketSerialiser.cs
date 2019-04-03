namespace Networker.Common.Abstractions
{
	public interface IPacketSerialiser
	{
		bool CanReadLength { get; }
		bool CanReadName { get; }
		bool CanReadOffset { get; }
		T Deserialise<T>(byte[] packetBytes) where T : class;
		T Deserialise<T>(byte[] packetBytes, int offset, int length);
		byte[] Package(string name, byte[] bytes);
		byte[] Serialise<T>(T packet) where T : class;
	}
}
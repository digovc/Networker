namespace Networker.Common.Abstractions
{
	public interface IPacketModule
	{
		void Register(IPacketModuleBuilder builder);
	}
}
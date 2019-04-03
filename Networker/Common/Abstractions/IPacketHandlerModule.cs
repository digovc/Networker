using System;
using System.Collections.Generic;

namespace Networker.Common.Abstractions
{
	[Obsolete("Use IPacketModule")]
	public interface IPacketHandlerModule
	{
		Dictionary<Type, Type> GetPacketHandlers();
	}
}
using System;

namespace Boxsie.Network.Core.Objects
{
    public interface INetworkObject
    {
        Guid Id { get; }
        bool IsLocal { get; }
    }
}
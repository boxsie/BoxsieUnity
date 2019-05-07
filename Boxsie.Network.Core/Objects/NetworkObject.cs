using System;

namespace Boxsie.Network.Core.Objects
{
    public abstract class NetworkObject : INetworkObject
    {
        public Guid Id { get; }
        public bool IsLocal { get; }

        protected NetworkObject()
        {
            Id = Guid.NewGuid();
            IsLocal = true;
        }

        protected NetworkObject(Guid id)
        {
            Id = id;
            IsLocal = false;
        }
    }
}
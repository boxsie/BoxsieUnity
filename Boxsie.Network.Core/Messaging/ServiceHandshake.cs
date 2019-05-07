using Boxsie.Network.Core.Connection;
using ProtoBuf;

namespace Boxsie.Network.Core.Messaging
{
    [ProtoContract]
    public class ServiceHandshake
    {
        [ProtoMember(1)]
        public float Version { get; set; }
        [ProtoMember(2)]
        public UserDto UserDto { get; set; }

        public ServiceHandshake() { }
    }
}
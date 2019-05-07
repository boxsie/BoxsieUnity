using ProtoBuf;

namespace Boxsie.Network.Core.Data
{
    [ProtoContract]
    public class PubSubDto
    {
        [ProtoMember(1)]
        public PublishType PublishType { get; set; }
        [ProtoMember(2)]
        public byte[] Data { get; set; }
    }
}
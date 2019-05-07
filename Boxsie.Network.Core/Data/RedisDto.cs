using ProtoBuf;

namespace Boxsie.Network.Core.Data
{
    [ProtoContract]
    public class RedisDto
    {
        [ProtoMember(1)]
        public byte[] Bytes { get; set; }
        [ProtoMember(2)]
        public string TopicKey { get; set; }
        [ProtoMember(3)]
        public string ObjectKey { get; set; }
        [ProtoMember(4)]
        public string Id { get; set; }
        
        public string Key => $"{TopicKey}:{ObjectKey}:{Id}";
    }
}
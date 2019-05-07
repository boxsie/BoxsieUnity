using ProtoBuf;

namespace Boxsie.Network.Core.Connection
{
    [ProtoContract]
    public class UserDto
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Username { get; set; }
        [ProtoMember(3)]
        public string Password { get; set; }
        [ProtoMember(4)]
        public string Salt { get; set; }

        public UserDto() { }
    }
}
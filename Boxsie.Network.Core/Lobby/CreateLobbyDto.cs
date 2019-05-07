using ProtoBuf;

namespace Boxsie.Network.Core.Lobby
{
    [ProtoContract]
    public class CreateLobbyDto
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public bool IsPublic { get; set; }
        [ProtoMember(3)]
        public int MaxUsers { get; set; }
        
        public CreateLobbyDto() { }
    }
}
using System;
using ProtoBuf;

namespace Boxsie.Network.Core.Lobby
{
    [ProtoContract]
    public class LobbyInfoDto
    {
        [ProtoMember(1)]
        public Guid GameLobbyId { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public bool IsPublic { get; set; }
        [ProtoMember(4)]
        public int MaxUsers { get; set; }
        [ProtoMember(5)]
        public int UserCount { get; set; }

        public LobbyInfoDto() { }
    }
}
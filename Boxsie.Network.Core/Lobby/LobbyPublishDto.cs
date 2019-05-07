using System;
using ProtoBuf;

namespace Boxsie.Network.Core.Lobby
{
    [ProtoContract]
    public class LobbyPublishDto
    {
        [ProtoMember(1)]
        public Guid GameLobbyId { get; set; }
        [ProtoMember(2)]
        public Guid FromSessionId { get; set; }
        [ProtoMember(3)]
        public LobbyPublishType PublishType { get; set; }
        [ProtoMember(4)]
        public byte[] PublishBytes { get; set; }

        public LobbyPublishDto() { }
    }
}
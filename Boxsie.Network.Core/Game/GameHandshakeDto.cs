using System;
using Boxsie.Network.Core.Messaging;
using ProtoBuf;

namespace Boxsie.Network.Core.Game
{
    [ProtoContract]
    public class GameHandshakeDto
    {
        [ProtoMember(1)]
        public Guid GameLobbyId { get; set; }
        [ProtoMember(2)]
        public string ConnectionToken { get; set; }

        public GameHandshakeDto() { }
    }
}
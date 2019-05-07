using System;
using System.Collections.Generic;
using System.Net;
using Boxsie.Network.Core.Messaging;
using ProtoBuf;

namespace Boxsie.Network.Core.Lobby
{
    [ProtoContract]
    public class JoinLobbyDto
    {
        [ProtoMember(1)]
        public Guid GameLobbyId { get; set; }
    }
}
using System;
using ProtoBuf;

namespace Boxsie.Network.Core.Game
{
    [ProtoContract]
    public class GameRegisterDto
    {
        [ProtoMember(1)]
        public Guid GameLobbyId { get; set; }
        [ProtoMember(2)]
        public string LocalEndpoint { get; set; }
        [ProtoMember(3)]
        public int RemotePort { get; set; }

        public bool HasValue()
        {
            return GameLobbyId != Guid.Empty && string.IsNullOrEmpty(LocalEndpoint);
        }
    }
}
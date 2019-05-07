using System;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Objects;
using ProtoBuf;

namespace Boxsie.Network.Core.Connection
{
    [ProtoContract]
    public class PeerEndpointModel : IRepositoryItem
    {
        public HubType Hub => HubType.Game;
        public string ObjectName => "PeerEndpoint";
        public Guid ItemId => SessionId;

        [ProtoMember(1)]
        public Guid SessionId { get; set; }
        [ProtoMember(2)]
        public Guid LobbyId { get; set; }
        [ProtoMember(3)]
        public string LocalEndpoint { get; set; }
        [ProtoMember(4)]
        public string RemoteEndpoint { get; set; }
        [ProtoMember(5)]
        public string ConnectionToken { get; set; }
    }
}
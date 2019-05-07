using System;
using System.Collections.Generic;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Objects;
using ProtoBuf;

namespace Boxsie.Network.Core.Lobby
{
    [ProtoContract]
    public class LobbyUserModel : IRepositoryItem
    {
        public HubType Hub => HubType.Lobby;
        public string ObjectName => "LobbyUser";

        public Guid ItemId => SessionId;

        [ProtoMember(1)]
        public Guid SessionId { get; set; }
        [ProtoMember(2)]
        public string Username { get; set; }
        [ProtoMember(3)]
        public Guid CurrentGameLobby { get; set; }
        [ProtoMember(4)]
        public List<Guid> PlayerIds { get; set; }
        
        public LobbyUserModel()
        {
            PlayerIds = new List<Guid>();
        }
    }
}
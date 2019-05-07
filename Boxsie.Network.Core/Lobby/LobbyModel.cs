using System;
using System.Collections.Generic;
using System.Linq;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Core.Objects;
using ProtoBuf;

namespace Boxsie.Network.Core.Lobby
{
    [ProtoContract]
    public class LobbyModel : IRepositoryItem
    {
        public HubType Hub => HubType.Lobby;
        public string ObjectName => "Lobby";
        public Guid ItemId => LobbyId;
        
        [ProtoMember(1)]
        public Guid LobbyId { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public bool IsPublic { get; set; }
        [ProtoMember(4)]
        public int MaxUsers { get; set; }
        [ProtoMember(5)]
        public List<Guid> UserSessionIds { get; set; }
        [ProtoMember(6)]
        public List<Guid> PlayerIds { get; set; }
        [ProtoMember(7)]
        public Guid HostId { get; set; }

        public LobbyModel()
        {
            UserSessionIds = new List<Guid>();
            PlayerIds = new List<Guid>();
        }

        public LobbyModel(CreateLobbyDto lobbyDto)
        {
            UserSessionIds = new List<Guid>();
            PlayerIds = new List<Guid>();

            LobbyId = Guid.NewGuid();
            Name = lobbyDto.Name;
            IsPublic = lobbyDto.IsPublic;
            MaxUsers = lobbyDto.MaxUsers;
        }
    }
}
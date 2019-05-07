using System;
using System.Collections.Generic;
using System.Linq;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Objects;
using ProtoBuf;

namespace Boxsie.Network.Core.Connection
{
    [ProtoContract]
    public class ConnectionModel : IRepositoryItem
    {
        public HubType Hub => HubType.Core;
        public string ObjectName => "Connection";

        public Guid ItemId => SessionId;

        [ProtoMember(1)]
        public Guid SessionId { get; set; }
        [ProtoMember(2)]
        public int DataId { get; set; }
        [ProtoMember(3)]
        public string Endpoint { get; set; }
        [ProtoMember(4)]
        public string Username { get; set; }
        [ProtoMember(5)]
        public bool IsGuest { get; set; }
        [ProtoMember(6)]
        public bool IsAuthenticated { get; set; }
        [ProtoMember(7)]
        public Dictionary<HubType, AuthLevels> HubAuths { get; set; }
        
        public ConnectionModel()
        {
            SessionId = Guid.NewGuid();
            IsAuthenticated = false;
        }
        
        public void Authenticate(UserDto userDto, bool isAuthenticated, List<UserAuthDto> hubAuths)
        {
            DataId = userDto.Id;
            Username = userDto.Username;
            IsGuest = userDto.Username == "Guest";
            IsAuthenticated = isAuthenticated;

            if (!isAuthenticated)
                return;

            HubAuths = new Dictionary<HubType, AuthLevels>();

            var coreAuth = hubAuths.FirstOrDefault(x => x.HubId == (int)HubType.Core);

            if (coreAuth == null)
            {
                IsAuthenticated = false;
                return;
            }

            var hubCount = Enum.GetNames(typeof(HubType)).Length;

            for (var i = 0; i < hubCount; i++)
            {
                var dbAuth = hubAuths.FirstOrDefault(x => x.HubId == i);

                var authLevel = dbAuth != null 
                    ? (AuthLevels) dbAuth.AuthLevel 
                    : (AuthLevels) coreAuth.AuthLevel;

                HubAuths.Add((HubType)i, authLevel);
            }
        }
    }
}

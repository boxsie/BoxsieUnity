using System;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Lobby;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Hub.Service.Attributes;
using Boxsie.Network.Hub.Service.Core;
using Boxsie.Network.Repositories.Interfaces;

namespace Boxsie.Server.Hubs.Lobby.Actions
{
    [HubType(HubType.Lobby)]
    [ActionType((int)LobbyActionType.GetLobby)]
    public class GetLobbyAction : HubAction
    {
        private readonly IRepository<LobbyModel> _lobbies;

        public GetLobbyAction(IRepositoryFactory repositories)
        {
            _lobbies = repositories.GetRepository<LobbyModel>();
        }
        
        public override void Request(Msg msg)
        {
            if (msg.Data != null)
            {
                var lobby = _lobbies.Get(msg.Data.ProtoDeserialise<Guid>());
                var header = msg.GetResponseHeader(MessageType.Response, lobby.ProtoSerialise());

                SocketService.SendMessageToClient(header, msg.SenderEndPoint);
            }
        }
    }
}
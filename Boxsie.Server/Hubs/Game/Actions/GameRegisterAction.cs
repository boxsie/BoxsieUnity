using System;
using System.Net;
using System.Threading.Tasks;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Game;
using Boxsie.Network.Core.Lobby;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Hub.Service.Attributes;
using Boxsie.Network.Hub.Service.Core;
using Boxsie.Network.Repositories.Interfaces;
using Boxsie.Network.Sockets.Helpers;
using Boxsie.Server.Hubs.Lobby;

namespace Boxsie.Server.Hubs.Game.Actions
{
    [HubType(HubType.Game)]
    [ActionType((int) GameActionType.GameRegister)]
    public class GameRegisterAction : HubAction
    {
        private readonly ILobbyFactory _lobbyFactory;
        private readonly IRepository<PeerEndpointModel> _peers;

        public GameRegisterAction(ILobbyFactory lobbyFactory, IRepositoryFactory repositoryFactory)
        {
            _lobbyFactory = lobbyFactory;
            _peers = repositoryFactory.GetRepository<PeerEndpointModel>();
        }

        public override void Request(Msg msg)
        {
            var user = msg.Connection;
            var gameRegisterDto = msg.Data.ProtoDeserialise<GameRegisterDto>();

            if (user == null || gameRegisterDto == null || gameRegisterDto.HasValue())
                return;
            
            var lobby = _lobbyFactory.GetGameLobby(SocketService, gameRegisterDto.GameLobbyId, false);

            if (lobby == null || !lobby.Model.UserSessionIds.Contains(user.SessionId))
                return;

            var peerEndpoint = new PeerEndpointModel
            {
                SessionId = user.SessionId,
                LobbyId = gameRegisterDto.GameLobbyId,
                LocalEndpoint = gameRegisterDto.LocalEndpoint,
                RemoteEndpoint = new IPEndPoint(msg.SenderEndPoint.Address, gameRegisterDto.RemotePort).ToString(),
                ConnectionToken = string.Concat(SocketHelper.Version, user.SessionId, DateTime.Now.Ticks.ToString()).ToBase64String()
            };

            _peers.Update(peerEndpoint);

            // The first to register becomes the host
            var host = lobby.Model.HostId == Guid.Empty 
                ? SetHost(peerEndpoint)
                : _peers.Get(lobby.Model.HostId);
            
            SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Response, host.ProtoSerialise()), msg.SenderEndPoint);
        }

        private PeerEndpointModel SetHost(PeerEndpointModel host)
        {
            _lobbyFactory.SetHost(host.LobbyId, host.SessionId);
            return host;
        }
    }
}
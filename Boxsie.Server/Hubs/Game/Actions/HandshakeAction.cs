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
using Boxsie.Network.Sockets.Service;
using Boxsie.Server.Hubs.Lobby;

namespace Boxsie.Server.Hubs.Game.Actions
{
    [HubType(HubType.Game)]
    [ActionType((int)GameActionType.Handshake)]
    public class HandshakeAction : HubAction
    {
        private readonly ILobbyFactory _lobbyFactory;
        private readonly IRepository<PeerEndpointModel> _peers;

        public HandshakeAction(ILobbyFactory lobbyFactory, IRepositoryFactory repositoryFactory)
        {
            _lobbyFactory = lobbyFactory;
            _peers = repositoryFactory.GetRepository<PeerEndpointModel>();
        }
        
        public override void Request(Msg msg)
        {
            if (msg.Data != null)
            {
                var user = msg.Connection;
                var gameHandshake = msg.Data.ProtoDeserialise<GameHandshakeDto>();

                if (user == null || gameHandshake == null || gameHandshake.GameLobbyId == Guid.Empty)
                    return;

                var lobby = _lobbyFactory.GetGameLobby(SocketService, gameHandshake.GameLobbyId, false);

                if (lobby?.Host == null || !lobby.Model.UserSessionIds.Contains(user.SessionId) || lobby.Host.ConnectionToken != gameHandshake.ConnectionToken)
                    return;

                var client = _peers.Get(user.SessionId);

                if (client == null)
                    return;

                if (lobby.Host.SessionId != user.SessionId)
                {
                    var hostInt = StringToEndpoint(lobby.Host.LocalEndpoint);
                    var hostExt = StringToEndpoint(lobby.Host.RemoteEndpoint);
                    var clientInt = StringToEndpoint(client.LocalEndpoint);
                    var clientExt = StringToEndpoint(client.RemoteEndpoint);

                    SocketService.Introduce(hostInt, hostExt, clientInt, clientExt, lobby.Host.ConnectionToken);

                    SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Success), msg.SenderEndPoint);
                }
            }
        }

        private static IPEndPoint StringToEndpoint(string endpoint)
        {
            var endpointSplit = endpoint.Split(':');

            IPAddress ip;
            if (!IPAddress.TryParse(endpointSplit[0], out ip))
                return null;

            int port;
            if (!int.TryParse(endpointSplit[1], out port))
                return null;

            return new IPEndPoint(ip, port);
        }
    }
}
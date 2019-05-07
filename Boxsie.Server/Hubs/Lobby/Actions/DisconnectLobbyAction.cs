using Boxsie.Core.Debug;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Lobby;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Hub.Service.Attributes;
using Boxsie.Network.Hub.Service.Core;
using Boxsie.Network.Repositories.Interfaces;

namespace Boxsie.Server.Hubs.Lobby.Actions
{
    [HubType(HubType.Lobby)]
    [ActionType((int)LobbyActionType.Disconnect, IsDisconnectAction = true)]
    public class DisconnectLobbyAction : HubAction
    {
        private readonly IObservableRepository<LobbyUserModel, SocketSubscriberModel> _lobbyUsers;

        public DisconnectLobbyAction(IRepositoryFactory repositoryFactory)
        {
            _lobbyUsers = repositoryFactory.GetSocketObservable<LobbyUserModel>((bytes, point) => SocketService.SendMessageToClient(bytes, point), HubType.Lobby, (int)LobbyActionType.Disconnect);
        }

        public override void Request(Msg msg)
        {
            var lobbyUser = _lobbyUsers.Get(msg.Connection.SessionId);

            Header header;

            if (lobbyUser == null)
            {
                Debug.Log($"Lobby service leave failed, '{msg.Connection.Username}' is not connected to the lobby service.", DebugLogType.Warning);
                header = msg.GetResponseHeader(MessageType.Failed);
            }
            else
            {
                Debug.Log($"'{msg.Connection.Username}' left the lobby service.");
                header =  msg.GetResponseHeader(MessageType.Success);

                _lobbyUsers.Delete(msg.Connection.SessionId);
            }

            SocketService.SendMessageToClient(header, msg.SenderEndPoint);
        }
    }
}
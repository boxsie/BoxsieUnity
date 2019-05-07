using Boxsie.Core.Debug;
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
    [ActionType((int) LobbyActionType.GetLobbyUser)]
    public class GetLobbyUserAction : HubAction
    {
        private readonly IObservableRepository<LobbyUserModel, SocketSubscriberModel> _lobbyUsers;

        public GetLobbyUserAction(IRepositoryFactory repositoryFactory)
        {
            _lobbyUsers = repositoryFactory.GetSocketObservable<LobbyUserModel>((bytes, point) => SocketService.SendMessageToClient(bytes, point), HubType.Lobby, (int) LobbyActionType.GetLobbyUser);
        }

        public override void Request(Msg msg)
        {
            Debug.Log($"{msg.Connection.Username}' is requesting their lobby user data.");

            var lobbyUser = _lobbyUsers.Get(msg.Connection.SessionId);
            
            if (lobbyUser == null)
            {
                Debug.Log($"'{msg.Connection.Username}' connected to the lobby service.");

                lobbyUser = new LobbyUserModel
                {
                    SessionId = msg.Connection.SessionId,
                    Username = msg.Connection.Username
                };
                
                _lobbyUsers.Update(lobbyUser);
            }

            var userData = lobbyUser.ProtoSerialise();

            SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Success, userData), msg.SenderEndPoint, true);
        }
    }
}
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
    [ActionType((int)LobbyActionType.GetLobbyies)]
    public class GetLobbiesAction : HubAction
    {
        private readonly IObservableRepository<LobbyModel, SocketSubscriberModel> _gameLobbies;

        public GetLobbiesAction(IRepositoryFactory repositoryFactory)
        {
            _gameLobbies = repositoryFactory.GetSocketObservable<LobbyModel>((bytes, point) =>  SocketService.SendMessageToClient(bytes, point), HubType.Lobby, (int)LobbyActionType.GetLobbyies);
        }

        public override void Request(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is requesting all the game lobbies.");

            var lobbies = _gameLobbies.GetAll();

            SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Response, lobbies.ProtoSerialise()), msg.SenderEndPoint);
        }
    }
}
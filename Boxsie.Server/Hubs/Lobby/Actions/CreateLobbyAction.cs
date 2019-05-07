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
    [AuthLevel(AuthLevels.User)]
    [ActionType((int) LobbyActionType.CreateLobby)]
    public class CreateLobbyAction : HubAction
    {
        private readonly IObservableRepository<LobbyModel, SocketSubscriberModel> _gameLobbyModels;
        private readonly ILobbyFactory _lobbyFactory;

        public CreateLobbyAction(IRepositoryFactory repositoryFactory, ILobbyFactory lobbyFactory)
        {
            _gameLobbyModels = repositoryFactory.GetSocketObservable<LobbyModel>((bytes, point) =>  SocketService.SendMessageToClient(bytes, point), HubType.Lobby, (int)LobbyActionType.LobbyPubSub);
            _lobbyFactory = lobbyFactory;
        }

        public override void Request(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is trying to create a game lobby.");

            var createDto = msg.Data.ProtoDeserialise<CreateLobbyDto>();

            if (createDto == null)
            {
                Debug.Log($"Creation of game lobby by '{msg.Connection.Username}' failed, header data is missing.", DebugLogType.Warning);
                SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Failed), msg.SenderEndPoint);
            }
            else
            {
                var newLobby = _lobbyFactory.CreateGameLobbyModel(createDto);

                _gameLobbyModels.Update(newLobby.ItemId, newLobby.ProtoSerialise());

                Debug.Log($"'{msg.Connection.Username}' created lobby '{newLobby.Name}'.");

                SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Success, newLobby.LobbyId.ProtoSerialise()), msg.SenderEndPoint);
            }
        }
    }
}
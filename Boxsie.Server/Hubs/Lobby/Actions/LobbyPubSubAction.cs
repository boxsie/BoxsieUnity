using System;
using Boxsie.Core.Debug;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Core.Data;
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
    [ActionType((int)LobbyActionType.LobbyPubSub)]
    public class LobbyPubSubAction : HubAction
    {
        private readonly ILobbyFactory _lobbyFactory;
        private readonly IRepository<LobbyUserModel> _lobbyUsers;
        private readonly IObservableRepository<LobbyModel, SocketSubscriberModel> _gameLobbyModels;

        public LobbyPubSubAction(IRepositoryFactory repositoryFactory, ILobbyFactory lobbyFactory)
        {
            _lobbyFactory = lobbyFactory;
            _lobbyUsers = repositoryFactory.GetPublisher<LobbyUserModel>();
            _gameLobbyModels = repositoryFactory.GetSocketObservable<LobbyModel>((bytes, point) => SocketService.SendMessageToClient(bytes, point), HubType.Lobby, (int)LobbyActionType.LobbyPubSub);
        }

        public override void Subscribe(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is trying to join a game lobby.");

            var joinResult = AutheriseJoin(msg.Data.ProtoDeserialise<JoinLobbyDto>(), msg.Connection);

            var gameLobby = JoinLobby(joinResult.JoinDto.GameLobbyId, joinResult, msg);

            if (gameLobby != null)
            {
                SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Success, gameLobby.Model.ProtoSerialise()), msg.SenderEndPoint);

                //await gameLobby.BroadcastToLobby(Guid.Empty, LobbyPublishType.RoomUpdate, (await _lobbyFactory.CreateGameLobby(gameLobby)).ProtoSerialise());
            }
        }

        public override void Publish(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is trying to publish to game lobby");

            var publishDto = msg.Data.ProtoDeserialise<PubSubDto>();
            
            if (publishDto == null || publishDto.PublishType != PublishType.Broadcast)
                return;

            var lobbyPublishDto = publishDto.Data.ProtoDeserialise<LobbyPublishDto>();
            
            if (lobbyPublishDto == null)
                return;

            var gameLobby = _lobbyFactory.GetGameLobby(SocketService, lobbyPublishDto.GameLobbyId, false);

            if (gameLobby == null)
                return;

            gameLobby.BroadcastToLobby(lobbyPublishDto);
        }

        public override void Unsubscribe(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is leaving the game lobby.");
            
            var lobbyUser = _lobbyUsers.Get(msg.Connection.SessionId);

            if (lobbyUser == null || lobbyUser.CurrentGameLobby == Guid.Empty)
            {
                Debug.Log($"Cannot unsubscribe '{msg.Connection.Username}' from the game lobby as they have not joined.");
                return;
            }

            var gameLobby = _lobbyFactory.GetGameLobby(SocketService, lobbyUser.CurrentGameLobby, false);

            if (gameLobby == null)
            {
                Debug.Log($"Cannot unsubscribe '{msg.Connection.Username}' from the game lobby as the lobby cannot be found.");
                return;
            }

            gameLobby.RemoveUserFromLobby(lobbyUser);

            //await gameLobby.BroadcastToLobby(Guid.Empty, LobbyPublishType.RoomUpdate, (await _lobbyFactory.CreateGameLobby(gameLobby)).ProtoSerialise());

            _gameLobbyModels.Update(gameLobby.Model);
            _lobbyUsers.Update(lobbyUser);

            Debug.Log($"'{msg.Connection.Username}' has left the game lobby.");
        }

        private void RespondFail(string failMessage, Msg msg)
        {
            Debug.Log(failMessage, DebugLogType.Warning);
            SocketService.SendMessageToClient(msg.GetResponseHeader(MessageType.Failed, failMessage.ProtoSerialise()), msg.SenderEndPoint);
        }

        private JoinGameLobbyResult AutheriseJoin(JoinLobbyDto joinDto, ConnectionModel user)
        {
            string failMessage;

            if (joinDto == null)
            {
                failMessage = $"Game lobby join by '{user.Username}' failed, header data is missing.";
                return new JoinGameLobbyResult { FailMessage = failMessage }; 
            }

            var userModel = _lobbyUsers.Get(user.SessionId);

            if (userModel == null)
            { 
                failMessage = $"Game lobby join by '{user.Username}' failed, user has not joined the lobby service.";
                return new JoinGameLobbyResult { FailMessage = failMessage };
            }

            failMessage = PerformRoomValidation(userModel);

            return string.IsNullOrEmpty(failMessage)
                ? new JoinGameLobbyResult
                {
                    JoinDto = joinDto,
                    UserModel = userModel,
                    FailMessage = null
                } 
                : new JoinGameLobbyResult {FailMessage = failMessage};
        }

        private static string PerformRoomValidation(LobbyUserModel user)
        {
            if (user == null)
                return "ConnectionModel is not connected to the lobby service.";

            var failStart = $"Chat room join by '{user.Username}' failed,";

            //if (lobby == null)
            //    return $"{failStart} room not found.";

            //if (lobby.Users.Count == lobby.Model.MaxUsers)
            //    return $"{failStart} room is full.";

            return null;
        }

        private GameLobby JoinLobby(Guid lobbyId, JoinGameLobbyResult joinResult, Msg msg)
        {
            if (!string.IsNullOrEmpty(joinResult.FailMessage))
            {
                RespondFail(joinResult.FailMessage, msg);
                return null;
            }

            var gameLobby = _lobbyFactory.GetGameLobby(SocketService, lobbyId, true);

            gameLobby.AddUserToLobby(new SocketSubscriberModel(msg.TransactionId, msg.SessionId, msg.SenderEndPoint), joinResult);
                    
            _gameLobbyModels.Update(gameLobby.Model);
            _lobbyUsers.Update(joinResult.UserModel);

            Debug.Log($"'{msg.Connection.Username}' joined the '{gameLobby.Model.Name}' lobby.");

            return gameLobby;
        }
    }
}
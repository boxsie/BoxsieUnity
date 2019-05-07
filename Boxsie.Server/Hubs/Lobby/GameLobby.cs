using System;
using System.Collections.Generic;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Core.Data;
using Boxsie.Network.Core.Lobby;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Repositories.Socket;

namespace Boxsie.Server.Hubs.Lobby
{
    public class GameLobby
    {
        public Guid GameLobbyId => Model.LobbyId;

        public LobbyModel Model { get; set; }
        public List<LobbyUserModel> Users { get; set; }
        public SocketSubscribe LobbyPubSub { get; set; }
        public PeerEndpointModel Host { get; set; }
        
        public void AddUserToLobby(SocketSubscriberModel subscriber, JoinGameLobbyResult lobbyResult)
        {
            var lobbyPlayerId = Guid.NewGuid();

            lobbyResult.UserModel.PlayerIds.Add(lobbyPlayerId);
            Model.PlayerIds.Add(lobbyPlayerId);

            // Is this a player 2 of an already registered player?
            if (lobbyResult.UserModel.PlayerIds.Count != 1)
                return;

            LobbyPubSub.Subscribe(subscriber);
            Model.UserSessionIds.Add(lobbyResult.UserModel.SessionId);
            lobbyResult.UserModel.CurrentGameLobby = Model.LobbyId;
            Users.Add(lobbyResult.UserModel);
        }

        public void RemoveUserFromLobby(LobbyUserModel lobbyUser)
        {
            if (!Model.UserSessionIds.Contains(lobbyUser.SessionId))
                return;
            
            foreach (var playerId in lobbyUser.PlayerIds)
                Model.PlayerIds.Remove(playerId);

            Model.UserSessionIds.Remove(lobbyUser.SessionId);

            if (lobbyUser.SessionId == Model.HostId)
                Model.HostId = Guid.Empty;

            LobbyPubSub.Unsubscribe(lobbyUser.SessionId.ToString());

            lobbyUser.CurrentGameLobby = Guid.Empty;
            lobbyUser.PlayerIds.Clear();

            if (Users.Contains(lobbyUser))
                Users.Remove(lobbyUser);
        }

        public void BroadcastToLobby(LobbyPublishDto publishDto)
        {
            LobbyPubSub.Publish(PublishType.Broadcast, publishDto.ProtoSerialise());
        }

        public void BroadcastToLobby(Guid fromSessionId, LobbyPublishType publishType, byte[] publishBytes)
        {
            BroadcastToLobby(new LobbyPublishDto
            {
                FromSessionId = fromSessionId,
                GameLobbyId = Model.LobbyId,
                PublishType = publishType,
                PublishBytes = publishBytes
            });
        }

        private void MigrateHost()
        {
            // Doesnt migrate at the moment, just clears. Need a handle for it clientside.
            Host = null;
        }
    }
}
using System;
using System.Collections.Generic;
using Boxsie.Core.Debug;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Lobby;
using Boxsie.Network.Repositories.Interfaces;
using Boxsie.Network.Sockets.Core;

namespace Boxsie.Server.Hubs.Lobby
{
    public class LobbyFactory : ILobbyFactory
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepository<LobbyUserModel> _lobbyUsersRepo;
        private readonly IRepository<LobbyModel> _gameLobbyRepo;
        private readonly IRepository<PeerEndpointModel> _hosts;

        public LobbyFactory(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _lobbyUsersRepo = _repositoryFactory.GetRepository<LobbyUserModel>();
            _gameLobbyRepo = _repositoryFactory.GetRepository<LobbyModel>();
            _hosts = repositoryFactory.GetRepository<PeerEndpointModel>();
        }

        public LobbyModel CreateGameLobbyModel(CreateLobbyDto createDto)
        {
            var newSession = new LobbyModel(createDto);
            return newSession;
        }

        public GameLobby GetGameLobby(ISocketService socketService, Guid lobbyId, bool includeUsers = false)
        {
            if (lobbyId == Guid.Empty)
            {
                Debug.Log("Trying to get a lobby with an empty lobby Id.", DebugLogType.Warning);
                return null;
            }

            var lobby = new GameLobby
            {
                Model = _gameLobbyRepo.Get(lobbyId),
                Users = new List<LobbyUserModel>()
            };

            if (lobby.Model == null)
                return null;

            lobby.Host = _hosts.Get(lobby.Model.HostId);

            if (includeUsers && lobby.Model.UserSessionIds != null)
            {
                foreach (var serviceId in lobby.Model.UserSessionIds)
                    lobby.Users.Add(_lobbyUsersRepo.Get(serviceId));
            }

            lobby.LobbyPubSub = _repositoryFactory.GetSocketPubSub((bytes, point) => socketService.SendMessageToClient(bytes, point), "Lobby", lobbyId.ToString(), HubType.Lobby, (int)LobbyActionType.LobbyPubSub);

            return lobby;
        }

        public void SetHost(Guid lobbyId, Guid hostId)
        {
            var lobby = _gameLobbyRepo.Get(lobbyId);

            lobby.HostId = hostId;

            _gameLobbyRepo.Update(lobby);
        }
    }
}

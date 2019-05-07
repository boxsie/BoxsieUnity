using System;
using Boxsie.Network.Core.Lobby;
using Boxsie.Network.Sockets.Core;

namespace Boxsie.Server.Hubs.Lobby
{
    public interface ILobbyFactory
    {
        LobbyModel CreateGameLobbyModel(CreateLobbyDto createDto);
        GameLobby GetGameLobby(ISocketService socketService, Guid lobbyId, bool includeUsers);
        void SetHost(Guid lobbyId, Guid hostId);
    }
}
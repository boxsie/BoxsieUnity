using System;
using Boxsie.Network.Core.Lobby;

namespace Boxsie.Server.Hubs.Lobby
{
    public class JoinGameLobbyResult
    {
        public JoinLobbyDto JoinDto { get; set; }
        public LobbyUserModel UserModel { get; set; }
        public string FailMessage { get; set; }
    }
}
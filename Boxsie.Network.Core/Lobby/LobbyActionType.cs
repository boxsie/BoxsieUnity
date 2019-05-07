namespace Boxsie.Network.Core.Lobby
{
    public enum LobbyActionType
    {
        GetLobbyUser,
        Disconnect,
        LobbyPubSub,
        CreateLobby,
        GetLobbyies,
        GetLobby,
    }

    public enum GameActionType
    {
        GameRegister,
        Handshake,
        Disconnect,
    }
}
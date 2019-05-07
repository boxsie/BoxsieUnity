namespace Boxsie.Network.Sockets.Service
{
    public interface ISocket
    {
        event OnConnectHandle OnConnect;
        event OnIncomingMsgHandle OnIncomingMsg;
        event OnDisconnectHandle OnDisconnect;

        void MessageLoop();
    }
}
namespace Boxsie.Network.Sockets.Core
{
    public interface ISocketClient
    {
        bool Connected { get; }

        void Start(int localPort, string encryptId);
        void Connect(string ip, int port, byte[] handshakeBytes);
        void Disconnect();
        void SendMessageToHost(byte[] sendBytes, bool isReliable);
    }
}
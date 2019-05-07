using Boxsie.Network.Core.Messaging;

namespace Boxsie.Network.Sockets.Core
{
    public interface IClient
    {
        void Start(int localPort, string encryptId);
        void Connect(string ip, int port, byte[] handshakeBytes);
        void SendMessageToHost(IHeader header, bool isReliable = true);
        void SendMessageToHost(byte[] bytes, bool isReliable = true);
        void Disconnect();
    }
}
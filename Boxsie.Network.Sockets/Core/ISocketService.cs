using System.Net;
using Boxsie.Network.Core.Messaging;

namespace Boxsie.Network.Sockets.Core
{
    public interface ISocketService
    {
        void Start(int localPort, string encryptId);
        void SendMessageToClient(IHeader header, IPEndPoint receiver, bool isReliable = true);
        void SendMessageToClient(byte[] bytes, IPEndPoint receiver, bool isReliable = true);
        void SendMessageToAll(IHeader header, bool isReliable = true);
        void SendMessageToAll(byte[] bytes, bool isReliable = true);
        void Introduce(IPEndPoint hostInt, IPEndPoint hostExt, IPEndPoint clientInt, IPEndPoint clientExt, string connectionToken);
    }
}
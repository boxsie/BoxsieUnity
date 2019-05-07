using System.Net;

namespace Boxsie.Network.Sockets.Core
{
    public interface ISocketServer
    {
        void Start(int port, string encryptId);
        void SendMessageToClient(byte[] sendBytes, IPEndPoint receiverIP, bool isReliable);
        void SendMessageToAll(byte[] sendBytes, bool isReliable);
        void Approve(IPEndPoint endPoint, byte[] response);
        void Kick(IPEndPoint endPoint, string message);
        void SetTag<T>(IPEndPoint endPoint, T obj);
        T GetTag<T>(IPEndPoint endPoint);
        void Introduce(IPEndPoint hostInt, IPEndPoint hostExt, IPEndPoint clientInt, IPEndPoint clientExt, string connectionToken);
    }
}
using System.Net;
using Lidgren.Network;

namespace Boxsie.Network.Sockets.Core
{
    public interface ISocketCore
    {
        void Start(int localPort, string encryptId, NetPeer netPeer, bool isHost);
        void SendMessage(byte[] sendBytes, IPEndPoint receiverIP, bool isReliable);
        void Approve(IPEndPoint endPoint, byte[] response);
    }
}
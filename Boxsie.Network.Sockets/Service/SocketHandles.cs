using System.Net;

namespace Boxsie.Network.Sockets.Service
{
    public delegate void OnConnectHandle(IPEndPoint endPoint, byte[] connectRequest, int channel);
    public delegate void OnIncomingMsgHandle(IPEndPoint endPoint, byte[] incMsg, int channel);
    public delegate void OnDisconnectHandle(IPEndPoint endPoint);
}
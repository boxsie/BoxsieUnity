using System.Net;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Sockets.Core;

namespace Boxsie.Network.Sockets.Service
{
    public abstract class SocketService : ISocketService
    {
        protected ISocketServer SocketServer { get; }

        private readonly IMessageLoop _messageLoop;

        protected SocketService(ISocketServer socketServer, IMessageLoop messageLoop)
        {
            SocketServer = socketServer;
            _messageLoop = messageLoop;

            var socket = (ISocket)SocketServer;

            socket.OnConnect += SocketOnConnect;
            socket.OnIncomingMsg += SocketOnIncomingMsg;
            socket.OnDisconnect += SocketOnDisconnect;
        }

        protected abstract void SocketOnConnect(IPEndPoint endpoint, byte[] connectRequest, int channel);
        protected abstract void SocketOnIncomingMsg(IPEndPoint endpoint, byte[] msgBytes, int channel);
        protected abstract void SocketOnDisconnect(IPEndPoint endpoint);

        public virtual void Start(int localPort, string encryptId)
        {
            SocketServer.Start(localPort, encryptId);
            _messageLoop.Start((ISocket)SocketServer);
        }

        public virtual void SendMessageToClient(IHeader header, IPEndPoint receiver, bool isReliable)
        {
            SocketServer.SendMessageToClient(header.ProtoSerialise(), receiver, isReliable);
        }

        public virtual void SendMessageToAll(IHeader header, bool isReliable)
        {
            SocketServer.SendMessageToAll(header.ProtoSerialise(), isReliable);
        }

        public void SendMessageToClient(byte[] bytes, IPEndPoint receiver, bool isReliable = true)
        {
            SocketServer.SendMessageToClient(bytes, receiver, isReliable);
        }

        public void SendMessageToAll(byte[] bytes, bool isReliable = true)
        {
            SocketServer.SendMessageToAll(bytes, isReliable);
        }

        public void Introduce(IPEndPoint hostInt, IPEndPoint hostExt, IPEndPoint clientInt, IPEndPoint clientExt, string connectionToken)
        {
            SocketServer.Introduce(hostInt, hostExt, clientInt, clientExt, connectionToken);
        }
    }
}
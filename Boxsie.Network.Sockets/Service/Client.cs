using System.Net;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Sockets.Core;

namespace Boxsie.Network.Sockets.Service
{
    public abstract class Client : IClient
    {
        private readonly ISocketClient _socketClient;
        private readonly IMessageLoop _messageLoop;

        protected Client(ISocketClient socketClient, IMessageLoop messageLoop)
        {
            _socketClient = socketClient;
            _messageLoop = messageLoop;
            
            var socket = (ISocket)_socketClient;

            socket.OnConnect += SocketClientOnConnect;
            socket.OnIncomingMsg += SocketClientOnIncomingMsg;
            socket.OnDisconnect += SocketClientOnDisconnect;
        }

        public virtual void Start(int localPort, string encryptId)
        {
            _socketClient.Start(localPort, encryptId);

            _messageLoop.Start((ISocket)_socketClient);
        }

        public virtual void Connect(string ip, int port, byte[] handshakeBytes)
        {
            _socketClient.Connect(ip, port, handshakeBytes);
        }

        public virtual void SendMessageToHost(IHeader header, bool isReliable)
        {
            _socketClient.SendMessageToHost(header.ProtoSerialise(), isReliable);
        }

        public virtual void SendMessageToHost(byte[] bytes, bool isReliable = true)
        {
            _socketClient.SendMessageToHost(bytes, isReliable);
        }

        public virtual void Disconnect()
        {
            _socketClient.Disconnect();
        }

        protected abstract void SocketClientOnConnect(IPEndPoint endpoint, byte[] connectResponse, int channel);
        protected abstract void SocketClientOnIncomingMsg(IPEndPoint endpoint, byte[] msgBytes, int channel);
        protected abstract void SocketClientOnDisconnect(IPEndPoint endpoint);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Boxsie.Core.Debug;
using Boxsie.Network.Sockets.Core;
using Boxsie.Network.Sockets.Helpers;
using Lidgren.Network;

namespace Boxsie.Network.Sockets.Service
{
    public class SocketServer : Socket, ISocketServer
    {
        public IEnumerable<IPEndPoint> Connections { get { return _netServer.Connections.Select(x => x.RemoteEndPoint).ToList(); } }
        
        private NetServer _netServer;

        public void Start(int port, string encryptId)
        {
            var socketConfig = new NetPeerConfiguration(SocketHelper.AppName)
            {
                Port = port
            };

            socketConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            socketConfig.EnableMessageType(NetIncomingMessageType.UnconnectedData);

            _netServer = new NetServer(socketConfig);

            Debug.Log("Starting server and opening socket.");

            base.Start(port, encryptId, _netServer, true);
        }

        public void Kick(IPEndPoint endPoint, string message)
        {
            var connection = _netServer.GetConnection(endPoint);
            connection.Disconnect(message);
        }

        public void SendMessageToClient(byte[] sendBytes, IPEndPoint receiverIP, bool isReliable)
        {
            SendMessage(sendBytes, receiverIP, isReliable);
        }

        public void SendMessageToAll(byte[] sendBytes, bool isReliable)
        {
            foreach (var ip in Connections)
                SendMessage(sendBytes, ip, isReliable);
        }

        public void SetTag<T>(IPEndPoint endPoint, T obj)
        {
            var connection = _netServer.GetConnection(endPoint);

            if (connection != null)
                connection.Tag = obj;
        }

        public T GetTag<T>(IPEndPoint endPoint)
        {
            var connection = _netServer.GetConnection(endPoint);
            
            return connection == null
                ? default(T) 
                : (T) connection.Tag;
        }

        public void Introduce(IPEndPoint hostInt, IPEndPoint hostExt, IPEndPoint clientInt, IPEndPoint clientExt, string connectionToken)
        {
            Debug.Log($"Attempting to NAT introduce client '{clientExt}' to host '{hostExt}...'");

            _netServer.Introduce(hostInt, hostExt, clientInt, clientExt, connectionToken);
        }
    }
}
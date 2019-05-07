using Boxsie.Core.Debug;
using Boxsie.Network.Sockets.Core;
using Boxsie.Network.Sockets.Helpers;
using Lidgren.Network;

namespace Boxsie.Network.Sockets.Service
{
    public class SocketClient : Socket, ISocketClient
    {
        public bool Connected => _netClient?.ConnectionStatus == NetConnectionStatus.Connected;

        private NetClient _netClient;

        public void Start(int localPort, string encryptId)
        {
            var socketConfig = new NetPeerConfiguration(SocketHelper.AppName)
            {
                Port = localPort
            };

            socketConfig.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            
            _netClient = new NetClient(socketConfig);

            Debug.Log("Starting client and opening socket.");

            base.Start(localPort, encryptId, _netClient, false);
        }
        
        public void Connect(string ip, int port, byte[] handshakeBytes)
        {
            Debug.Log(string.Concat("Attempting to connect to '", ip, "' on port '", port, "'"));

            _netClient.Connect(ip, port, BytesToNOM(handshakeBytes));
        }

        public void Disconnect()
        {
            _netClient.Disconnect("I'm out...");
        }

        public void SendMessageToHost(byte[] sendBytes, bool isReliable)
        {
            SendMessage(sendBytes, _netClient.ServerConnection.RemoteEndPoint, isReliable);
        }
    }
}

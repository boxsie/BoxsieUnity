using System.Collections.Generic;
using System.Net;
using Boxsie.Core.Debug;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Sockets.Helpers;
using Lidgren.Network;

namespace Boxsie.Network.Sockets.Core
{
    public enum NetworkStatus
    {
        Offline,
        Online,
        Connecting,
        Connected,
        Disconnected
    }

    public abstract class SocketCore : ISocketCore
    {
        protected bool IsHost { get; set; }

        private NetPeer _netPeer;
        private string _encryptId;
        private Dictionary<IPEndPoint, NetConnection> _unapprovedConnections;
        
        public virtual void Start(int localPort, string encryptId, NetPeer netPeer, bool isHost)
        {
            _encryptId = encryptId;
            _netPeer = netPeer;
            IsHost = isHost;

            if (IsHost)
                _unapprovedConnections = new Dictionary<IPEndPoint, NetConnection>();

            Debug.Log(string.Concat("Starting socket ", isHost ? "host" : "client", " on port '", localPort, "'."));

            _netPeer.Start();
        }

        public void SendMessage(byte[] sendBytes, IPEndPoint receiverIP, bool isReliable)
        {
            var nom = BytesToNOM(sendBytes);
            var connection = _netPeer.GetConnection(receiverIP);

            if (connection == null)
                return;

            _netPeer.SendMessage(nom, connection, isReliable ? NetDeliveryMethod.ReliableOrdered : NetDeliveryMethod.Unreliable);

            Debug.LogShortDataMessage(false, receiverIP, sendBytes.Length);
        }

        public void Approve(IPEndPoint endPoint, byte[] response)
        {
            if (_unapprovedConnections.ContainsKey(endPoint))
            {
                _unapprovedConnections[endPoint].Approve(BytesToNOM(response));
                _unapprovedConnections.Remove(endPoint);
            }
            else
                Debug.Log($"Unable to approve connection for endpoint '{endPoint}', connection cannot be found.", DebugLogType.Warning);
        }

        protected MessageRoute GetMessageRoute(out NetIncomingMessage nim)
        {
            nim = _netPeer.ReadMessage();

            if (nim == null)
                return MessageRoute.Empty;

            var netStatus = nim.MessageType == NetIncomingMessageType.StatusChanged
                ? (NetConnectionStatus)nim.ReadByte()
                : NetConnectionStatus.None;

            SocketHelper.LogSocketMessage(nim, netStatus);
            
            if (IsHost && nim.MessageType == NetIncomingMessageType.ConnectionApproval)
                _unapprovedConnections.Add(nim.SenderEndPoint, nim.SenderConnection);

            switch (nim.MessageType)
            {
                case NetIncomingMessageType.ConnectionApproval:
                    return MessageRoute.ConnectionApproval;
                case NetIncomingMessageType.Data:
                    return MessageRoute.Data;
                case NetIncomingMessageType.UnconnectedData:
                    return MessageRoute.UnconnectedData;
                case NetIncomingMessageType.NatIntroductionSuccess:
                    var token = nim.ReadString();
                    Debug.Log("Nat introduction success to " + nim.SenderEndPoint + " token is: " + token);
                    break;
            }

            switch (netStatus)
            {
                case NetConnectionStatus.Connected:
                    return MessageRoute.Connected;
                case NetConnectionStatus.Disconnected:
                    return MessageRoute.Disconnected;
                default:
                    return MessageRoute.Unhandled;
            }
        }

        protected NetOutgoingMessage BytesToNOM(byte[] headerBytes)
        {
            var nom = _netPeer.CreateMessage();
            nom.Write(headerBytes);

            if (_encryptId == null)
                return nom;

            var algo = new NetXtea(_netPeer, _encryptId);
            nom.Encrypt(algo);

            return nom;
        }

        protected byte[] NIMToBytes(NetIncomingMessage nim)
        {
            if (_encryptId == null)
                return nim.ReadBytes(nim.LengthBytes);

            var algo = new NetXtea(_netPeer, _encryptId);

            return nim.Decrypt(algo)
                ? nim.ReadBytes(nim.LengthBytes)
                : null;
        }
    }
}
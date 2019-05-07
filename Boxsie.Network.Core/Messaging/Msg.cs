using System;
using System.Net;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Core.Enums;

namespace Boxsie.Network.Core.Messaging
{
    public interface IMsg
    {
        byte[] Data { get; set; }
    }
    
    public interface ISMsg
    {
        void Read(byte[] data);
        byte[] Write();
    }

    public class Msg : IMsg
    {
        public byte[] Data { get; set; }

        public Guid SessionId { get; set; }
        public Guid TransactionId { get; set; }
        public ConnectionModel Connection { get; set; }
        public IPEndPoint SenderEndPoint { get; set; }
        public HubType? Hub { get; set; }
        public int? Action { get; set; }
        public MessageType MessageType { get; set; }

        public void SetDestination(HubType hubType, int action)
        {
            Hub = hubType;
            Action = action;
        }

        public void SetUser(ConnectionModel connection)
        {
            Connection = connection;
            SessionId = connection?.SessionId ?? Guid.Empty;
        }

        public void SetData(byte[] data)
        {
            Data = data;
        }

        public Header GetResponseHeader(MessageType messageType, byte[] data = null)
        {
            if (Hub != null && Action != null)
                return new Header(Connection.SessionId, TransactionId, Hub.Value, Action.Value, messageType, data);

            return null;
        }
    }
}

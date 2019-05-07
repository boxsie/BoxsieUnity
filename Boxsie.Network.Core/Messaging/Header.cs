using System;
using Boxsie.Network.Core.Enums;
using ProtoBuf;

namespace Boxsie.Network.Core.Messaging
{
    public interface IHeader
    {
        byte[] Data { get; }
    }

    [ProtoContract]
    public class StreamHeader : IHeader
    {
        [ProtoMember(1)]
        public byte[] Data { get; set; }

        public StreamHeader() { }
    }

    [ProtoContract]
    public class Header : IHeader
    {
        [ProtoMember(1)]
        public Guid SenderSessionId { get; set; }
        [ProtoMember(2)]
        public Guid TransactionId { get; set; }
        [ProtoMember(3)]
        public HubType Hub { get; set; }
        [ProtoMember(4)]
        public int Action { get; set; }
        [ProtoMember(5)]
        public MessageType MessageType { get; set; }
        [ProtoMember(6)]
        public byte[] Data { get; set; }
        
        public Header() { }

        public Header(Guid senderSessionId, HubType hub, int action, MessageType messageType, byte[] data)
        {
            SenderSessionId = senderSessionId;
            TransactionId = Guid.NewGuid();
            Hub = hub;
            Action = action;
            MessageType = messageType;
            Data = data;
        }

        public Header(Guid senderSessionId, Guid transactionId, HubType hub, int action, MessageType messageType, byte[] data)
        {
            SenderSessionId = senderSessionId;
            TransactionId = transactionId;
            Hub = hub;
            Action = action;
            MessageType = messageType;
            Data = data;
        }
    }
}
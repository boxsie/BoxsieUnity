using System;
using System.Net;

namespace Boxsie.Network.Core.Messaging
{
    public class SocketSubscriberModel : ISubscriberModel
    {
        public string Id { get; }
        public Guid SubscriptionId { get; }
        public Guid UserId { get; }
        public IPEndPoint Endpoint { get; }

        public SocketSubscriberModel(Guid transactionId, Guid userId, IPEndPoint endpoint)
        {
            Id = userId.ToString();
            SubscriptionId = transactionId;
            UserId = userId;
            Endpoint = endpoint;
        }
    }

    public interface ISubscriberModel
    {
        string Id { get; }
    }
}
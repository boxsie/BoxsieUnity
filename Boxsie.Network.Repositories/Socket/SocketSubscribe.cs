using System;
using System.Net;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Repositories.Redis;

namespace Boxsie.Network.Repositories.Socket
{
    public class SocketSubscribe : RedisSubscribe<SocketSubscriberModel>
    {
        private readonly Action<byte[], IPEndPoint> _sendMsg;
        private readonly HubType _hub;
        private readonly int _actionId;

        public SocketSubscribe(Action<byte[], IPEndPoint> sendMsg, HubType hub, int actionId)
        {
            _sendMsg = sendMsg;
            _hub = hub;
            _actionId = actionId;
        }

        protected override void OnReceive(byte[] bytes)
        {
            foreach (var subscriber in Subscribers)
            {
                var header = new Header(subscriber.UserId, subscriber.SubscriptionId, _hub, _actionId, MessageType.Publish, bytes);

                _sendMsg?.Invoke(header.ProtoSerialise(), subscriber.Endpoint);
            }
        }
    }
}
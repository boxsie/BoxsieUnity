using System.Collections.Generic;
using Boxsie.Network.Core.Messaging;

namespace Boxsie.Network.Repositories.Redis
{
    public class RedisPubSubReceiveDto
    {
        public List<SocketSubscriberModel> Subscribers { get; set; }
        public byte[] Data { get; set; }
    }
}
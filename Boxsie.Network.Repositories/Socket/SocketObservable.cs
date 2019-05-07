using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Core.Objects;
using Boxsie.Network.Repositories.Redis;

namespace Boxsie.Network.Repositories.Socket
{
    public class SocketObservable<T> : ObservableRedisRepository<T, SocketSubscriberModel> where T : IRepositoryItem, new()
    {
        public SocketObservable(RedisSubscribe<SocketSubscriberModel> subscribe) : base(subscribe) { }
    }
}
using System;
using System.Net;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Objects;
using Boxsie.Network.Repositories.Interfaces;
using Boxsie.Network.Repositories.Redis;
using Boxsie.Network.Repositories.Socket;

namespace Boxsie.Network.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepository<T> GetRepository<T>() where T : IRepositoryItem, new()
        {
            return new RedisRepository<T>();
        }

        public IRepository<T> GetPublisher<T>() where T : IRepositoryItem, new()
        {
            return new PublisherRedisRepository<T>();
        }

        public SocketObservable<T> GetSocketObservable<T>(Action<byte[], IPEndPoint> sendMsg, HubType hub, int actionId) where T : IRepositoryItem, new()
        {
            var repo = new SocketObservable<T>(new SocketSubscribe(sendMsg, hub, actionId));
            repo.Register();
            return repo;
        }

        public SocketSubscribe GetSocketPubSub(Action<byte[], IPEndPoint> sendMsg, string topic, string objectName, HubType hub, int actionId)
        {
            var pubsub = new SocketSubscribe(sendMsg, hub, actionId);
            pubsub.Register(topic, objectName);
            return pubsub;
        }
    }
}
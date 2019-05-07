using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Data;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Repositories.Interfaces;
using StackExchange.Redis;

namespace Boxsie.Network.Repositories.Redis
{
    public abstract class RedisSubscribe<T> : ISubscribe<T>, IPublish where T : ISubscriberModel
    {
        public bool PubSubEnabled { get; private set; }
        
        protected readonly List<T> Subscribers;

        private const string PubSubId = "PubSub";
        private RedisDto _redisKey;

        protected RedisSubscribe()
        {
            Subscribers = new List<T>();
            PubSubEnabled = false;
        }

        protected abstract void OnReceive(byte[] bytes);

        public virtual void Register(string topicKey, string objectKey)
        {
            CreateRedisKey(topicKey, objectKey);

            RedisHelper.Subscribe(_redisKey, (channel, value) => OnReceive(value));
        }

        public virtual void Subscribe(T subscriber)
        {
            Subscribers.Add(subscriber);
        }

        public virtual void Unsubscribe(string id)
        {
            var sub = Subscribers.FirstOrDefault(x => x.Id == id);

            if (sub != null)
                Subscribers.Remove(sub);
        }

        public virtual void Publish(PublishType publishType, byte[] data)
        {
            var dto = new PubSubDto
            {
                PublishType = publishType,
                Data = data
            };

            RedisHelper.Publish(_redisKey, dto.ProtoSerialise());
        }

        private void CreateRedisKey(string topicKey, string objectKey)
        {
            _redisKey = new RedisDto
            {
                TopicKey = topicKey,
                ObjectKey = objectKey,
                Id = PubSubId
            };
        }
    }
}
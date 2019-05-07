using System;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Data;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Core.Objects;
using Boxsie.Network.Repositories.Interfaces;

namespace Boxsie.Network.Repositories.Redis
{
    public class ObservableRedisRepository<T, TY> : RedisRepository<T>, IObservableRepository<T, TY> where T : IRepositoryItem, new() where TY : ISubscriberModel
    {
        private readonly RedisSubscribe<TY> _subscribe;

        public ObservableRedisRepository(RedisSubscribe<TY> subscribe)
        {
            _subscribe = subscribe;
        }

        public override void Update(T obj)
        {
            var bytes = obj.ProtoSerialise();

            base.Update(obj.ItemId, bytes);
            Publish(PublishType.Update, bytes);
        }

        public override void Update(Guid itemId, byte[] bytes)
        {
            base.Update(itemId, bytes);
            Publish(PublishType.Update, bytes);
        }

        public override void Delete(Guid id)
        {
            base.Delete(id);
            Publish(PublishType.Delete, id.ProtoSerialise());
        }

        public void Register()
        {
            _subscribe.Register(TopicKey, ObjectKey);
        }

        public void Subscribe(TY subscriber)
        {
            _subscribe.Subscribe(subscriber);
        }

        public void Unsubscribe(string id)
        {
            _subscribe.Unsubscribe(id);
        }

        protected void Publish(PublishType publishType, byte[] data = null)
        {
            _subscribe.Publish(publishType, data);
        }
    }
}
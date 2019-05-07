using System;
using System.Threading.Tasks;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Data;
using Boxsie.Network.Core.Objects;
using Boxsie.Network.Repositories.Interfaces;

namespace Boxsie.Network.Repositories.Redis
{
    public class PublisherRedisRepository<T> : RedisRepository<T>, IPublish where T : IRepositoryItem, new()
    {
        private readonly RedisDto _redisKey;
        private const string PubSubId = "PubSub";

        public PublisherRedisRepository()
        {
            _redisKey = new RedisDto
            {
                TopicKey = TopicKey,
                ObjectKey = ObjectKey,
                Id = PubSubId
            };
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

        public void Publish(PublishType publishType, byte[] data = null)
        {
            var dto = new PubSubDto
            {
                PublishType = publishType,
                Data = data
            };

            RedisHelper.Publish(_redisKey, dto.ProtoSerialise());
        }
    }
}
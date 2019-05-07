using System;
using System.Collections.Generic;
using Boxsie.Core.Debug;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Data;
using Boxsie.Network.Core.Objects;
using Boxsie.Network.Repositories.Interfaces;

namespace Boxsie.Network.Repositories.Redis
{
    public class RedisRepository<T> : IRepository<T> where T : IRepositoryItem, new()
    {
        public bool UseCollection { get; set; }

        protected readonly string TopicKey;
        protected readonly string ObjectKey;
        
        private const string CollectionId = "Collection";
        
        public RedisRepository()
        {
            var t = new T();
            TopicKey = t.Hub.ToString();
            ObjectKey = t.ObjectName;
            
            UseCollection = true;
        }

        public T Get(Guid id)
        {
            var value = RedisHelper.Get($"{TopicKey}:{ObjectKey}:{id}");
            return value.HasValue 
                ?  ((byte[]) value).ProtoDeserialise<T>() 
                : default(T);
        }

        public List<T> GetAll()
        {
            var retList = new List<T>();

            if (UseCollection)
            {
                var collection = GetCollection();

                foreach (var redisDto in collection)
                    retList.Add(redisDto.Value.Bytes.ProtoDeserialise<T>());
            }

            return retList;
        }

        public virtual void Update(T obj)
        {
            Update(obj.ItemId, obj.ProtoSerialise());
        }

        public virtual void Update(Guid itemId, byte[] bytes)
        {
            try
            {
                var dto = CreateDto(itemId.ToString(), bytes);
                RedisHelper.Set(dto);

                if (UseCollection)
                {
                    var collection = GetCollection();

                    if (collection.ContainsKey(itemId))
                        collection[itemId] = dto;
                    else
                        collection.Add(itemId, dto);


                    SetCollection(collection);
                }
            }
            catch (KeyNotFoundException ex)
            {
                Debug.Log(ex.Message, DebugLogType.Warning);
            }
        }

        public virtual void Delete(Guid id)
        {
            try
            {
                var dto =  CreateDto(id.ToString());
                RedisHelper.Remove(dto);

                if (UseCollection)
                {
                    var collection = GetCollection();
                    collection.Remove(id);

                    SetCollection(collection);
                }
            }
            catch (KeyNotFoundException ex)
            {
                Debug.Log(ex.Message, DebugLogType.Warning);
            }
        }
        
        protected RedisDto CreateDto(string id, byte[] bytes = null)
        {
            return new RedisDto
            {
                TopicKey = TopicKey,
                ObjectKey = ObjectKey,
                Id = id,
                Bytes = bytes
            };
        }

        private Dictionary<Guid, RedisDto> GetCollection()
        {
            var collectionDto = CreateDto(CollectionId);
            var value = RedisHelper.Get(collectionDto.Key);
            return value.HasValue
                ?  ((byte[]) value).ProtoDeserialise<Dictionary<Guid, RedisDto>>() 
                : new Dictionary<Guid, RedisDto>();
        }

        private void SetCollection(Dictionary<Guid, RedisDto> collection)
        {
            var collectionDto =  CreateDto(CollectionId,  collection.ProtoSerialise());
            RedisHelper.Set(collectionDto);
        }
    }
}
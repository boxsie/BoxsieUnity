using System;
using System.Threading.Tasks;
using Boxsie.Core.Debug;
using Boxsie.Network.Core.Data;
using StackExchange.Redis;

namespace Boxsie.Network.Repositories.Redis
{
    public static class RedisHelper
    {
        private static ConnectionMultiplexer _connection;
        private static readonly TimeSpan ExpireTime = TimeSpan.FromHours(1);

        public static bool Connect()
        {
            if (_connection == null || !_connection.IsConnected)
                _connection = ConnectionMultiplexer.Connect("redis:6379,allowAdmin=true");

            return _connection.IsConnected;
        }

        public static RedisValue Get(string key)
        {
            Debug.Log($"Getting redis key '{key}'.");
            var cache = _connection.GetDatabase();
            cache.KeyExpire(key, ExpireTime);
            return cache.StringGet(key);
        }

        public static void Set(RedisDto dto)
        {
            Debug.Log($"Setting redis key '{dto.Key}' for {dto.TopicKey}.");
            _connection.GetDatabase().StringSet(dto.Key, dto.Bytes, ExpireTime);
        }

        public static void Remove(RedisDto dto)
        {
            Debug.Log($"Removing redis key '{dto.Key}' for {dto.TopicKey}.");
            _connection.GetDatabase().KeyDelete(dto.Key);
        }

        public static void Subscribe(RedisDto dto, Action<RedisChannel, RedisValue> onMessageReceived)
        {
            Debug.Log($"Subscribing to redis channel '{dto.Key}' for {dto.TopicKey}.");
            _connection.GetSubscriber().Subscribe(dto.Key, onMessageReceived);
        }

        public static void Publish(RedisDto dto, RedisValue val)
        {
            Debug.Log($"Publishing to redis channel '{dto.Key}' for {dto.TopicKey}.");
            _connection.GetSubscriber().Publish(dto.Key, val);
        }

        public static void Unsubscribe(RedisDto dto, Action<RedisChannel, RedisValue> onMessageReceived)
        {
            Debug.Log($"Unsubscribing from redis channel '{dto.Key}' for {dto.TopicKey}.");
            _connection.GetSubscriber().Unsubscribe(dto.Key, onMessageReceived);
        }

        public static void Clear()
        {
            var endpoints = _connection.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        public static async Task<RedisValue> GetAsync(string key)
        {
            Debug.Log($"Getting redis key '{key}'.");
            var cache = _connection.GetDatabase();
            await cache.KeyExpireAsync(key, ExpireTime);
            return await cache.StringGetAsync(key);
        }

        public static async Task SetAsync(RedisDto dto)
        {
            Debug.Log($"Setting redis key '{dto.Key}' for {dto.TopicKey}.");
            await _connection.GetDatabase().StringSetAsync(dto.Key, dto.Bytes, ExpireTime);
        }

        public static async Task RemoveAsync(RedisDto dto)
        {
            Debug.Log($"Removing redis key '{dto.Key}' for {dto.TopicKey}.");
            await _connection.GetDatabase().KeyDeleteAsync(dto.Key);
        }

        public static async Task SubscribeAsync(RedisDto dto, Action<RedisChannel, RedisValue> onMessageReceived)
        {
            Debug.Log($"Subscribing to redis channel '{dto.Key}' for {dto.TopicKey}.");
            await _connection.GetSubscriber().SubscribeAsync(dto.Key, onMessageReceived);
        }

        public static async Task PublishAsync(RedisDto dto, RedisValue val)
        {
            Debug.Log($"Publishing to redis channel '{dto.Key}' for {dto.TopicKey}.");
            await _connection.GetSubscriber().PublishAsync(dto.Key, val);
        }

        public static async Task UnsubscribeAsync(RedisDto dto, Action<RedisChannel, RedisValue> onMessageReceived)
        {
            Debug.Log($"Unsubscribing from redis channel '{dto.Key}' for {dto.TopicKey}.");
            await _connection.GetSubscriber().UnsubscribeAsync(dto.Key, onMessageReceived);
        }

        public static async Task ClearAsync()
        {
            var endpoints = _connection.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _connection.GetServer(endpoint);
                await server.FlushAllDatabasesAsync();
            }
        }
    }
}

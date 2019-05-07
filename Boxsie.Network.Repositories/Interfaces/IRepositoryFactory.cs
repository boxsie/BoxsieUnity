using System;
using System.Net;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Objects;
using Boxsie.Network.Repositories.Socket;

namespace Boxsie.Network.Repositories.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : IRepositoryItem, new();
        IRepository<T> GetPublisher<T>() where T : IRepositoryItem, new();
        SocketObservable<T> GetSocketObservable<T>(Action<byte[], IPEndPoint> sendMsg, HubType hub, int actionId) where T : IRepositoryItem, new();
        SocketSubscribe GetSocketPubSub(Action<byte[], IPEndPoint> sendMsg, string topic, string objectName, HubType hub, int actionId);
    }
}
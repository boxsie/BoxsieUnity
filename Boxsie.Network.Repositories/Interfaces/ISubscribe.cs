using Boxsie.Network.Core.Messaging;

namespace Boxsie.Network.Repositories.Interfaces
{
    public interface ISubscribe<in T> where T : ISubscriberModel
    {
        void Register(string topicKey, string objectKey);
        void Subscribe(T subscriber);
        void Unsubscribe(string id);
    }
}
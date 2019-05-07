using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Sockets.Core;

namespace Boxsie.Network.Hub.Service.Core
{
    public interface IHubAction
    {
        void RegisterService(ISocketService socketService);
        void Request(Msg msg);
        void Publish(Msg msg);
        void Subscribe(Msg msg);
        void Unsubscribe(Msg msg);
    }
}
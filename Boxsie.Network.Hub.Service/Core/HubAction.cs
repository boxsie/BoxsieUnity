using Boxsie.Core.Debug;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Sockets.Core;

namespace Boxsie.Network.Hub.Service.Core
{
    public abstract class HubAction : IHubAction
    {
        protected ISocketService SocketService;

        public void RegisterService(ISocketService socketService)
        {
            SocketService = socketService;
        }

        public virtual void Request(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is trying to request to A: '{msg.Action}' and this action does not accept requests.");
        }

        public virtual void Publish(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is trying to publish to A: '{msg.Action}' and this action does not accept subscriptions.");
        }

        public virtual void Subscribe(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is trying to subscribe to A: '{msg.Action}' and this action does not accept subscriptions.");
        }

        public virtual void Unsubscribe(Msg msg)
        {
            Debug.Log($"'{msg.Connection.Username}' is trying to unsubscribe to A: '{msg.Action}' and this action does not accept subscriptions.");
        }
    }
}
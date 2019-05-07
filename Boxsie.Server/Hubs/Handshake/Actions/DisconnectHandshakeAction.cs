using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Hub.Service.Attributes;
using Boxsie.Network.Hub.Service.Core;

namespace Boxsie.Server.Hubs.Handshake.Actions
{
    [HubType(HubType.Handshake)]
    [ActionType((int) HandshakeActionType.Disconnect, IsDisconnectAction = true)]
    public class DisconnectHandshakeAction : HubAction
    {
        private readonly ISession _session;

        public void Init() { }

        public DisconnectHandshakeAction(ISession session)
        {
            _session = session;
        }

        public override void Request(Msg msg)
        {
            _session.EndSession(msg.Connection.SessionId);
        }

        public override void Publish(Msg msg)
        {

        }

        public override void Subscribe(Msg msg)
        {

        }

        public override void Unsubscribe(Msg msg)
        {

        }
    }
}
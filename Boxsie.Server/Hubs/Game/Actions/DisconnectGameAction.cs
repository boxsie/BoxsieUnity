using Boxsie.Network.Core.Connection;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Lobby;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Hub.Service.Attributes;
using Boxsie.Network.Hub.Service.Core;
using Boxsie.Network.Repositories.Interfaces;

namespace Boxsie.Server.Hubs.Game.Actions
{
    [HubType(HubType.Game)]
    [ActionType((int)GameActionType.Disconnect, IsDisconnectAction = true)]
    public class DisconnectGameAction : HubAction
    {
        private readonly IRepository<PeerEndpointModel> _peers;

        public DisconnectGameAction(IRepositoryFactory repositoryFactory)
        {
            _peers = repositoryFactory.GetRepository<PeerEndpointModel>();
        }

        public override void Request(Msg msg)
        {
            var peer = _peers.Get(msg.Connection.SessionId);

            Header header;

            if (peer == null)
            {
                header = msg.GetResponseHeader(MessageType.Failed);
            }
            else
            {
                header =  msg.GetResponseHeader(MessageType.Success);
                _peers.Delete(msg.Connection.SessionId);
            }

            SocketService.SendMessageToClient(header, msg.SenderEndPoint, true);
        }
    }
}
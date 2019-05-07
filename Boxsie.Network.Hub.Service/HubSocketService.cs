using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Boxsie.Core.Debug;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Messaging;
using Boxsie.Network.Hub.Service.Core;
using Boxsie.Network.Repositories.SQL;
using Boxsie.Network.Sockets.Core;
using Boxsie.Network.Sockets.Helpers;
using Boxsie.Network.Sockets.Service;

namespace Boxsie.Network.Hub.Service
{
    public class HubSocketService : SocketService, IHubService
    {
        private readonly ISession _session;
        private readonly IUserQuery _userQuery;
        private readonly Dictionary<HubType, Core.Hub> _hubs;

        public HubSocketService(ISocketServer socket, ISession session, IUserQuery userQuery, HubAction[] actions, IMessageLoop messageLoop) : base(socket, messageLoop)
        {
            _session = session;
            _userQuery = userQuery;

            _hubs = RegisterHubs(actions);
        }
        
        public void Disconnect()
        {

        }

        public override void SendMessageToClient(IHeader header, IPEndPoint receiver, bool isReliable = true)
        {
            var hubHeader = (Header)header;

            Debug.Log($"MSG-OUT To: '{hubHeader.SenderSessionId}' TId: '{hubHeader.TransactionId}' L: '{hubHeader.Data?.Length.ToString("##,###") ?? "0"}b' H: '{hubHeader.Hub}' A: '{hubHeader.Action}' MT: '{hubHeader.MessageType}'");

            base.SendMessageToClient(header, receiver, isReliable);
        }

        public override void SendMessageToAll(IHeader header, bool isReliable)
        {
            var hubHeader = (Header)header;

            Debug.Log($"MSG-OUT To: '{hubHeader.SenderSessionId}' TId: '{hubHeader.TransactionId}' L: '{hubHeader.Data?.Length.ToString("##,###") ?? "0"}b' H: '{hubHeader.Hub}' A: '{hubHeader.Action}' MT: '{hubHeader.MessageType}'");

            base.SendMessageToAll(header, isReliable);
        }

        protected override void SocketOnConnect(IPEndPoint endpoint, byte[] connectrequest, int channel)
        {
            Debug.Log("Client not connected, attempting handshake.");

            var msg = HubHelper.CreateMsg(endpoint, connectrequest);
            var handshake = HubHelper.GetHandshake(msg.Data);

            if (handshake == null)
            {
                Debug.Log($"'{ msg.SenderEndPoint }' is trying to connect with an incorrect or corrupt version Id.", DebugLogType.Warning);
                SocketServer.Kick(endpoint, "Version is incorrect!");
                return;
            }

            msg.SetUser(_session.CreateSession(msg.SenderEndPoint));
            msg.SetData(handshake.UserDto.ProtoSerialise());

            if (Authenticate(msg))
            {
                Approve(msg);

                Debug.Log($"'{ msg.SenderEndPoint }' has successfully connected and authenticated.");
            }
            else
                Debug.Log($"Connection from '{ msg.SenderEndPoint }' has been refused.");
        }

        protected override void SocketOnIncomingMsg(IPEndPoint endpoint, byte[] msgBytes, int channel)
        {
            var msg = HubHelper.CreateMsg(endpoint, msgBytes);

            msg.SetUser(_session.GetSession(msg.SessionId));

            if (msg.Connection.IsAuthenticated)
                RouteIncomingMsg(msg);
        }

        protected override void SocketOnDisconnect(IPEndPoint endpoint)
        {
            var sessionId = SocketServer.GetTag<Guid>(endpoint);

            if (sessionId == Guid.Empty)
                return;

            var msg = new Msg
            {
                SessionId = sessionId,
                TransactionId = Guid.NewGuid(),
                SenderEndPoint = endpoint
            };

            msg.SetUser(_session.GetSession(msg.SessionId));

            RouteDisconnectMessage(msg);
        }

        private Dictionary<HubType, Core.Hub> RegisterHubs(HubAction[] actions)
        {
            var hubs = new Dictionary<HubType, Core.Hub>();

            foreach (HubType hubType in Enum.GetValues(typeof(HubType)))
            {
                var hubActions = actions.Where(x => x.HubType() == hubType).ToList();

                foreach (var hubAction in hubActions)
                    hubAction.RegisterService(this);

                hubs.Add(hubType, new global::Boxsie.Network.Hub.Service.Core.Hub(hubType, hubActions));
            }

            return hubs;
        }

        private void RouteIncomingMsg(Msg msg)
        {
            if (msg.Hub == null || msg.Action == null)
            {
                Debug.Log($"'{msg.Connection.Username}' message has a corrupt header.");
                return;
            }

            var hubId = msg.Hub.Value;
            var actionId = msg.Action.Value;

            var hub = _hubs.ContainsKey(hubId) ? _hubs[hubId] : null;

            if (hub == null)
            {
                Debug.Log($"'{msg.Connection.Username}' requested hub '{hubId}' which doesn't exist.");
                return;
            }

            //TODO ADD HUB AUTH
            if (hub.ActionAuthLevels.ContainsKey(msg.Action.Value))
            {
                var actionAuth = hub.ActionAuthLevels[actionId];

                var userAuth = hub.HubType != HubType.Handshake
                    ? msg.Connection.HubAuths[msg.Hub.Value]
                    : AuthLevels.Super;

                if ((int)userAuth > (int)actionAuth)
                {
                    Debug.Log($"'{msg.Connection.Username}' failed autherisation for action '{actionId}' on hub '{hub.HubType}'.");
                    return;
                }

                if (!hub.Actions.ContainsKey(actionId))
                {
                    Debug.Log($"'{msg.Connection.Username}' tried to call an action that doesn't exist on hub '{hub.HubType}'.");
                    return;
                }

                switch (msg.MessageType)
                {
                    case MessageType.Message:
                    case MessageType.Request:
                        hub.Actions[actionId].Request(msg);
                        break;
                    case MessageType.Subscribe:
                        hub.Actions[actionId].Subscribe(msg);
                        break;
                    case MessageType.Publish:
                        hub.Actions[actionId].Publish(msg);
                        break;
                    case MessageType.Unsubscribe:
                        hub.Actions[actionId].Unsubscribe(msg);
                        break;
                }
                return;
            }

            Debug.Log($"'{msg.Connection.Username}' failed autherisation for action '{actionId}' on hub '{hub.HubType}' due to a missing autherisation key.");
        }

        private void RouteDisconnectMessage(Msg msg)
        {
            foreach (var hub in _hubs.Select(x => x.Value))
            {
                IHubAction disconnect = null;

                foreach (var action in hub.Actions.Select(x => x.Value))
                {
                    var actionType = action.ActionType();

                    msg.SetDestination(hub.HubType, actionType.Id);

                    if (actionType.IsDisconnectAction)
                        disconnect = action;

                    action.Unsubscribe(msg);
                }

                //var request = disconnect?.Request(msg);

                //if (request != null)
                //    request;
            }
        }

        private bool Authenticate(Msg msg)
        {
            var clientUser = HubHelper.DecryptClientUserData(msg);
            var serverUser = _userQuery.GetUser(clientUser.Username);

            if (serverUser == null)
            {
                Debug.Log($"'{ msg.SenderEndPoint }' attempted to log in with an unknown username '{ clientUser.Username }'.", DebugLogType.Warning);
                return false;
            }

            var authLevels = _userQuery.GetUserAuth(serverUser.Id);
            var approved = _session.AuthenticateUser(msg.Connection, authLevels, clientUser, serverUser);

            Debug.Log(approved
                ? $"User '{msg.SessionId}' has been successfully authenticated."
                : $"User '{msg.SessionId}' has failed authentication.");

            return approved;
        }

        private void Approve(Msg msg)
        {
            var header = new Header(Guid.Empty, msg.TransactionId, HubType.Handshake, (int)HandshakeActionType.Connect, MessageType.Success, msg.SessionId.ProtoSerialise());

            Debug.Log($"MSG-OUT To: '{msg.SenderEndPoint}' TId: '{header.TransactionId}' L: '{header.Data?.Length.ToString("##,###") ?? "0"}b' H: '{header.Hub}' A: '{header.Action}' MT: '{header.MessageType}'");

            SocketServer.SetTag(msg.SenderEndPoint, msg.SessionId);
            SocketServer.Approve(msg.SenderEndPoint, header.ProtoSerialise());
        }
    }
}
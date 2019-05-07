using Boxsie.Network.Core.Enums;
using Boxsie.Network.Sockets.Core;
using Lidgren.Network;

namespace Boxsie.Network.Sockets.Service
{
    public class Socket : SocketCore, ISocket
    {
        public event OnConnectHandle OnConnect;
        public event OnIncomingMsgHandle OnIncomingMsg;
        public event OnDisconnectHandle OnDisconnect;

        public void MessageLoop()
        {
            MessageRoute route;
            NetIncomingMessage nim;

            while ((route = GetMessageRoute(out nim)) != MessageRoute.Empty)
            {
                if (route == MessageRoute.Unhandled)
                    continue;

                switch (route)
                {
                    case MessageRoute.Connected:
                    case MessageRoute.ConnectionApproval:
                        if (OnConnect != null)
                        {
                            if (IsHost && route == MessageRoute.ConnectionApproval || !IsHost && route == MessageRoute.Connected)
                                OnConnect?.Invoke(nim.SenderEndPoint, NIMToBytes(nim.SenderConnection.RemoteHailMessage), nim.SequenceChannel);
                        }
                        break;
                    case MessageRoute.Data:
                        OnIncomingMsg?.Invoke(nim.SenderEndPoint, NIMToBytes(nim), nim.SequenceChannel);
                        break;
                    case MessageRoute.Disconnected:
                        OnDisconnect?.Invoke(nim.SenderEndPoint);
                        break;
                }
            }
        }
    }
}
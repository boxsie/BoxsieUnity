using Boxsie.Core.Debug;
using Lidgren.Network;

namespace Boxsie.Network.Sockets.Helpers
{
    public static class SocketHelper
    {
        public static string AppName => "Bx";
        public static int HubPortExt => 8558;
        public static int PeerPortExt => 8559;
        public static float Version => 0.1f;
        public static string HubEncryptId => "d3nH437hdy35h15h41r";

        public static void LogSocketMessage(NetIncomingMessage nim, NetConnectionStatus netStatus)
        {
            string response;

            switch (nim.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    Debug.Log($"Network Status Change: '{netStatus}'", DebugLogType.Info, 2);
                    return;
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    Debug.Log($"Network Debug: '{nim.ReadString()}'", DebugLogType.Warning, 2);
                    return;
                case NetIncomingMessageType.Receipt:
                case NetIncomingMessageType.DiscoveryRequest:
                case NetIncomingMessageType.DiscoveryResponse:
                case NetIncomingMessageType.NatIntroductionSuccess:
                case NetIncomingMessageType.ConnectionLatencyUpdated:
                case NetIncomingMessageType.UnconnectedData:
                    response = "Unhandled message";
                    break;
                case NetIncomingMessageType.ConnectionApproval:
                    response = "Handshake pending";
                    break;
                case NetIncomingMessageType.Data:
                    response = "Incoming data";
                    break;
                default:
                    response = "Unknown error";
                    break;
            }

            Debug.Log($"MSG-IN From: '{nim.SenderEndPoint}' Type: '{nim.MessageType}' Output: {response}", DebugLogType.Info, 2);
        }
    }
}

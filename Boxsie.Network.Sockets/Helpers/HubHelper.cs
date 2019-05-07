using System;
using System.Net;
using Boxsie.Core.Debug;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Core.Messaging;

namespace Boxsie.Network.Sockets.Helpers
{
    public static class HubHelper
    {
        public static Msg CreateMsg(IPEndPoint endPoint, byte[] bytes)
        {
            var header = bytes.ProtoDeserialise<Header>();

            return new Msg
            {
                SessionId = header.SenderSessionId,
                TransactionId = header.TransactionId,
                SenderEndPoint = endPoint,
                Hub = header.Hub,
                Action = header.Action,
                MessageType = header.MessageType,
                Data = header.Data
            };
        }

        public static ServiceHandshake GetHandshake(byte[] handshakeData)
        {
            var handshake = handshakeData.ProtoDeserialise<ServiceHandshake>();

            if (handshake == null)
            {
                Debug.Log("Connecting client handshake is corrupt.", DebugLogType.Warning);
                return null;
            }

            if (!(handshake.Version < SocketHelper.Version))
                return handshake;

            Debug.Log("Connecting client version is not compatible.", DebugLogType.Warning);

            return null;
        }

        public static UserDto DecryptClientUserData(Msg msg)
        {
            var clientDto = msg.Data.ProtoDeserialise<UserDto>();

            string responseText = null;

            if (clientDto == null)
                responseText = "Authentication credentials are missing.";
            else if (msg.Hub != HubType.Handshake || msg.Action != (int)HandshakeActionType.Connect)
                responseText = "Handshake header is incomplete.";

            if (string.IsNullOrEmpty(responseText))
                return clientDto;

            Debug.Log(responseText, DebugLogType.Warning);

            return null;
        }
    }
}
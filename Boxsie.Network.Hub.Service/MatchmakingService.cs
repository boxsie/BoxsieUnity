using System;
using System.Net;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Repositories.Interfaces;
using Boxsie.Network.Sockets.Core;
using Boxsie.Network.Sockets.Service;

namespace Boxsie.Network.Hub.Service
{
    public class MatchmakingService : SocketService, IMatchmakingService
    {
        private readonly IRepository<PeerEndpointModel> _peers;

        public MatchmakingService(IRepositoryFactory repositoryFactory, ISocketServer socketServer, IMessageLoop messageLoop) : base(socketServer, messageLoop)
        {
            _peers = repositoryFactory.GetRepository<PeerEndpointModel>();
        }

        protected override void SocketOnConnect(IPEndPoint endpoint, byte[] connectrequest, int channel)
        {

        }

        protected override void SocketOnIncomingMsg(IPEndPoint endpoint, byte[] msgBytes, int channel)
        {

        }

        protected override void SocketOnDisconnect(IPEndPoint endpoint)
        {

        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
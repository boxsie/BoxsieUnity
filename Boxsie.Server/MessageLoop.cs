using System.Threading.Tasks;
using Boxsie.Network.Sockets.Service;

namespace Boxsie.Server
{
    public class MessageLoop : IMessageLoop
    {
        private ISocket _socket;
        
        public Task TheLoop()
        {
            while (true)
            {
                _socket.MessageLoop();
            }
        }

        public void Start(ISocket socket)
        {
            _socket = socket;

            TheLoop();
        }
    }
}
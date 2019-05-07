namespace Boxsie.Network.Sockets.Service
{
    public interface IMessageLoop
    {
        void Start(ISocket socket);
    }
}
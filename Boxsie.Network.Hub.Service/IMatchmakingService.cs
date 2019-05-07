namespace Boxsie.Network.Hub.Service
{
    public interface IMatchmakingService
    {
        void Start(int localPort, string encryptId);
        void Disconnect();
    }
}
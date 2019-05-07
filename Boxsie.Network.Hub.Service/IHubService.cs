namespace Boxsie.Network.Hub.Service
{
    public interface IHubService
    {
        void Start(int localPort, string encryptId);
        void Disconnect();
    }
}
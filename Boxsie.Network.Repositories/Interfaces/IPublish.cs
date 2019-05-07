using Boxsie.Network.Core.Data;

namespace Boxsie.Network.Repositories.Interfaces
{
    public interface IPublish
    {
        void Publish(PublishType publishType, byte[] data = null);
    }
}
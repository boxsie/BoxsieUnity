using System.Linq;
using Boxsie.Network.Core.Enums;
using Boxsie.Network.Hub.Service.Attributes;

namespace Boxsie.Network.Hub.Service.Core
{
    public static class Extend
    {
        public static HubType HubType(this IHubAction hubAction)
        {
            return ((HubTypeAttribute)hubAction.GetType().GetCustomAttributes(typeof(HubTypeAttribute), false).FirstOrDefault()).Hub;
        }

        public static ActionTypeAttribute ActionType(this IHubAction hubAction)
        {
            return (ActionTypeAttribute)hubAction.GetType().GetCustomAttributes(typeof(ActionTypeAttribute), false).FirstOrDefault();
        }

        public static AuthLevels AuthLevel(this IHubAction hubAction)
        {
            var authLevel = (AuthLevelAttribute)hubAction.GetType().GetCustomAttributes(typeof(AuthLevelAttribute), false).FirstOrDefault();
            return authLevel?.AuthLevel ?? AuthLevels.Guest;
        }
    }
}

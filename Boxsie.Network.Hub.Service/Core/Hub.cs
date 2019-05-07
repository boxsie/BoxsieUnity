using System.Collections.Generic;
using Boxsie.Network.Core.Enums;

namespace Boxsie.Network.Hub.Service.Core
{
    public class Hub
    {
        public HubType HubType { get; set; }
        public Dictionary<int, IHubAction> Actions { get; }
        public Dictionary<int, AuthLevels> ActionAuthLevels { get; }

        public Hub(HubType hubType, IEnumerable<IHubAction> actions)
        {
            HubType = hubType;
            Actions = new Dictionary<int, IHubAction>();
            ActionAuthLevels = new Dictionary<int, AuthLevels>();

            foreach (var action in actions)
            {
                var actionType = action.ActionType();

                Actions.Add(actionType.Id, action);
                ActionAuthLevels.Add(actionType.Id, action.AuthLevel());
            }
        }
    }
}
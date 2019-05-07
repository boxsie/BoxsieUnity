using System;
using Boxsie.Network.Core.Enums;

namespace Boxsie.Network.Hub.Service.Attributes
{
    public class ActionTypeAttribute : Attribute
    {
        public int Id { get; set; }
        public bool IsDisconnectAction { get; set; }

        public ActionTypeAttribute(int id, bool isDisconnect = false)
        {
            Id = id;
            IsDisconnectAction = isDisconnect;
        }
    }

    public class HubTypeAttribute : Attribute
    {
        public HubType Hub { get; set; }

        public HubTypeAttribute(HubType hub)
        {
            Hub = hub;
        }
    }
}
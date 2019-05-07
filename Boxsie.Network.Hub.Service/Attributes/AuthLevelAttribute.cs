using System;
using Boxsie.Network.Core.Enums;

namespace Boxsie.Network.Hub.Service.Attributes
{
    public class AuthLevelAttribute : Attribute
    {
        public AuthLevels AuthLevel { get; set; }

        public AuthLevelAttribute(AuthLevels authLevel)
        {
            AuthLevel = authLevel;
        }
    }
}
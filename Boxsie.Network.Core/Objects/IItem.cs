using System;
using Boxsie.Network.Core.Enums;

namespace Boxsie.Network.Core.Objects
{
    public interface IRepositoryItem
    {
        HubType Hub { get; }
        string ObjectName { get; }
        Guid ItemId { get; }
    }
}
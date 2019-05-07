using System;

namespace Boxsie.Network.Core.Objects
{
    public interface IVal
    {
        bool NeedsUpdate { get; }
    }
}
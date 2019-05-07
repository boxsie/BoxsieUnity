using System;
using System.Collections.Generic;
using System.Net;
using Boxsie.Network.Core.Connection;

namespace Boxsie.Network.Hub.Service.Core
{
    public interface ISession
    {
        ConnectionModel CreateSession(IPEndPoint endpoint);
        bool AuthenticateUser(ConnectionModel connection, IEnumerable<UserAuthDto> userAuths, UserDto clientDto, UserDto serverDto);
        ConnectionModel GetSession(Guid id);
        void EndSession(Guid userId);
    }
}
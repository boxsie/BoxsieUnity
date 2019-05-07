using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Boxsie.Core.Debug;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Connection;
using Boxsie.Network.Repositories.Interfaces;

namespace Boxsie.Network.Hub.Service.Core
{
    public class Session : ISession
    {
        private readonly IRepository<ConnectionModel> _connections;

        public Session(IRepositoryFactory repositoryFactory)
        {
            _connections = repositoryFactory.GetRepository<ConnectionModel>();

            _connections.UseCollection = false;
        }

        public ConnectionModel CreateSession(IPEndPoint endPoint)
        {
            var connection = new ConnectionModel
            {
                Endpoint = endPoint.ToString()
            };

            _connections.Update(connection);

            return connection;
        }

        public bool AuthenticateUser(ConnectionModel connection, IEnumerable<UserAuthDto> userAuths, UserDto clientDto, UserDto serverDto)
        {
            Debug.Log("Attempting to authenticate.");

            var clientPass =  CryptoHelper.GetEncryptedHash(clientDto.Password, serverDto.Salt);

            var approved = serverDto.Password == clientPass;

            if (approved)
            {
                 connection.Authenticate(clientDto, true, userAuths.ToList());
                 _connections.Update(connection);
            }
            else
            {
                connection.Authenticate(clientDto, false, new List<UserAuthDto>());
                Debug.Log($"'{clientDto.Username}' attempted to log in with an incorrect password.", DebugLogType.Warning);
            }

            return approved;
        }

        public ConnectionModel GetSession(Guid id)
        {
            return _connections.Get(id);
        }

        public void EndSession(Guid userId)
        {
            _connections.Delete(userId);
        }
    }
}

using System.Collections.Generic;
using Boxsie.Network.Core.Connection;

namespace Boxsie.Network.Repositories.SQL
{
    public interface IUserQuery
    {
        void Create(UserDto obj, IEnumerable<UserAuthDto> auth);
        UserDto GetUser(string username);
        IEnumerable<UserAuthDto> GetUserAuth(int dataId);
    }
}
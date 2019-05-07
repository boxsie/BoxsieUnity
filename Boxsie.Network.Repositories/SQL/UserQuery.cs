using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Boxsie.Core.Debug;
using Boxsie.Network.Core;
using Boxsie.Network.Core.Connection;
using Dapper;

namespace Boxsie.Network.Repositories.SQL
{
    public class UserQuery : IUserQuery
    {
        private const string BoxsieDb = "Server=tcp:boxsiedb.database.windows.net,1433;Initial Catalog=boxsiedb;Persist Security Info=False;User Id=TopDog@boxsiedb;Password=H0m3r13*db;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly ConcurrentCache<string, UserDto> _userCache;
        private readonly ConcurrentCache<int, IEnumerable<UserAuthDto>> _authCache;

        public UserQuery()
        {
            _userCache = new ConcurrentCache<string, UserDto>();
            _authCache = new ConcurrentCache<int, IEnumerable<UserAuthDto>>();
        }

        public void Create(UserDto obj, IEnumerable<UserAuthDto> auth)
        {
            const string sqlQUser = @"INSERT INTO Users (Username, PasswordHash, Salt)
                                             VALUES (@username, @passhash, @salt)
                                      SELECT CAST(SCOPE_IdENTITY() as int)";

            const string sqlQAuth = @"INSERT INTO Auth (UserId, HubId, AuthLevel)
                                      VALUES (@userid, @hubid, @authlevel)";

            var usernameSalt =  obj.Username.ToBase64String();
            var passHash =  CryptoHelper.GetEncryptedHash(obj.Password, usernameSalt);
            var accountSalt =  CryptoHelper.GetRandomSalt();
            var storedPassHash =  CryptoHelper.GetEncryptedHash(passHash, accountSalt);
            
            using (var sqlCon = new SqlConnection(BoxsieDb))
            {
                var userId = sqlCon.Query<int>(sqlQUser, new { username = obj.Username, passhash = storedPassHash, salt = accountSalt }).FirstOrDefault();

                foreach (var authDto in auth)
                     sqlCon.Execute(sqlQAuth, new { userid = userId, hubid = authDto.HubId, authlevel = authDto.AuthLevel });
            }
        }

        public UserDto GetUser(string username)
        {
            var user = _userCache.Get(username);

            if (user != null)
                return user;

            const string sqlQ = @"SELECT 
                                    Id, 
                                    Username, 
                                    PasswordHash AS Password, 
                                    Salt
                                  FROM Users
                                  WHERE Username = @username";

            Debug.Log($"Getting for user '{username}' from DB.");

            using (var sqlCon = new SqlConnection(BoxsieDb))
            {
                var result = sqlCon.Query<UserDto>(sqlQ, new { username });

                user = result.FirstOrDefault();

                if (user != null)
                    _userCache.Set(username, user);

                return user;
            }
        }

        public IEnumerable<UserAuthDto> GetUserAuth(int dataId)
        {
            var auth = _authCache.Get(dataId);

            if (auth != null)
                return auth;

            const string sqlQ = @"SELECT 
                                    HubId,
	                                AuthLevel
                                  FROM Auth AS ba
                                  WHERE ba.UserId = @userid";

            Debug.Log($"Getting auth for user '{dataId}' from DB.");

            using (var sqlCon = new SqlConnection(BoxsieDb))
            {
                var result = sqlCon.Query<UserAuthDto>(sqlQ, new { userid = dataId });
                var userAuthDtos = result as UserAuthDto[] ?? result.ToArray();
                _authCache.Set(dataId, userAuthDtos);
                return userAuthDtos;
            }

        }

        public void CreateNewUser(UserDto clientUser, IEnumerable<UserAuthDto> auth)
        {
            var dbUser = GetUser(clientUser.Username);

            if (dbUser != null)
            {
                Debug.Log($"Create user '{dbUser.Username}' failed, user already exists.");
                return;
            }

             Create(clientUser, auth);
            Debug.Log($"Created user '{clientUser.Username}'.");
        }
    }
}

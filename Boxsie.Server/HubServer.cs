using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Boxsie.Core.Debug;
using Boxsie.Network.Hub.Service;
using Boxsie.Network.Hub.Service.Core;
using Boxsie.Network.Repositories;
using Boxsie.Network.Repositories.Interfaces;
using Boxsie.Network.Repositories.Redis;
using Boxsie.Network.Repositories.SQL;
using Boxsie.Network.Sockets.Core;
using Boxsie.Network.Sockets.Helpers;
using Boxsie.Network.Sockets.Service;
using Boxsie.Server.Hubs.Lobby;
using SimpleInjector;

namespace Boxsie.Server
{
    public class HubServer
    {
        private static Container _container;
        private IHubService _hubService;

        public void Start()
        {
            Debug.Log("Server starting...");

            ConnectToRedis();

            _container = new Container();

            _container.Register<ISocketServer, SocketServer>();
            _container.Register<ISession, Session>();
            _container.Register<IUserQuery, UserQuery>();
            _container.Register<IRepositoryFactory, RepositoryFactory>();
            _container.Register<ILobbyFactory, LobbyFactory>();
            _container.Register<IMessageLoop, MessageLoop>();
            _container.Register<IHubService, HubSocketService>();

            _container.RegisterCollection<HubAction>(GetTypes(typeof(HubAction)));

            _hubService = _container.GetInstance<IHubService>();

            _hubService.Start(SocketHelper.HubPortExt, SocketHelper.HubEncryptId);
        }

        public static T GetInstance<T>() where T : class
        {
            return _container.GetInstance<T>();
        }

        private static IEnumerable<Type> GetTypes(Type baseType)
        {
            var types = Assembly.GetCallingAssembly().GetExportedTypes().Where(x => baseType.IsAssignableFrom(x) && x != baseType).ToArray();
            return types;
        }

        private static void ConnectToRedis()
        {
            Debug.Log("Attempting to connect to Redis...");

            while (!RedisHelper.Connect())
            {
                Debug.Log("Unable to connect to Redis, retrying...");
                Thread.Sleep(3000);
            }

            Debug.Log("Redis connection complete!");
        }
    }
}
    

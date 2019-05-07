using System.Diagnostics;
using System.Threading;
using Boxsie.Core.Debug;

namespace Boxsie.Server.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Debug.Debug.Enable(System.Console.WriteLine);
            var bxS = new HubServer();
            bxS.Start();
            while (true) { }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using MultiplayerShooter.Library;

namespace MultiplayerShooter.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Run();
            server.End();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MultiplayerShooter.Library;
using MultiplayerShooter.Server.Commands;

namespace MultiplayerShooter.Server
{
    internal class GameMap
    {
        // Players
        public List<PlayerAndConnection> Players { get; set; }

        // Task
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        // Server
        private Server _server;
        
        public GameMap(Server server)
        {
            Players = new List<PlayerAndConnection>();

            _cancellationTokenSource = new CancellationTokenSource();
            _task = new Task(Update, _cancellationTokenSource.Token);
            _task.Start();

            _server = server;
        }

        private void Update()
        {
            var lastInterationTime = DateTime.Now;
            var stepSize = TimeSpan.FromSeconds(0.01f);
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                while (lastInterationTime + stepSize < DateTime.Now)
                {
                    Run(stepSize.Milliseconds);
                    lastInterationTime += stepSize;
                }
            }
        }

        private void Run(double gameTime)
        {
            // update all players positions
            var command = new AllPlayersCommand();
            command.Run(_server, null, null, this);
        }

        public void AddPlayer(PlayerAndConnection playerAndConnection)
        {
            Players.Add(playerAndConnection);
        }
    }
}

using MultiplayerShooter.Library.Projectiles;
using MultiplayerShooter.Server.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerShooter.Server
{
    internal class GameMap
    {
        // Players
        public List<PlayerAndConnection> Players { get; set; }

        // Projectiles
        public List<ProjectileData> Projectiles { get; set; }

        // Task
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        // Server
        private Server _server;
        
        public GameMap(Server server)
        {
            Players = new List<PlayerAndConnection>();
            Projectiles = new List<ProjectileData>();

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

        public ProjectileData AddProjectile(ProjectileData projectile)
        {
            projectile.Id = GenerateProjectileId();
            Projectiles.Add(projectile);
            return projectile;
        }

        public byte GeneratePlayerId()
        {
            Debug.Assert(Players.Count + 1 <= 255, "Players count cannot be lower than 255");
            var count = Players.Count + 1;
            return BitConverter.GetBytes(count)[0];
        }

        public int GenerateProjectileId()
        {
            return Projectiles.Count + 1;
        }
    }
}

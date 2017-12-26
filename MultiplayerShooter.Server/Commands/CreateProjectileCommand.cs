using Lidgren.Network;
using MultiplayerShooter.Library.Networking.PacketIO;

namespace MultiplayerShooter.Server.Commands
{
    class CreateProjectileCommand : ICommand
    {
        public void Run(Server server, NetIncomingMessage im, PlayerAndConnection playerAndConnection, GameMap gameMap)
        {
            var createProjectilePacketIO = new CreateProjectilePacketIO();
            var projectileData = createProjectilePacketIO.ReadRequest(im);
            var projectile = gameMap.AddProjectile(projectileData.ProjectileData);

            // broadcast the new projectile with the generated id, but not for the player who generated it
            foreach (var mapPlayerAndConnection in gameMap.Players)
            {
                if (mapPlayerAndConnection.Connection.RemoteUniqueIdentifier ==
                    im.SenderConnection.RemoteUniqueIdentifier) continue;

                var connection = mapPlayerAndConnection.Connection;
                var om = server.NetServer.CreateMessage();
                new CreateProjectilePacketIO().WriteResponse(om, new CreateProjectilePacketIO.PacketDataResponse
                {
                    ProjectileData = projectile
                });
                server.NetServer.SendMessage(om, connection, NetDeliveryMethod.ReliableUnordered);
            }
        }
    }
}

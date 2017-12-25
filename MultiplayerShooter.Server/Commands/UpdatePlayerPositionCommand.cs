using Lidgren.Network;
using MultiplayerShooter.Library.Networking.PacketIO;
using System.Linq;

namespace MultiplayerShooter.Server.Commands
{
    class UpdatePlayerPositionCommand : ICommand
    {
        public void Run(Server server, NetIncomingMessage im, PlayerAndConnection playerAndConnection, GameMap gameMap)
        {
            var playerData = new UpdatePlayerPositionPacketIO().ReadResponse(im).Player;
            var player = gameMap.Players.FirstOrDefault(p => p.Player.Id == playerData.Id);
            if (player != null)
            {
                player.Player = playerData;
            }
        }
    }
}

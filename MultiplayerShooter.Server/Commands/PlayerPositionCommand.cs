using Lidgren.Network;
using MultiplayerShooter.Library;
using System.Linq;
using MultiplayerShooter.Library.Networking;

namespace MultiplayerShooter.Server.Commands
{
    class PlayerPositionCommand : ICommand
    {
        public void Run(Server server, NetIncomingMessage im, PlayerAndConnection playerAndConnection, GameMap gameMap)
        {
            if (playerAndConnection != null)
            {
                var msg = server.NetServer.CreateMessage();
                msg.Write((byte)PacketType.PlayerPosition);
                msg.Write(playerAndConnection.Player.Id);
                msg.Write(playerAndConnection.Player.PositionX);
                msg.Write(playerAndConnection.Player.PositionY);
                server.NetServer.SendMessage(msg, gameMap.Players.Select(x => x.Connection).ToList(),
                    NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
    }
}

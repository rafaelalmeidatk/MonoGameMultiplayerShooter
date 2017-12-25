using Lidgren.Network;
using MultiplayerShooter.Library.Networking.PacketIO;
using System.Linq;

namespace MultiplayerShooter.Server.Commands
{
    internal class AllPlayersCommand : ICommand
    {
        public void Run(Server server, NetIncomingMessage im, PlayerAndConnection playerAndConnection, GameMap gameMap)
        {
            var msg = server.CreateMessage();
            var data = new AllPlayersPacketIO.PacketDataResponse
            {
                Players = gameMap.Players.Select(p => p.Player).ToArray()
            };
            new AllPlayersPacketIO().WriteResponse(msg, data);
            var connections = gameMap.Players.Select(x => x.Connection).ToList();
            server.NetServer.SendMessage(msg, connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}

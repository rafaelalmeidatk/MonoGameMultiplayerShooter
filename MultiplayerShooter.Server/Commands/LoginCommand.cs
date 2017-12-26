using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using MultiplayerShooter.Library;
using MultiplayerShooter.Library.Networking.PacketIO;

namespace MultiplayerShooter.Server.Commands
{
    internal class LoginCommand : ICommand
    {
        public void Run(Server server, NetIncomingMessage im, PlayerAndConnection playerAndConnection, GameMap gameMap)
        {
            Console.WriteLine("New connection attempt...");
            var data = im.ReadByte();
            if (data == (byte) PacketType.Login)
            {
                Console.WriteLine(" .. approved.");
                
                // Create the player and add it to the map
                var packetData = new LoginPacketIO().ReadRequest(im);
                playerAndConnection = CreatePlayer(im, packetData.Username, gameMap);

                // Create the hail message
                // it contains the player id and all the players already connected
                var hmsg = CreateHailMessage(server, playerAndConnection, gameMap);
                im.SenderConnection.Approve(hmsg);

                // Add the player to the map
                gameMap.AddPlayer(playerAndConnection);

                server.SendNewPlayerBroadcast(playerAndConnection);
            }
            else
            {
                im.SenderConnection.Deny("Bad packet");
            }
        }

        private NetOutgoingMessage CreateHailMessage(Server server, PlayerAndConnection playerAndConnection, GameMap gameMap)
        {
            var otherPlayers = gameMap.Players.Select(p => p.Player).ToArray();
            foreach (var player in otherPlayers)
            {
                Console.WriteLine($"{player.Id} - {player.PositionX} - {player.PositionY}");
            }
            var data = new LoginPacketIO.PacketDataResponse
            {
                Player = playerAndConnection.Player,
                OtherPlayers = otherPlayers
            };
            var hmsg = server.NetServer.CreateMessage();
            new LoginPacketIO().WriteResponse(hmsg, data);
            return hmsg;
        }

        private PlayerAndConnection CreatePlayer(NetIncomingMessage im, string username, GameMap gameMap)
        {
            var rand = new Random();
            var player = new PlayerData
            {
                Id = gameMap.GeneratePlayerId(),
                Username = username,
                PositionX = rand.Next(400),
                PositionY = 0
            };
            var playerAndConnection = new PlayerAndConnection(player, im.SenderConnection);
            return playerAndConnection;
        }
    }
}

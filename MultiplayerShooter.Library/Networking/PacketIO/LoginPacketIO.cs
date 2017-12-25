using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace MultiplayerShooter.Library.Networking.PacketIO
{
    public class LoginPacketIO : IPacketIO<LoginPacketIO.PacketDataRequest, LoginPacketIO.PacketDataResponse>
    {
        public struct PacketDataRequest
        {
            public string Username;
        }

        public struct PacketDataResponse
        {
            public PlayerData Player;
            public PlayerData[] OtherPlayers;
        }

        public void WriteRequest(NetOutgoingMessage msg, PacketDataRequest packetData)
        {
            msg.Write((byte)PacketType.Login);
            msg.Write(packetData.Username);
        }

        public PacketDataRequest ReadRequest(NetIncomingMessage msg)
        {
            return new PacketDataRequest
            {
                Username = msg.ReadString()
            };
        }

        public void WriteResponse(NetOutgoingMessage msg, PacketDataResponse packetData)
        {
            var player = packetData.Player;
            msg.WriteAllProperties(player);
            msg.Write(packetData.OtherPlayers.Length);
            foreach (var otherPlayer in packetData.OtherPlayers)
            {
                msg.WriteAllProperties(otherPlayer);
            }
        }

        public PacketDataResponse ReadResponse(NetIncomingMessage msg)
        {
            var player = new PlayerData();
            msg.ReadAllProperties(player);

            var n = msg.ReadInt32();
            var otherPlayers = new PlayerData[n];
            for (var i = 0; i < n; i++)
            {
                var otherPlayer = new PlayerData();
                msg.ReadAllProperties(otherPlayer);
                otherPlayers[i] = otherPlayer;
            }
            return new PacketDataResponse
            {
                Player = player,
                OtherPlayers = otherPlayers
            };
        }
    }
}

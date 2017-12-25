using Lidgren.Network;
using System;

namespace MultiplayerShooter.Library.Networking.PacketIO
{
    public class AllPlayersPacketIO : IPacketIO<AllPlayersPacketIO.PacketDataRequest, AllPlayersPacketIO.PacketDataResponse>
    {
        public struct PacketDataRequest
        {
        }

        public struct PacketDataResponse
        {
            public PlayerData[] Players;
        }

        public void WriteRequest(NetOutgoingMessage msg, PacketDataRequest packetData)
        {
            throw new NotImplementedException();
        }

        public PacketDataRequest ReadRequest(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        public void WriteResponse(NetOutgoingMessage msg, PacketDataResponse packetData)
        {
            msg.Write((byte)PacketType.AllPlayers);
            msg.Write(packetData.Players.Length);
            foreach (var player in packetData.Players)
            {
                msg.WriteAllProperties(player);
            }
        }

        public PacketDataResponse ReadResponse(NetIncomingMessage msg)
        {
            var n = msg.ReadInt32();
            var players = new PlayerData[n];
            for (var i = 0; i < n; i++)
            {
                var player = new PlayerData();
                msg.ReadAllProperties(player);
                players[i] = player;
            }
            return new PacketDataResponse {Players = players};
        }
    }
}

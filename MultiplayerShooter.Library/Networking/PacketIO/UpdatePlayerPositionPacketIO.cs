using Lidgren.Network;
using System;

namespace MultiplayerShooter.Library.Networking.PacketIO
{
    public class UpdatePlayerPositionPacketIO : IPacketIO<UpdatePlayerPositionPacketIO.PacketDataRequest, UpdatePlayerPositionPacketIO.PacketDataResponse>
    {
        public struct PacketDataRequest
        {
        }

        public struct PacketDataResponse
        {
            public PlayerData Player;
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
            msg.Write((byte)PacketType.UpdatePlayerPosition);
            msg.WriteAllProperties(packetData.Player);
        }

        public PacketDataResponse ReadResponse(NetIncomingMessage msg)
        {
            var player = new PlayerData();
            msg.ReadAllProperties(player);
            return new PacketDataResponse {Player = player};
        }
    }
}

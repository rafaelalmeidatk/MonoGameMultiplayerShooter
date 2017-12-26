using Lidgren.Network;
using MultiplayerShooter.Library.Projectiles;

namespace MultiplayerShooter.Library.Networking.PacketIO
{
    public class CreateProjectilePacketIO : IPacketIO<CreateProjectilePacketIO.PacketDataRequest, CreateProjectilePacketIO.PacketDataResponse>
    {
        public struct PacketDataRequest
        {
            public int LocalId;
            public ProjectileData ProjectileData;
        }

        public struct PacketDataResponse
        {
            public int LocalId;
            public ProjectileData ProjectileData;
        }

        public void WriteRequest(NetOutgoingMessage msg, PacketDataRequest packetData)
        {
            msg.Write((byte)PacketType.CreateProjectile);
            msg.Write(packetData.LocalId);
            msg.WriteAllProperties(packetData.ProjectileData);
        }

        public PacketDataRequest ReadRequest(NetIncomingMessage msg)
        {
            var localId = msg.ReadInt32();
            var projectileData = new ProjectileData();
            msg.ReadAllProperties(projectileData);
            return new PacketDataRequest {LocalId = localId, ProjectileData = projectileData};
        }

        public void WriteResponse(NetOutgoingMessage msg, PacketDataResponse packetData)
        {
            msg.Write((byte)PacketType.CreateProjectile);
            msg.Write(packetData.LocalId);
            msg.WriteAllProperties(packetData.ProjectileData);
        }

        public PacketDataResponse ReadResponse(NetIncomingMessage msg)
        {
            var projectileData = new ProjectileData();
            var localId = msg.ReadInt32();
            msg.ReadAllProperties(projectileData);
            return new PacketDataResponse {LocalId = localId, ProjectileData = projectileData};
        }
    }
}

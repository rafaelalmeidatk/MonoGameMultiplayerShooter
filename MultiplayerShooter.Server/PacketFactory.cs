using MultiplayerShooter.Library;
using MultiplayerShooter.Server.Commands;
using System;
using MultiplayerShooter.Library.Networking;

namespace MultiplayerShooter.Server
{
    internal class PacketFactory
    {
        public static ICommand GetCommand(PacketType packetType)
        {
            switch (packetType)
            {
                case PacketType.Login:
                    return new LoginCommand();
                case PacketType.PlayerPosition:
                    return new PlayerPositionCommand();
                case PacketType.UpdatePlayerPosition:
                    return new UpdatePlayerPositionCommand();
                case PacketType.CreateProjectile:
                    return new CreateProjectileCommand();
                default:
                    throw new ArgumentOutOfRangeException(nameof(packetType));
            }
        }
    }
}

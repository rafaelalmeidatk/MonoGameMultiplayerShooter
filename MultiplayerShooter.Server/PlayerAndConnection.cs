using Lidgren.Network;
using MultiplayerShooter.Library;

namespace MultiplayerShooter.Server
{
    internal class PlayerAndConnection
    {
        public PlayerData Player { get; set; }
        public NetConnection Connection { get; set; }

        public PlayerAndConnection(PlayerData player, NetConnection connection)
        {
            Player = player;
            Connection = connection;
        }
    }
}

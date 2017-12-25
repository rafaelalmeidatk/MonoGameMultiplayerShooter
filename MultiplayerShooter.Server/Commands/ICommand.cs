using Lidgren.Network;

namespace MultiplayerShooter.Server.Commands
{
    internal interface ICommand
    {
        void Run(Server server, NetIncomingMessage im, PlayerAndConnection playerAndConnection, GameMap gameMap);
    }
}

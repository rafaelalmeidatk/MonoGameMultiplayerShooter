using Lidgren.Network;
using MultiplayerShooter.Library;
using MultiplayerShooter.Library.Networking.PacketIO;
using MultiplayerShooter.Library.Projectiles;
using MultiplayerShooter.Server.Commands;
using System;
using System.Threading;
using MultiplayerShooter.Library.Networking;

namespace MultiplayerShooter.Server
{
    internal class Server
    {
        public NetServer NetServer { get; }

        // Game Map
        private GameMap _gameMap;

        public Server()
        {
            var config = new NetPeerConfiguration(GlobalConstants.APPNAME);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.Port = GlobalConstants.PORT;

            NetServer = new NetServer(config);
        }

        public void Run()
        {
            NetServer.Start();

            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = NetServer.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(msg.ReadString());
                            break;

                        case NetIncomingMessageType.ConnectionApproval:
                            var login = new LoginCommand();
                            var gameMap = GetGameMap();
                            login.Run(this, msg, null, gameMap);
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            /*
                            var status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                //Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
                                Console.WriteLine(msg.SenderConnection.RemoteUniqueIdentifier + " connected!");

                                var playerData = new PlayerData {Id = msg.SenderConnection.RemoteUniqueIdentifier};

                                // send the id back to the new player and a list of current users
                                {
                                    var om = _server.CreateMessage();
                                    om.Write((byte)PacketType.Connection);
                                    om.Write(msg.SenderConnection.RemoteUniqueIdentifier);
                                    _server.SendMessage(om, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                                    om = _server.CreateMessage();
                                    om.Write((byte)PacketType.AllPlayers);
                                    om.Write(_players.Count);
                                    foreach (var player in _players)
                                    {
                                        om.WriteAllProperties(player);
                                    }
                                    _server.SendMessage(om, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                                }

                                // send broadcast to all the players about the new player (including itself)
                                foreach (var player in _server.Connections)
                                {
                                    var om = _server.CreateMessage();
                                    om.Write((byte)PacketType.Login);
                                    om.Write(playerData.Id);
                                    _server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered);
                                }

                                // add the player to the players list
                                _players.Add(playerData);
                            }
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                var id = msg.SenderConnection.RemoteUniqueIdentifier;
                                {
                                    var player = _players.First(x => x.Id == id);
                                    if (player != null)
                                    {
                                        _players.Remove(player);
                                    }
                                }
                            }
                                */
                            break;

                        case NetIncomingMessageType.Data:
                            HandleRequest(msg);
                            break;
                    }

                    Thread.Sleep(1);
                }
            }
        }

        private void HandleRequest(NetIncomingMessage im)
        {
            var packetType = (PacketType)im.ReadByte();
            var gameMap = GetGameMap();
            var command = PacketFactory.GetCommand(packetType);
            command.Run(this, im, null, gameMap);
        }

        public NetOutgoingMessage CreateMessage()
        {
            return NetServer.CreateMessage();
        }

        public NetSendResult SendMessage(NetOutgoingMessage msg, NetConnection connection, NetDeliveryMethod method)
        {
            return NetServer.SendMessage(msg, connection, method);
        }

        public void SendNewPlayerBroadcast(PlayerAndConnection newPlayer)
        {
            foreach (var playerAndConnection in _gameMap.Players)
            {
                var connection = playerAndConnection.Connection;
                var om = NetServer.CreateMessage();
                om.Write((byte)PacketType.NewPlayer);
                om.WriteAllProperties(newPlayer.Player);

                NetServer.SendMessage(om, connection, NetDeliveryMethod.ReliableOrdered);
            }
        }

        public void SendNewProjectileBroadcast(ProjectileData projectileData)
        {
            foreach (var playerAndConnection in _gameMap.Players)
            {
                var connection = playerAndConnection.Connection;
                var om = NetServer.CreateMessage();
                new CreateProjectilePacketIO().WriteResponse(om, new CreateProjectilePacketIO.PacketDataResponse
                {
                    ProjectileData = projectileData
                });
                NetServer.SendMessage(om, connection, NetDeliveryMethod.ReliableUnordered);
            }
        }

        public void End()
        {
            NetServer.Shutdown("app exiting");
        }

        private GameMap GetGameMap()
        {
            return _gameMap ?? (_gameMap = new GameMap(this));
        }
    }
}

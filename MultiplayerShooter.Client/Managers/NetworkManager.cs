using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using MultiplayerShooter.Library;
using MultiplayerShooter.Library.Networking.PacketIO;
using MultiplayerShooter.Library.Projectiles;
using Nez;
using Random = System.Random;

namespace MultiplayerShooter.Client.Managers
{
    class NetworkManager : IUpdatableManager
    {
        //--------------------------------------------------
        // Connection

        private NetClient _client;
        private bool _triedConnection;
        private bool _firstConnected;

        //--------------------------------------------------
        // Players

        public Dictionary<long, PlayerData> Players { get; }
        
        //--------------------------------------------------
        // Player

        public PlayerData PlayerData { get; set; }

        //--------------------------------------------------
        // Events

        public Action<PlayerData> OnConnected { get; set; }
        public Action<PlayerData> OnPlayerAdded { get; set; }
        public Action<ProjectileData> OnCreateProjectile { get; set; }

        //--------------------------------------------------
        // Is Active

        public bool Active { get; private set; }

        //----------------------//------------------------//

        public NetworkManager()
        {
            Players = new Dictionary<long, PlayerData>();
        }

        public bool Start()
        {
            var config = new NetPeerConfiguration(GlobalConstants.APPNAME);
            //config.SimulatedRandomLatency = 0.2f;
            _client = new NetClient(config);
            _client.Start();

            var username = GetRandomUsername();
            var msg = _client.CreateMessage();
            new LoginPacketIO().WriteRequest(msg, new LoginPacketIO.PacketDataRequest { Username = username });
            _client.Connect(GlobalConstants.HOSTNAME, GlobalConstants.PORT, msg);
            return EstabilishConnection();
        }

        private bool EstabilishConnection()
        {
            var time = DateTime.Now;
            NetIncomingMessage im;
            while (DateTime.Now.Subtract(time).Seconds < 5)
            {
                if ((im = _client.ReadMessage()) == null) continue;
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(im.ReadString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (im.SenderConnection.Status == NetConnectionStatus.Connected)
                        {
                            Console.WriteLine("Connection accepted");

                            var hmsg = im.SenderConnection.RemoteHailMessage;
                            var data = new LoginPacketIO().ReadResponse(hmsg);

                            PlayerData = data.Player;

                            Console.WriteLine($"Our id is '{PlayerData.Id}'");

                            foreach (var player in data.OtherPlayers)
                            {
                                Players[player.Id] =
                                    new PlayerData
                                    {
                                        Id = player.Id,
                                        PositionX = player.PositionX,
                                        PositionY = player.PositionY
                                    };

                                Console.WriteLine($"Already connected player with id {player.Id}");
                                OnPlayerAdded?.Invoke(Players[player.Id]);
                            }
                            
                            OnConnected?.Invoke(PlayerData);

                            Active = true;
                            return true;
                        }
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        Console.WriteLine($"Connection approval: {im.ReadString()}");
                        break;
                    default:
                        Console.WriteLine($"not handled: {im.MessageType} | {(PacketType)im.ReadByte()}");
                        break;
                }
            }
            return false;
        }

        public void update()
        {
            if (!Active) return;

            // send the messages to the server
            
            if (PlayerData != null) // make sure we have data to send
            {
                var om = _client.CreateMessage();
                new PlayerPositionPacketIO().WriteResponse(om,
                    new PlayerPositionPacketIO.PacketDataResponse {Player = PlayerData});
                _client.SendMessage(om, NetDeliveryMethod.UnreliableSequenced);
                _client.FlushSendQueue();
            }

            NetIncomingMessage msg;
            while ((msg = _client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;
                    case NetIncomingMessageType.Data:
                        handleRequest(msg);
                        break;
                    default:
                        Console.WriteLine($"not handled: {msg.MessageType}");
                        break;
                }
            }
        }

        private void handleRequest(NetIncomingMessage im)
        {
            var packageType = (PacketType)im.ReadByte();
            switch (packageType)
            {
                case PacketType.NewPlayer:
                    Console.WriteLine("New player!");
                    var newPlayer = new NewPlayerPacketIO().ReadResponse(im);
                    OnPlayerAdded?.Invoke(newPlayer.Player);
                    break;
                case PacketType.AllPlayers:
                    var data = new AllPlayersPacketIO().ReadResponse(im);
                    foreach (var player in data.Players)
                    {
                        Players[player.Id] = player;
                    }
                    break;
                case PacketType.CreateProjectile:
                    Console.WriteLine("projectile response");
                    var respo = new CreateProjectilePacketIO().ReadResponse(im);
                    Console.WriteLine($"with id {respo.ProjectileData.Id}");
                    OnCreateProjectile?.Invoke(respo.ProjectileData);
                    break;
            }
        }

        public void CreateProjectile(ProjectileData projectileData)
        {
            var om = _client.CreateMessage();
            new CreateProjectilePacketIO().WriteRequest(om, new CreateProjectilePacketIO.PacketDataRequest
            {
                ProjectileData = projectileData
            });
            _client.SendMessage(om, NetDeliveryMethod.ReliableUnordered);
        }

        private string GetRandomUsername()
        {
            return new[]
            {
                "Robbie",
                "Paris",
                "Diego",
                "Mikel",
                "Ward",
                "Antwan",
                "Wesley",
                "Harry",
                "Damion",
                "Jeffry",
                "Rhea",
                "Margrett",
                "Jenine",
                "Lakeesha",
                "Zoe",
                "Gilma",
                "Kimi",
                "Vita",
                "Alina",
                "Valarie",
            }[new Random().Next(20)];
        }
    }
}

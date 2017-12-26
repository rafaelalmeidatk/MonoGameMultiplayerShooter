using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerShooter.Client.Components;
using MultiplayerShooter.Client.Components.Player;
using MultiplayerShooter.Client.Components.Windows;
using MultiplayerShooter.Client.Managers;
using MultiplayerShooter.Client.Systems;
using MultiplayerShooter.Library;
using MultiplayerShooter.Library.ECS.Components;
using MultiplayerShooter.Library.ECS.Components.Battle;
using MultiplayerShooter.Library.ECS.Components.Sprites;
using MultiplayerShooter.Library.Projectiles;
using Nez;
using Nez.Tiled;

namespace MultiplayerShooter.Client.Scenes
{
    class SceneMap : Scene
    {
        //--------------------------------------------------
        // Map

        private TiledMapComponent _tiledMapComponent;
        private TiledMap _tiledMap;

        //--------------------------------------------------
        // Player entities

        private Entity _player;
        private List<Entity> _playersEntities;

        //----------------------//------------------------//

        public override void initialize()
        {
            addRenderer(new DefaultRenderer());
            setupMap();
            //setupPlayer();
            //setupEnemies

            _playersEntities = new List<Entity>();
            connectToServer();
        }

        public override void onStart()
        {
            base.onStart();

            setupPlayers();
            
            setupEntityProcessors();
        }

        #region setupScene

        private void setupMap()
        {
            _tiledMap = content.Load<TiledMap>("maps/map");

            var tiledEntity = createEntity("tiled-map");
            var collisionLayer = _tiledMap.properties["collisionLayer"];
            var defaultLayers = _tiledMap.properties["defaultLayers"].Split(',').Select(s => s.Trim()).ToArray();

            _tiledMapComponent = tiledEntity.addComponent(new TiledMapComponent(_tiledMap, collisionLayer) { renderLayer = GlobalConstants.TILED_MAP_RENDER_LAYER });
            _tiledMapComponent.setLayersToRender(defaultLayers);

            if (_tiledMap.properties.ContainsKey("aboveWaterLayer"))
            {
                var aboveWaterLayer = _tiledMap.properties["aboveWaterLayer"];
                var tiledAboveWater = tiledEntity.addComponent(new TiledMapComponent(_tiledMap) { renderLayer = GlobalConstants.WATER_RENDER_LAYER });
                tiledAboveWater.setLayerToRender(aboveWaterLayer);
            }
        }

        private void setupPlayer()
        {
            var sysManager = Core.getGlobalManager<SystemManager>();

            var collisionLayer = _tiledMap.properties["collisionLayer"];
            Vector2? playerSpawn;

            if (sysManager.SpawnPosition.HasValue)
            {
                playerSpawn = sysManager.SpawnPosition;
            }
            else
            {
                playerSpawn = _tiledMap.getObjectGroup("objects").objectWithName("playerSpawn").position;
            }

            var player = createEntity("player");
            player.transform.position = playerSpawn.Value;
            player.addComponent(new TiledMapMover(_tiledMap.getLayer<TiledTileLayer>(collisionLayer)));
            player.addComponent(new BoxCollider(-16f, -16f, 32f, 32f));
            player.addComponent(new PlatformerObject(_tiledMap));
            player.addComponent<TextWindowComponent>();
            player.addComponent(new CharacterComponent());
            player.addComponent(new BattleComponent());
            var playerComponent = player.addComponent<PlayerComponent>();
            playerComponent.sprite.renderLayer = GlobalConstants.PLAYER_RENDER_LAYER;

            var box = createEntity("player");
            box.transform.position = playerSpawn.Value + 100 * Vector2.UnitX;
            box.addComponent(new TiledMapMover(_tiledMap.getLayer<TiledTileLayer>(collisionLayer)));
            box.addComponent(new BoxCollider(-16f, -16f, 32f, 32f));
            box.addComponent(new PlatformerObject(_tiledMap));
        }

        private void setupEntityProcessors()
        {
            addEntityProcessor(new BattleSystem());
            addEntityProcessor(new HitscanSystem());
            addEntityProcessor(new ProjectilesSystem(_player));
            setupCamera(_player);
        }

        private void setupCamera(Entity target)
        {
            addEntityProcessor(new CameraSystem(target)
            {
                mapLockEnabled = true,
                mapSize = new Vector2(_tiledMap.widthInPixels, _tiledMap.heightInPixels),
                followLerp = 0.08f,
                deadzoneSize = new Vector2(20, 10)
            });
        }

        #endregion

        #region setupNetwork

        private void setupPlayers()
        {
            return;
            var collisionLayer = _tiledMap.properties["collisionLayer"];

            //_playersEntities = new Entity[GlobalConstants.MAX_PLAYERS];
            for (var i = 0; i < GlobalConstants.MAX_PLAYERS; i++)
            {
                var entity = createEntity($"player-{i}");
                entity
                    /*
                    .addComponent(new TiledMapMover(_tiledMap.getLayer<TiledTileLayer>(collisionLayer)))
                    .addComponent(new PlatformerObject(_tiledMap))
                    .addComponent<BattleComponent>()
                    .addComponent<CharacterComponent>()*/
                    .addComponent(new PrototypeSprite(32, 32){color = Color.IndianRed})
                    .addComponent<NetworkComponent>();

                var collider = entity.addComponent(new BoxCollider(-16f, -16f, 32f, 32f));
                Flags.setFlagExclusive(ref collider.physicsLayer, GlobalConstants.ENEMY_LAYER);

                // disable entity
                entity.setEnabled(false);
                entity.setPosition(Nez.Random.nextInt(500), Nez.Random.nextInt(50));

                _playersEntities[i] = entity;
            }
        }

        private Entity createCharacterEntity()
        {
            var collisionLayer = _tiledMap.properties["collisionLayer"];

            var entity = createEntity("character-entity");
            entity
                .addComponent(new TiledMapMover(_tiledMap.getLayer<TiledTileLayer>(collisionLayer)))
                .addComponent(new PlatformerObject(_tiledMap))
                .addComponent<BattleComponent>()
                .addComponent<CharacterComponent>()
                .addComponent(new PrototypeSprite(32, 32) { color = Color.IndianRed })
                .addComponent<NetworkComponent>();

            // setup collider
            var collider = entity.addComponent(new BoxCollider(-16f, -16f, 32f, 32f));
            Flags.setFlagExclusive(ref collider.physicsLayer, GlobalConstants.ENEMY_LAYER);

            // setup sprite
            var texture = entity.scene.content.Load<Texture2D>(Content.Characters.placeholder);
            var sprite = entity.addComponent(new AnimatedSprite(texture, "stand"));
            sprite.CreateAnimation("stand", 0.25f);
            sprite.AddFrames("stand", new List<Rectangle>
            {
                new Rectangle(0, 0, 32, 32),
            });

            // disable entity
            entity.setEnabled(false);
            entity.setPosition(Nez.Random.nextInt(500), Nez.Random.nextInt(50));

            _playersEntities.Add(entity);

            return entity;
        }

        #endregion

        private void connectToServer()
        {
            var networkManager = Core.getGlobalManager<NetworkManager>();

            // assign events
            networkManager.OnPlayerAdded = OnPlayerAdded;
            networkManager.OnConnected = OnConnected;
            networkManager.OnCreateProjectile = OnCreateProjectile;

            // start the server
            networkManager.Start();
        }

        private void OnCreateProjectile(ProjectileData projectileData)
        {
            var shot = createEntity("projectile");
            shot.addComponent(new ProjectileComponent(projectileData, 1, 500));
            shot.transform.position = new Vector2(projectileData.PositionX, projectileData.PositionY);

            // sprite
            var texture = content.Load<Texture2D>("arrow");
            var sprite = shot.addComponent(new AnimatedSprite(texture, "default"));
            sprite.CreateAnimation("default", 0.2f);
            sprite.AddFrames("default", new List<Rectangle>
            {
                new Rectangle(0, 0, 12, 5)
            });
        }

        private void OnConnected(PlayerData playerData)
        {
            var player = createCharacterEntity();
            player.getComponent<NetworkComponent>().Id = playerData.Id; // assign our id
            player.addComponent<PlayerComponent>(); // set as player
            player.setEnabled(true);
            player.setPosition(playerData.PositionX, playerData.PositionY);

            // move the collider to the player layer
            var collider = player.getComponent<BoxCollider>();
            Flags.setFlagExclusive(ref collider.physicsLayer, GlobalConstants.PLAYER_LAYER);

            _player = player;
        }

        private void OnPlayerAdded(PlayerData playerData)
        {
            // if we already have a player with this id we just ignore it
            if (_playersEntities.Any(x => x.getComponent<NetworkComponent>().Id == playerData.Id)) return;

            Console.WriteLine($"player added with id: {playerData.Id}");

            var entity = createCharacterEntity();
            var networkComponent = entity.getComponent<NetworkComponent>();
            networkComponent.Id = playerData.Id;
            entity.setPosition(playerData.PositionX, playerData.PositionY);
            entity.setEnabled(true);
        }

        public override void update()
        {
            base.update();

            var networkManager = Core.getGlobalManager<NetworkManager>();

            // Update the position of the player on the network manager
            if (_player == null) return;
            networkManager.PlayerData.PositionX = (int) _player.position.X;
            networkManager.PlayerData.PositionY = (int) _player.position.Y;

            // Update the position of the other players
            foreach (var player in networkManager.Players)
            {
                var id = player.Key;
                Entity entity = null;
                foreach (var playersEntity in _playersEntities)
                {
                    var nc = playersEntity.getComponent<NetworkComponent>();
                    if (nc.Id == id)
                    {
                        entity = playersEntity;
                    }
                }
                if (entity != null && entity != _player)
                {
                    entity.transform.setLocalPosition(new Vector2(player.Value.PositionX, player.Value.PositionY));
                }
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using MultiplayerShooter.Client.Components;
using MultiplayerShooter.Client.Components.Battle;
using MultiplayerShooter.Client.Components.Player;
using MultiplayerShooter.Client.Components.Sprites;
using MultiplayerShooter.Client.Components.Windows;
using MultiplayerShooter.Client.Managers;
using MultiplayerShooter.Client.Systems;
using MultiplayerShooter.Library;
using Nez;
using Nez.Tiled;

namespace MultiplayerShooter.Client.Scenes
{
    class SceneMap : Scene
    {
        //--------------------------------------------------
        // Render Layers Constants

        public const int BACKGROUND_RENDER_LAYER = 10;
        public const int TILED_MAP_RENDER_LAYER = 9;
        public const int WATER_RENDER_LAYER = 6;
        public const int MISC_RENDER_LAYER = 5;
        public const int ENEMIES_RENDER_LAYER = 4;
        public const int PLAYER_RENDER_LAYER = 3;
        public const int PARTICLES_RENDER_LAYER = 2;
        public const int HUD_BACK_RENDER_LAYER = 1;
        public const int HUD_FILL_RENDER_LAYER = 0;

        //--------------------------------------------------
        // Tags

        public const int PLAYER_TAG = 1;

        //--------------------------------------------------
        // Layer Masks

        public const int PLAYER_LAYER = 1;
        public const int ENEMY_LAYER = 2;

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

            _tiledMapComponent = tiledEntity.addComponent(new TiledMapComponent(_tiledMap, collisionLayer) { renderLayer = TILED_MAP_RENDER_LAYER });
            _tiledMapComponent.setLayersToRender(defaultLayers);

            if (_tiledMap.properties.ContainsKey("aboveWaterLayer"))
            {
                var aboveWaterLayer = _tiledMap.properties["aboveWaterLayer"];
                var tiledAboveWater = tiledEntity.addComponent(new TiledMapComponent(_tiledMap) { renderLayer = WATER_RENDER_LAYER });
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
            playerComponent.sprite.renderLayer = PLAYER_RENDER_LAYER;

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
                Flags.setFlagExclusive(ref collider.physicsLayer, ENEMY_LAYER);

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

            var collider = entity.addComponent(new BoxCollider(-16f, -16f, 32f, 32f));
            Flags.setFlagExclusive(ref collider.physicsLayer, ENEMY_LAYER);

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

            // start the server
            networkManager.Start();
        }

        private void OnConnected(PlayerData playerData)
        {
            Console.WriteLine($"on connected with id: {playerData.Id}");
            // grab the first entity and set is as our player
            var player = createCharacterEntity();
            player.getComponent<NetworkComponent>().Id = playerData.Id; // assign our id
            player.addComponent<PlayerComponent>(); // set as player
            player.setEnabled(true);
            player.setPosition(playerData.PositionX, playerData.PositionY);

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
            networkManager.PlayerData.PositionY = (int)_player.position.Y;

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
﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerShooter.Client.FSM;
using MultiplayerShooter.Client.Managers;
using MultiplayerShooter.Client.Scenes;
using MultiplayerShooter.Library.ECS.Components;
using MultiplayerShooter.Library.ECS.Components.Battle;
using MultiplayerShooter.Library.ECS.Components.Sprites;
using MultiplayerShooter.Library.Projectiles;
using Nez;
using Nez.Tiled;
using System.Collections.Generic;
using MultiplayerShooter.Library;

namespace MultiplayerShooter.Client.Components.Player
{
    public class PlayerComponent : Component, IUpdatable
    {
        //--------------------------------------------------
        // Animations

        public enum Animations
        {
            Stand,
            Shot
        }

        private Dictionary<Animations, string> _animationMap;

        //--------------------------------------------------
        // Sprite

        public AnimatedSprite sprite;

        //--------------------------------------------------
        // Platformer Object

        PlatformerObject _platformerObject;
        public PlatformerObject platformerObject => _platformerObject;

        //--------------------------------------------------
        // Collision State

        public TiledMapMover.CollisionState CollisionState => _platformerObject.collisionState;

        //--------------------------------------------------
        // Velocity

        public Vector2 Velocity => _platformerObject.velocity;

        //--------------------------------------------------
        // Finite State Machine

        private FiniteStateMachine<PlayerState, PlayerComponent> _fsm;
        public FiniteStateMachine<PlayerState, PlayerComponent> FSM => _fsm;

        //--------------------------------------------------
        // Forced Movement

        private bool _forceMovement;
        private Vector2 _forceMovementVelocity;

        //--------------------------------------------------
        // Character Component

        private CharacterComponent _characterComponent;

        //----------------------//------------------------//

        public override void initialize()
        {
            _animationMap = new Dictionary<Animations, string>
            {
                {Animations.Stand, "stand"},
                {Animations.Shot, "shot"},
            };

            // init fsm
            _fsm = new FiniteStateMachine<PlayerState, PlayerComponent>(this, new StandState());
        }

        public override void onAddedToEntity()
        {
            _platformerObject = entity.getComponent<PlatformerObject>();
            _characterComponent = entity.getComponent<CharacterComponent>();

            sprite = entity.getComponent<AnimatedSprite>();

            entity.setTag(GlobalConstants.PLAYER_TAG);
        }

        public void destroyEntity()
        {
            entity.setEnabled(false);
            Core.startSceneTransition(new SquaresTransition(() => new SceneMap()));
        }

        public void forceMovement(Vector2 velocity, bool walljumpForcedMovement = false)
        {
            if (velocity == Vector2.Zero)
            {
                _forceMovement = false;
            }
            else
            {
                _forceMovement = true;
                _forceMovementVelocity = velocity;
            }
        }

        public void update()
        {
            // Update FSM
            _fsm.update();
            
            // Update input
            var axis = Core.getGlobalManager<InputManager>().MovementAxis.value;
            var velocity = _forceMovement ? _forceMovementVelocity.X : axis;
            _characterComponent.Velocity = velocity * Vector2.UnitX;
        }

        public void SetAnimation(Animations animation)
        {
            if (sprite == null) return;
            var animationStr = _animationMap[animation];
            if (sprite.CurrentAnimation != animationStr)
            {
                sprite.play(animationStr);
            }
        }

        public bool isOnGround()
        {
            return CollisionState.below;
        }

        public Entity createEntityOnMap()
        {
            return entity.scene.createEntity();
        }

        public void shoot()
        {
            var position = entity.getComponent<BoxCollider>().absolutePosition;
            var networkManager = Core.getGlobalManager<NetworkManager>();

            var projectileData = new ProjectileData
            {
                Id = 0,
                FromPlayerId = networkManager.PlayerData.Id,
                PositionX = (int)position.X,
                PositionY = (int)position.Y,
                Type = ProjectileType.Linear,
                VelocityX = 5 * (sprite.spriteEffects == SpriteEffects.FlipHorizontally ? -1 : 1),
                VelocityY = 0
            };

            var shot = entity.scene.createEntity("projectile");
            shot.addComponent(new ProjectileComponent(projectileData));
            shot.transform.position = position;

            // sprite
            var texture = entity.scene.content.Load<Texture2D>("arrow");
            var projectileSprite = entity.addComponent(new AnimatedSprite(texture, "default"));
            projectileSprite.CreateAnimation("default", 0.2f);
            projectileSprite.AddFrames("default", new List<Rectangle>
            {
                new Rectangle(0, 0, 12, 5)
            });

            networkManager.CreateProjectile(projectileData);
        }

        public void Jump()
        {
            _platformerObject.jump();
        }
    }
}

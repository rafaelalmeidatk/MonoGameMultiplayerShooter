using MultiplayerShooter.Client.Components.Battle;
using MultiplayerShooter.Client.Scenes;
using Nez;
using Nez.ECS.Components.Physics.Colliders;
using System;

namespace MultiplayerShooter.Client.Systems
{
    public class ProjectilesSystem : EntityProcessingSystem
    {
        private readonly BattleComponent _playerBattler;
        private readonly BoxCollider _playerCollider;

        public ProjectilesSystem(Entity player) : base(new Matcher().one(typeof(ProjectileComponent)))
        {
            _playerBattler = player.getComponent<BattleComponent>();
            _playerCollider = player.getComponent<BoxCollider>();
        }

        public override void process(Entity entity)
        {
            var projectileComponent = entity.getComponent<ProjectileComponent>();

            var lastPosition = entity.position;
            projectileComponent.update();
            var newPosition = entity.position;

            var flag = 1 << (projectileComponent.Data.FromPlayerId == 0
                           ? SceneMap.PLAYER_LAYER
                           : SceneMap.ENEMY_LAYER);

            var linecast = Physics.linecast(lastPosition, newPosition, flag);
            if (linecast.collider != null)
            {
                if (_playerBattler.onHit(linecast.normal * -1))
                {
                    entity.destroy();
                }
                return;
            }

            CollisionResult collisionResult;
            var collider = entity.getComponent<Collider>();

            // shots vs map
            if (collider.collidesWithAnyOfType<MapBoxCollider>(out collisionResult))
            {
                Console.WriteLine("hey");
                entity.destroy();
            }

            // shots vs player
            if (_playerBattler.isOnImmunity() || _playerCollider == null ||
                projectileComponent.Data.FromPlayerId > 0) return;
            if (collider.collidesWith(_playerCollider, out collisionResult))
            {
                if (_playerBattler.onHit(collisionResult))
                    entity.destroy();
            }
        }
    }
}

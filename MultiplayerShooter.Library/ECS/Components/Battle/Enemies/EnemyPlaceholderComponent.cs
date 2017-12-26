using MultiplayerShooter.Library.ECS.Components.Colliders;

namespace MultiplayerShooter.Library.ECS.Components.Battle.Enemies
{
    public class EnemyPlaceholderComponent : EnemyComponent
    {
        public EnemyPlaceholderComponent(bool patrolStartRight) : base(patrolStartRight)
        {
        }

        public override void initialize()
        {
            base.initialize();

            // Init sprite
            /*
            var texture = entity.scene.content.Load<Texture2D>(Content.Characters.placeholder);
            sprite = entity.addComponent(new AnimatedSprite(texture, "stand"));
            sprite.CreateAnimation("stand", 0.25f);
            sprite.AddFrames("stand", new List<Rectangle>
            {
                new Rectangle(0, 0, 32, 32),
            });
            */

            // View range
            areaOfSight = entity.addComponent(new AreaOfSightCollider(-24, -12, 92, 32));
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            // Change move speed
            platformerObject.maxMoveSpeed = 60;
            platformerObject.moveSpeed = 60;
        }
    }
}

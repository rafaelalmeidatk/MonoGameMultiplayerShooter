using System.Collections.Generic;
using MultiplayerShooter.Library.ECS.Components.Battle;
using MultiplayerShooter.Library.ECS.Components.Sprites;
using Nez;

namespace MultiplayerShooter.Client.Systems
{
    class BattleSystem : EntityProcessingSystem
    {
        public List<Entity> Entities => _entities;

        public BattleSystem() : base(new Matcher().all(typeof(BattleComponent), typeof(AnimatedSprite), typeof(BoxCollider))) { }

        public override void process(Entity entity)
        {

        }
    }
}

using System.Collections.Generic;
using MultiplayerShooter.Client.Components.Battle;
using MultiplayerShooter.Client.Components.Sprites;
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

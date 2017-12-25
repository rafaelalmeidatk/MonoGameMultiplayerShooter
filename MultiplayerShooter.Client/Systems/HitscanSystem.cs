using System;
using MultiplayerShooter.Client.Components.Battle;
using MultiplayerShooter.Client.Scenes;
using Nez;

namespace MultiplayerShooter.Client.Systems
{
    class HitscanSystem : EntityProcessingSystem
    {
        public HitscanSystem() : base(new Matcher().one(typeof(HitscanComponent))) { }

        public override void process(Entity entity)
        {
            var hitscan = entity.getComponent<HitscanComponent>();
            
            var raycastHit = Physics.linecast(hitscan.Start, hitscan.End, 1 << SceneMap.ENEMY_LAYER);
            if (raycastHit.collider != null)
            {
                Console.WriteLine("hit!");
                var battler = raycastHit.collider.entity.getComponent<BattleComponent>();
                battler?.onHit(raycastHit.normal * -1);
            }

            entity.destroy();
        }
    }
}

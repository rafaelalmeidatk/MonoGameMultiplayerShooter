using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerShooter.Library.ECS.Components.Sprites;
using MultiplayerShooter.Library.Projectiles;
using Nez;

namespace MultiplayerShooter.Library.ECS.Components.Battle
{
    public class ProjectileComponent : Component
    {
        //--------------------------------------------------
        // Sprite

        public AnimatedSprite sprite;

        //--------------------------------------------------
        // Projectile Data

        public ProjectileData Data { get; }

        //--------------------------------------------------
        // Postion

        private Vector2 _initialPosition;
        private Vector2 _currentPosition;

        //----------------------//------------------------//

        public ProjectileComponent(ProjectileData data, int direction, float speed, float rotation = 0.0f)
        {
            Data = data;
        }

        public override void initialize()
        {
            /*
            */

            var collider = entity.addComponent(new BoxCollider(-6, -2, 12, 5));
            Flags.setFlagExclusive(ref collider.physicsLayer, GlobalConstants.PROJECTILES_LAYER);

            _initialPosition = new Vector2(Data.PositionX, Data.PositionY);
        }

        public override void onAddedToEntity()
        {
            sprite = entity.getComponent<AnimatedSprite>();
            if (sprite != null)
            {
                var rotation = (float)Math.Atan2(Data.VelocityY, Data.VelocityX);
                sprite.entity.setRotation(rotation);
                sprite.renderLayer = GlobalConstants.MISC_RENDER_LAYER;
            }
            entity.setTag(GlobalConstants.PROJECTILES_TAG);
        }

        public void update()
        {
            if (Data.Type == ProjectileType.Linear)
            {
                var velx = Mathf.cos(entity.transform.rotation) * Data.VelocityX * Time.timeScale;
                var vely = Mathf.sin(entity.transform.rotation) * Data.VelocityY * Time.timeScale;
                _currentPosition += new Vector2(velx, vely);
            }
            updateEntity();
        }

        private void updateEntity()
        {
            entity.setPosition(_initialPosition + _currentPosition);
        }
    }
}

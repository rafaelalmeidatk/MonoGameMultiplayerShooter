using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerShooter.Client.Components.Sprites;
using MultiplayerShooter.Client.Scenes;
using MultiplayerShooter.Library.Projectiles;
using Nez;

namespace MultiplayerShooter.Client.Components.Battle
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
            var texture = entity.scene.content.Load<Texture2D>("arrow");
            sprite = entity.addComponent(new AnimatedSprite(texture, "default"));
            sprite.CreateAnimation("default", 0.2f);
            sprite.AddFrames("default", new List<Rectangle>
            {
                new Rectangle(0, 0, 12, 5)
            });

            var collider = entity.addComponent(new BoxCollider(-6, -2, 12, 5));
            Flags.setFlagExclusive(ref collider.physicsLayer, SceneMap.PROJECTILES_LAYER);

            _initialPosition = new Vector2(Data.PositionX, Data.PositionY);

            var rotation = (float)Math.Atan2(Data.VelocityY, Data.VelocityX);

            sprite.entity.setRotation(rotation);
            sprite.renderLayer = SceneMap.MISC_RENDER_LAYER;
        }

        public override void onAddedToEntity()
        {
            entity.setTag(SceneMap.PROJECTILES_TAG);
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

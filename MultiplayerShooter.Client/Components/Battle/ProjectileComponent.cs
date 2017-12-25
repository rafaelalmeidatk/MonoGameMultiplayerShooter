using Microsoft.Xna.Framework;
using Nez;

namespace MultiplayerShooter.Client.Components.Battle
{
    public class ProjectileComponent : Component
    {
        private Vector2 _position;

        public ProjectileComponent(Vector2 position)
        {
            _position = position;
        }
    }
}

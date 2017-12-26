using Microsoft.Xna.Framework;
using Nez;

namespace MultiplayerShooter.Library.ECS.Components.Colliders
{
    public class InteractionCollider : BoxCollider
    {
        public InteractionCollider(float x, float y, float width, float height) : base(x, y, width, height)
        { }

        public override void debugRender(Graphics graphics)
        {
            var color = Debug.Colors.colliderEdge;
            Debug.Colors.colliderEdge = Color.DarkGreen;
            base.debugRender(graphics);
            Debug.Colors.colliderEdge = color;
        }
    }
}

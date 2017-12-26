using Microsoft.Xna.Framework;
using Nez;

namespace MultiplayerShooter.Library.ECS.Components.Battle
{
    public class HitscanComponent : Component
    {
        //--------------------------------------------------
        // From enemy

        public bool FromEnemy { get; }

        //--------------------------------------------------
        // Start and End

        public Vector2 Start { get; }
        public Vector2 End { get; }

        //----------------------//------------------------//

        public HitscanComponent(Vector2 start, Vector2 end, bool fromEnemy)
        {
            Start = start;
            End = end;
            FromEnemy = fromEnemy;
        }

        public override void debugRender(Graphics graphics)
        {
            graphics.batcher.drawLine(Start, End, Color.OrangeRed);
        }
    }
}

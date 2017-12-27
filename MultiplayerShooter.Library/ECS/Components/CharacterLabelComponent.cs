using Microsoft.Xna.Framework;
using Nez;

namespace MultiplayerShooter.Library.ECS.Components
{
    public class CharacterLabelComponent : RenderableComponent
    {
        public string Label = "";

        public override float width => string.IsNullOrEmpty(Label)
            ? 0
            : Graphics.instance.bitmapFont.measureString(Label).X;

        public override float height => string.IsNullOrEmpty(Label)
            ? 0
            : Graphics.instance.bitmapFont.measureString(Label).Y;

        public override void render(Graphics graphics, Camera camera)
        {
            if (string.IsNullOrEmpty(Label)) return;
            var pos = new Vector2(width / -2, -30);
            graphics.batcher.drawString(Graphics.instance.bitmapFont, Label, entity.position + pos, Color.White);
        }
    }
}

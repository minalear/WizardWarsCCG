using System;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class Single : Control
    {
        public Card Card { get; set; }

        public Color4 HighlightedColor { get; set; }
        public Color4 HoveredColor { get; set; }
        public Color4 OutlineHighlighted { get; set; }
        public Color4 OutlineHovered { get; set; }

        public Single(Control parent, Card card)
            : base(parent)
        {
            Card = card;

            HighlightedColor = new Color4(1f, 0.65f, 0.65f, 1f);
            HoveredColor = new Color4(0.65f, 1f, 0.65f, 1f);
            OutlineHighlighted = Color4.Red;
            OutlineHovered = Color4.Green;
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            Color4 color = (Hovered) ? HoveredColor : Color4.White;
            if (Card.Highlighted) color = HighlightedColor;

            float rotation = (Card.Tapped) ? 1.571f : 0f;
            renderer.AddRenderTask(Card.Art, Position, Size, rotation, OpenTK.Vector2.Zero, color);

            //Draw outline
            if (Hovered)
            {
                Color4 outlineColor = (Card.Highlighted) ? OutlineHighlighted : OutlineHovered;
                renderer.AddOutlineTask(Card.Art, Position, Size, rotation, OpenTK.Vector2.Zero, outlineColor);
            }

            base.Draw(gameTime, renderer);
        }
    }
}

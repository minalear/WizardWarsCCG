using System;
using OpenTK;
using OpenTK.Graphics;
using Minalear;
using OpenTK.Input;

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

            if (card != null && card.Meta.IsType(Types.Creature))
            {
                CardDisplay display = new CardDisplay(this);
                display.LoadContent();
                Children.Add(display);
            }

            HighlightedColor = new Color4(1f, 0.65f, 0.65f, 1f);
            HoveredColor = new Color4(0.65f, 1f, 0.65f, 1f);
            OutlineHighlighted = Color4.Red;
            OutlineHovered = Color4.Green;
        }

        public override void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            Color4 color = (Hovered) ? HoveredColor : Color4.White;
            if (Card.Highlighted) color = HighlightedColor;

            float rotation = (Card.Tapped) ? 1.571f : 0f;
            renderer.Draw(Card.Art, Position, Size, rotation, color);

            //Draw outline
            if (Hovered)
            {
                Color4 outlineColor = (Card.Highlighted) ? OutlineHighlighted : OutlineHovered;

                renderer.DrawOutline = true;
                renderer.Draw(Card.Art, Position, Size, rotation, outlineColor);
                renderer.DrawOutline = false;
            }

            base.Draw(gameTime, renderer);
        }
    }
}

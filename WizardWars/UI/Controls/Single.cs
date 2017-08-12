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

        public Single(Card card)
        {
            Card = card;

            if (card != null && card.Meta.IsType(Types.Creature))
            {
                CardDisplay display = new CardDisplay(this);
                display.LoadContent();
                Children.Add(display);
            }
        }
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
        }

        public override void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            Color4 color = Color4.White;
            if (Hovered)
                color = Color4.Green;
            else if (Card.Highlighted)
                color = Color4.Red;

            float rotation = (Card.Tapped) ? 1.571f : 0f;
            renderer.Draw(Card.Art, Position, Size, rotation, color);

            base.Draw(gameTime, renderer);
        }
    }
}

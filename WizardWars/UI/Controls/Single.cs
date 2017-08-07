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
        public Vector2 Scale { get { return scale; } set { SetScale(value); } }

        public Single(Card card)
        {
            Card = card;
            Scale = Vector2.One;
        }
        public Single(Control parent, Card card)
            : base(parent)
        {
            Card = card;
            Scale = Vector2.One;
        }

        public override void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            Color4 color = (Hovered) ? Color4.Green : Color4.White;

            float rotation = (Card.Tapped) ? 1.571f : 0f;
            renderer.Draw(Card.Art, Position, Scale, rotation, color);
        }

        public void SetScale(Vector2 value)
        {
            scale = value;
            Size = new Vector2(Card.Art.Width, Card.Art.Height) * scale;
        }

        public override void MouseEnter(MouseMoveEventArgs e)
        {
            base.MouseEnter(e);
        }

        private Vector2 scale;
    }
}

using System;
using OpenTK.Graphics;
using Minalear;
using OpenTK;

namespace WizardWars.UI.Controls
{
    public class Single : Control
    {
        public Card Card { get; set; }

        public Color4 HighlightedColor { get; set; }
        public Color4 HoveredColor { get; set; }
        public Color4 OutlineHighlighted { get; set; }
        public Color4 OutlineHovered { get; set; }
        public bool IgnoreCardStates { get; set; }

        public Single(Control parent, Card card)
            : base(parent)
        {
            Card = card;

            if (card != null && card.Meta.IsType(Types.Creature))
            {
                Display display = new Display(this);
                display.SetText(string.Format("{0}/{1}", card.Attack, card.Defense));
                display.Alignment = System.Drawing.ContentAlignment.BottomRight;
            }

            HighlightedColor = new Color4(1f, 0.65f, 0.65f, 1f);
            HoveredColor = new Color4(0.65f, 1f, 0.65f, 1f);
            OutlineHighlighted = Color4.Red;
            OutlineHovered = Color4.Green;
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            if (Card != null)
            {
                Color4 color = (Hovered) ? HoveredColor : Color4.White;
                if (Card.Highlighted) color = HighlightedColor;

                float rotation = (Card.IsTapped && !IgnoreCardStates) ? 1.571f : 0f;
                renderer.AddRenderTask(Card.Art, Position, Size, rotation, new Vector2(0.5f, 0.5f), color);

                //If the card is face down, add a half-transparent card back texture over it
                if (Card.IsFaceDown && !IgnoreCardStates)
                    renderer.AddRenderTask(CardInfo.CardBack, Position, Size, rotation, new Vector2(0.5f, 0.5f), new Color4(1f, 1f, 1f, 0.85f), -0.1f);

                //Draw outline
                if (Hovered)
                {
                    Color4 outlineColor = (Card.Highlighted) ? OutlineHighlighted : OutlineHovered;
                    renderer.AddOutlineTask(Card.Art, Position, Size, rotation, new Vector2(0.5f, 0.5f), outlineColor);
                }
            }

            base.Draw(gameTime, renderer);
        }
        public override bool Contains(Vector2 point)
        {
            if (Card == null) return base.Contains(point);

            RectangleF rectangle = new RectangleF(Position, Size);
            if (Card.IsTapped)
            {
                //Swap the width with the height
                float width = rectangle.Width;
                rectangle.Width = rectangle.Height;
                rectangle.Height = width;

                //Offset the position
                rectangle.X += (rectangle.Height - rectangle.Width) / 2f;
            }

            return rectangle.Contains(point);
        }
    }
}

using System;
using OpenTK;
using OpenTK.Graphics;
using Minalear;
using OpenTK.Input;

namespace WizardWars.UI.Controls
{
    public class CardGroup : Control
    {
        public Player Owner { get; set; }

        public Collection Collection { get; set; }

        public bool StackSimilar { get; set; }
        public int MaxStackCount { get; set; }
        public float DrawScale { get; set; }

        private int highlightedIndex = -1;
        private Vector2 mousePos;
        
        public CardGroup(Collection collection)
        {
            Collection = collection;
            DrawScale = 1f;
            StackSimilar = false;
            MaxStackCount = 3;
        }

        public override void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            float xPos = 0f;

            for (int i = 0; i < Collection.Count; i++)
            {
                Card card = Collection[i];

                Vector2 pos = new Vector2(X + xPos, Y);
                Vector2 scale = new Vector2(DrawScale);

                xPos += CardbackArt.Width * DrawScale;
                xPos += MIN_SPACING;

                Color4 color = (i == highlightedIndex) ? Color4.Green : Color4.White;
                renderer.Draw(card.Art, pos, scale, color);
            }
        }

        public override void MouseMove(MouseMoveEventArgs e)
        {
            float xPos = 0f;

            for (int i = 0; i < Collection.Count; i++)
            {
                Card card = Collection[i];

                Vector2 pos = new Vector2(X + xPos, Y);

                if (e.X > pos.X && e.X < pos.X + CardbackArt.Width * DrawScale &&
                    e.Y > pos.Y && e.Y < pos.Y + CardbackArt.Width * DrawScale)
                {
                    highlightedIndex = i;
                    return;
                }

                xPos += CardbackArt.Width * DrawScale;
                xPos += MIN_SPACING;
            }

            highlightedIndex = -1;
        }

        private const int MAX_SPACING = 26;
        private const int MIN_SPACING = -60;
        private const int STACK_SPACING = -95;

        public static Texture2D CardbackArt;
    }
}

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
        
        public CardGroup(Collection collection)
        {
            Collection = collection;
            DrawScale = 0.39f;
            StackSimilar = false;
            MaxStackCount = 3;

            float xPos = 0f;
            for (int i = 0; i < Collection.Count; i++)
            {
                Card card = collection[i];
                Single single = new Single(card);

                Vector2 pos = new Vector2(X + xPos, Y);
                Vector2 scale = new Vector2(DrawScale);

                xPos += CardbackArt.Width * DrawScale;
                xPos += MIN_SPACING;

                single.Position = pos;
                single.Scale = scale;

                Children.Add(single);
            }
        }

        private const int MAX_SPACING = 26;
        private const int MIN_SPACING = -60;
        private const int STACK_SPACING = -95;

        public static Texture2D CardbackArt;
    }
}

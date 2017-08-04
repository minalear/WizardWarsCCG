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
        
        public CardGroup(Control parent, Collection collection)
            : base(parent)
        {
            Collection = collection;
            DrawScale = 0.39f;
            StackSimilar = false;
            MaxStackCount = 3;

            UpdateList();

            Collection.CollectionChanged += Collection_CollectionChanged;
        }
        
        public void UpdateList()
        {
            Children.Clear();

            float xPos = 0f;
            for (int i = 0; i < Collection.Count; i++)
            {
                Card card = Collection[i];
                Single single = new Single(this, card);

                Vector2 pos = new Vector2(xPos, 0f);
                Vector2 scale = new Vector2(DrawScale);

                xPos += CardbackArt.Width * DrawScale;
                xPos += MIN_SPACING;

                single.Position = pos;
                single.Scale = scale;
            }
        }

        public override void MouseMove(MouseMoveEventArgs e)
        {
            base.MouseMove(e);

            Single newCard = null;
            foreach (Single card in Children)
            {
                if (card.Hovered)
                {
                    newCard = card;
                    card.Hovered = false;
                }
            }

            if (newCard != null)
            {
                if (selectedCard == null || newCard.Card.ID != selectedCard.Card.ID)
                {
                    newCard.Hovered = true;
                    selectedCard = newCard;

                    Console.WriteLine(selectedCard.Card.ToString());
                }
                else
                {
                    newCard.Hovered = true;
                }
            }
        }
        public override void MouseUp(MouseButtonEventArgs e)
        {
            if (selectedCard != null)
            {
                CardSelected?.Invoke(this, new CardSelectionArgs(selectedCard.Card));
            }

            base.MouseUp(e);
        }

        private void Collection_CollectionChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        public delegate void CardSelectedEvent(object sender, CardSelectionArgs e);
        public event CardSelectedEvent CardSelected;

        private Single selectedCard;

        private const int MAX_SPACING = 26;
        private const int MIN_SPACING = -60;
        private const int STACK_SPACING = -95;

        public static Texture2D CardbackArt;
    }

    public class CardSelectionArgs : EventArgs
    {
        public CardSelectionArgs(Card card)
        {
            SelectedCard = card;
        }

        public Card SelectedCard { get; private set; }
    }
}

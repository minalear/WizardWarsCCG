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
        
        public CardGroup(Control parent, Vector2 pos, Vector2 size, Collection collection)
            : base(parent)
        {
            Position = pos;
            Size = size;
            StackSimilar = false;
            MaxStackCount = 3;

            Collection = collection;
            UpdateList();

            Collection.CollectionChanged += Collection_CollectionChanged;
        }
        
        public void UpdateList()
        {
            Children.Clear();

            if (!ContentLoaded)
            {
                markedForUpdate = true;
                return;
            }

            float scale = Height / CardInfo.CardBack.Height;
            float cardWidth = CardInfo.CardBack.Width * scale;

            //Auto-scale group based on the number of cards in the collection
            float collectionWidth = Collection.Count * cardWidth;
            float overlap = (Collection.Count > 1) ? (collectionWidth - Width) / (Collection.Count - 1) : 0f;
            float spacing = (cardWidth - overlap > MAX_SPACING) ? MAX_SPACING : cardWidth - overlap;

            float xPos = 0f;
            for (int i = 0; i < Collection.Count; i++)
            {
                Card card = Collection[i];
                Single single = new Single(this, card);

                single.Position = new Vector2(xPos, 0f);
                single.Size = card.Art.Size * scale;

                xPos += spacing;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (markedForUpdate)
            {
                markedForUpdate = false;
                UpdateList();
            }

            base.Update(gameTime);
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
                CardSelected?.Invoke(this, new CardSelectionArgs(selectedCard.Card, e.Button));
                selectedCard = null;
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
        private bool markedForUpdate = false;

        private const float MAX_SPACING = 120f;
    }

    public class CardSelectionArgs : EventArgs
    {
        public CardSelectionArgs(Card card, MouseButton button)
        {
            SelectedCard = card;
            Button = button;
        }

        public Card SelectedCard { get; private set; }
        public MouseButton Button { get; private set; }
    }
}

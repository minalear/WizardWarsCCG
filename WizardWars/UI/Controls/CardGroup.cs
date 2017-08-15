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
            //Clear hovered event registration
            foreach (Single single in Children)
            {
                single.ControlHovered -= Single_ControlHovered;
            }
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

                single.ControlHovered += Single_ControlHovered;
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
        public override void MouseUp(MouseButtonEventArgs e)
        {
            //This is to avoid modifying the list while Invoking CardSelected
            Card selectedCard = null; 
            foreach (Single card in Children)
            {
                if (card.Hovered)
                {
                    selectedCard = card.Card;
                    break;
                }
            }

            if (selectedCard != null)
            {
                CardSelected?.Invoke(this, new CardSelectionArgs(selectedCard, e.Button));
            }

            base.MouseUp(e);
        }
        public override void MouseMove(MouseMoveEventArgs e)
        {
            Vector2 mousePos = new Vector2(e.X, e.Y);

            //Only consider the last card hovered to be hovered, to prevent stacking issues
            foreach (Single single in Children)
            {
                single.Hovered = false;
            }

            //Loop backwards and break after finding the first hovered control
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i].Contains(mousePos))
                {
                    Children[i].Hovered = true;
                    break;
                }
            }
        }

        private void Collection_CollectionChanged(object sender, EventArgs e)
        {
            UpdateList();
        }
        private void Single_ControlHovered(object sender, MouseMoveEventArgs e)
        {
            CardHovered?.Invoke(this, new CardHoveredArgs(((Single)sender).Card, new Vector2(e.X, e.Y)));
        }

        public delegate void CardSelectedEvent(object sender, CardSelectionArgs e);
        public delegate void CardHoveredEvent(object sender, CardHoveredArgs e);

        public event CardSelectedEvent CardSelected;
        public event CardHoveredEvent CardHovered;
        
        private bool markedForUpdate = false;

        private const float MAX_SPACING = 105f;
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
    public class CardHoveredArgs : EventArgs
    {
        public CardHoveredArgs(Card card, Vector2 position)
        {
            Card = card;
            MousePosition = position;
        }

        public Card Card { get; private set; }
        public Vector2 MousePosition { get; private set; }
    }
}

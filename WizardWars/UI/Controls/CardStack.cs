using System;
using OpenTK;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class CardStack : Control
    {
        public bool IsFaceUp { get; set; }
        public Collection Collection { get; set; }

        private Display display;

        public CardStack(Control parent, Collection collection, Vector2 pos, Vector2 size)
            : base(parent)
        {
            Collection = collection;
            Collection.CollectionChanged += Collection_CollectionChanged;

            Position = pos;
            Size = size;

            display = new Display(this);
            display.SetText(string.Format("{0} ({1})", Collection.Name, Collection.Count));
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            if (IsFaceUp && Collection.Count > 0)
                renderer.AddRenderTask(Collection[0].Art, Position, Size, Color4.White);
            else if (!IsFaceUp)
                renderer.AddRenderTask(CardInfo.CardBack, Position, Size, Color4.White);

            base.Draw(gameTime, renderer);
        }

        private void Collection_CollectionChanged()
        {
            display.SetText(string.Format("{0} ({1})", Collection.Name, Collection.Count));
        }
    }
}

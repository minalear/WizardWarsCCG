using System;
using OpenTK;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class GameStack : Control
    {
        public GameStack(Control parent, Vector2 position, Vector2 size)
            : base(parent, position, size) { }

        public void AddGameStack(StateAction action)
        {
            var element = new GameStackElement(this, action, 10f);
            element.WordWrap = true;
            element.BufferWidth = (int)this.Width;

            element.LoadContent();
        }
        public void RemoveGameStack(StateAction action)
        {
            //Assume it's the last child added
            Children[Children.Count - 1].UnloadContent();
            Children.RemoveAt(Children.Count - 1);
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            //Draw only the first X-children that fit vertically
            float bufferHeight = 0f;
            float elementBuffer = 5f;
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (!(Children[i] is GameStackElement)) continue;
                Control child = Children[i];

                if (bufferHeight + child.Height <= this.Height)
                {
                    child.Y = bufferHeight;
                    child.Draw(gameTime, renderer);

                    bufferHeight += child.Height + elementBuffer;
                }
            }
        }
    }

    public class GameStackElement : TextBox
    {
        public StateAction Action;

        public GameStackElement(Control parent, StateAction action, float fontSize)
            : base(parent, action.ToString(), fontSize)
        {
            Action = action;
        }
        public override string ToString()
        {
            return string.Format("GameStackElement: ({0})", Action.ToString());
        }
    }
}

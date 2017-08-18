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
            float elementBuffer = 0f;
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

        public override void LoadContent()
        {
            GameStackElement.PhaseSymbol = Texture2D.LoadFromSource("Content/Art/Assets/phase_symbol.png");
            GameStackElement.TurnSymbol = Texture2D.LoadFromSource("Content/Art/Assets/turn_symbol.png");

            base.LoadContent();
        }
        public override void UnloadContent()
        {
            GameStackElement.PhaseSymbol.Dispose();
            GameStackElement.TurnSymbol.Dispose();

            base.UnloadContent();
        }
    }

    public class GameStackElement : Control
    {
        public StateAction Action;

        private TextBox textBox;
        private static Color4 iconBorderColor = new Color4(83, 71, 65, 255);

        public GameStackElement(Control parent, StateAction action, float fontSize)
            : base(parent)
        {
            Action = action;
            textBox = new TextBox(this, action.ToString(), fontSize);
            textBox.WordWrap = true;
            textBox.BufferWidth = (int)(parent.Width - 50f);
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            //Choose Icon texture based on current action
            if (Action is PhaseAction)
            {
                Texture2D texture = (((PhaseAction)Action).Phase == Phases.Cleanup) ? TurnSymbol : PhaseSymbol;
                renderer.AddOutlineTask(texture, new Vector2(X + 5f, Y + 5f), texture.Size, Color4.White, 0f, Vector2.Zero, iconBorderColor);
            }
            else if (Action is CardCastAction)
            {
                Texture2D texture = ((CardCastAction)Action).Card.Art;
                renderer.AddOutlineTask(texture, new Vector2(X + 5f, Y + 5f), new Vector2(40f, 40f), Color4.White, new RectangleF(71f, 51f, 142f, 142f), iconBorderColor);
            }
            else if (Action is EffectAction)
            {
                Texture2D texture = ((EffectAction)Action).Card.Art;
                renderer.AddOutlineTask(texture, new Vector2(X + 5f, Y + 5f), new Vector2(40f, 40f), Color4.White, new RectangleF(71f, 51f, 142f, 142f), iconBorderColor);
            }

            //Adjust textbox position to be offset from the icon
            textBox.X = 50f;
            textBox.Y = (int)(Height / 2f - textBox.Height / 2f);

            base.Draw(gameTime, renderer);
        }
        public override void LoadContent()
        {
            base.LoadContent();

            this.Height = Math.Max(textBox.Height, 50f);
        }

        public static Texture2D PhaseSymbol, TurnSymbol;
    }
}

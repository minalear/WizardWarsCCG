using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Minalear;
using OpenTK.Input;

namespace WizardWars.UI.Controls
{
    public class ContextMenu : Control
    {
        private Texture2D fillTexture;

        public ContextMenu(Control parent)
            : base(parent)
        {
            orderPriority = -1;

            Visible = false;
            Enabled = false;
        }

        public override void LoadContent()
        {
            fillTexture = new Texture2D(4, 4, Color4.White);
            base.LoadContent();
        }
        public override void UnloadContent()
        {
            fillTexture.Dispose();
            base.UnloadContent();
        }
        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            //Draw each fill texture for individual elements
            foreach (Control child in Children)
            {
                Color4 color = (child.Hovered) ? Color4.White : Color4.Red;
                renderer.AddRenderTask(fillTexture, child.Position, child.Size, color, -2f);
            }

            base.Draw(gameTime, renderer);
        }

        public void SetMenuOptions(string[] options)
        {
            clearChildren();

            //Set the individual element heights to the largest of the textboxes, 100px width min
            float maxWidth = 100f;
            float maxHeight = 0f;

            for (int i = 0; i < options.Length; i++)
            {
                ContextMenuItem item = new ContextMenuItem(this, options[i]);
                item.LoadContent();

                if (item.TextBox.Height > maxHeight)
                    maxHeight = item.TextBox.Height;
                if (item.TextBox.Width > maxWidth)
                    maxWidth = item.TextBox.Width;
            }

            //Set the menu's size based on the biggest element width and the height of them all together
            this.Size = new Vector2(maxWidth, maxHeight * Children.Count);

            //Update the element's position and size based on the max sizes
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Position = new Vector2(5f, i * maxHeight);
                Children[i].Size = new Vector2(maxWidth, maxHeight);
            }
        }
        public void ToggleDisplay(Vector2 position)
        {
            Position = position;
            Enabled = !Enabled;
            Visible = !Visible;
        }
        public void SetDisplay(Vector2 position, bool state)
        {
            Position = position;
            Enabled = state;
            Visible = state;
        }

        public override void MouseLeave(MouseMoveEventArgs e)
        {
            foreach (Control child in Children)
                child.Hovered = false;

            base.MouseLeave(e);
        }

        private void clearChildren()
        {
            foreach (Control child in Children)
                child.UnloadContent();
            Children.Clear();
        }
    }

    public class ContextMenuItem : Control
    {
        private TextBox textBox;

        public ContextMenuItem(Control parent, string text)
            : base(parent)
        {
            textBox = new TextBox(this, text, 10f);
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(textBox.Texture, this.Position, textBox.Size, textBox.TextColor, -3f);
        }
        
        public TextBox TextBox { get { return textBox; } }
    }
}

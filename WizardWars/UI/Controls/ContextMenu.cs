using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class ContextMenu : Control
    {
        private Texture2D fillTexture;

        public ContextMenu(Control parent)
            : base(parent)
        {
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

            bool colorSwap = true;
            foreach (Control child in Children)
            {
                Vector2 childPos = child.Position;
                Vector2 childSize = new Vector2(this.Width, child.Height);

                Color4 color = (child.Hovered) ? Color4.White : Color4.Red;

                renderer.AddRenderTask(fillTexture, childPos, childSize, color);
            }

            base.Draw(gameTime, renderer);
        }

        public void SetMenuOptions(string[] options)
        {
            clearChildren();
            this.Height = 0f;

            for (int i = 0; i < options.Length; i++)
            {
                ContextMenuItem item = new ContextMenuItem(this, options[i]);
                item.LoadContent();

                item.X = 5f;
                item.Y = i * item.Height + 4f;
                this.Height += item.Height + 4f;
                if (item.Width + 10f > this.Width)
                    this.Width = item.Width + 10f;
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

        public override void LoadContent()
        {
            base.LoadContent();
            this.Size = textBox.Size;
        }
    }
}

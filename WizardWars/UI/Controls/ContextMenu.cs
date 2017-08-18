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

            BackgroundColor = Color4.DarkGray;
            HighlightColor = Color4.LightGray;
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
                Color4 color = (child.Hovered) ? HighlightColor : BackgroundColor;
                renderer.AddRenderTask(fillTexture, child.Position, child.Size, color, -2f);
            }

            base.Draw(gameTime, renderer);
        }

        public void SetMenuOptions(params string[] options)
        {
            clearChildren();

            ContextMenuItem[] items = new ContextMenuItem[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                items[i] = new ContextMenuItem(this, options[i]);
            }

            setContextList(items);
        }
        public void SetMenuOptions(params ContextInfo[] options)
        {
            clearChildren();

            ContextMenuItem[] items = new ContextMenuItem[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                items[i] = new ContextMenuItem(this, options[i]);
            }

            setContextList(items);
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
        public override void Click(MouseButtonEventArgs e)
        {
            Vector2 mousePos = new Vector2(e.X, e.Y);
            foreach (ContextMenuItem item in Children)
            {
                if (item.Contains(mousePos))
                {
                    ItemSelected?.Invoke(this, new ContextMenuItemSelectedArgs(item));
                    SetDisplay(Vector2.Zero, false);
                    break;
                }
            }
        }

        public override void MouseLeave(MouseMoveEventArgs e)
        {
            foreach (Control child in Children)
                child.Hovered = false;

            base.MouseLeave(e);
        }

        private void setContextList(ContextMenuItem[] options)
        {
            //Set the individual element heights to the largest of the textboxes, 100px width min
            float maxWidth = 100f;
            float maxHeight = 0f;

            for (int i = 0; i < options.Length; i++)
            {
                ContextMenuItem item = options[i];
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
        private void clearChildren()
        {
            foreach (Control child in Children)
            {
                if (child.ContentLoaded)
                    child.UnloadContent();
            }
            Children.Clear();
        }

        public delegate void ItemSelectedEvent(object sender, ContextMenuItemSelectedArgs e);
        public event ItemSelectedEvent ItemSelected;

        public Color4 BackgroundColor { get; set; }
        public Color4 HighlightColor { get; set; }
    }
    
    public class ContextMenuItem : Control
    {
        private TextBox textBox;

        public ContextMenuItem(Control parent, string text)
            : base(parent)
        {
            textBox = new TextBox(this, text, 10f);
        }
        public ContextMenuItem(Control parent, ContextInfo info)
            : base(parent)
        {
            textBox = new TextBox(this, info.Text, 10f);
            Info = info;
        }

        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(textBox.Texture, this.Position, textBox.Size, textBox.TextColor, -3f);
        }
        
        public TextBox TextBox { get { return textBox; } }
        public ContextInfo Info { get; private set; }
    }
    public class ContextInfo
    {
        public ContextInfo(string text, params object[] args)
        {
            Text = text;
            Args = args;
        }

        public string Text { get; private set; }
        public object[] Args { get; private set; }
    }

    public class ContextMenuItemSelectedArgs : EventArgs
    {
        public ContextMenuItemSelectedArgs(ContextMenuItem item)
        {
            Item = item;
        }

        public ContextMenuItem Item { get; private set; }
        public ContextInfo Info { get { return Item.Info; } }
        public string Text { get { return Item.TextBox.Text; } }
    }
}

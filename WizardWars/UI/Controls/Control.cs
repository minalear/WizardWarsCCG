using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class Control
    {
        protected Vector2 relativePosition, size;
        protected Control parent;
        protected List<Control> children;

        public Control() : this(Vector2.Zero, Vector2.Zero) { }
        public Control(Vector2 position, Vector2 size)
        {
            this.relativePosition = position;
            this.size = size;

            Enabled = true;
            Visible = true;

            children = new List<Control>();
        }

        public Control(Control parent) : this(parent, Vector2.Zero, Vector2.Zero) { }
        public Control(Control parent, Vector2 position, Vector2 size)
        {
            this.relativePosition = position;
            this.size = size;

            children = new List<Control>();

            Enabled = true;
            Visible = true;

            this.parent = parent;
            this.parent.Children.Add(this);

            Screen = getParentScreen();
        }

        public bool Contains(Vector2 point)
        {
            return (point.X > Left && point.X < Right && point.Y > Top && point.Y < Bottom);
        }

        public virtual void LoadContent()
        {
            foreach (Control control in Children)
                control.LoadContent();
            ContentLoaded = true;
        }
        public virtual void UnloadContent()
        {
            foreach (Control control in Children)
                control.UnloadContent();
            ContentLoaded = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Control control in children)
            {
                if (control.Enabled)
                    control.Update(gameTime);
            }
        }
        public virtual void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            foreach (Control control in children)
            {
                if (control.Visible)
                    control.Draw(gameTime, renderer);
            }
        }

        public virtual void MouseMove(MouseMoveEventArgs e)
        {
            Vector2 mousePos = new Vector2(e.X, e.Y);
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Control control = Children[i];

                if (control.Enabled)
                {
                    bool contains = control.Contains(mousePos);
                    if (contains && !control.Hovered)
                    {
                        control.Hovered = true;
                        control.MouseEnter(e);
                    }
                    else if (!contains && control.Hovered)
                    {
                        control.Hovered = false;
                        control.MouseLeave(e);
                    }

                    control.MouseMove(e);
                }
            }
        }
        public virtual void MouseDown(MouseButtonEventArgs e)
        {
            foreach (Control control in Children)
            {
                if (control.Hovered && control.Enabled)
                    control.MouseDown(e);
            }

            MouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
        }
        public virtual void MouseUp(MouseButtonEventArgs e)
        {
            foreach (Control control in Children)
            {
                if (control.Hovered && control.Enabled)
                    control.MouseUp(e);
            }

            MouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
        }

        public virtual void MouseEnter(MouseMoveEventArgs e) { }
        public virtual void MouseLeave(MouseMoveEventArgs e) { }

        protected virtual Vector2 getAbsolutePosition()
        {
            return relativePosition + parent.Position;
        }
        
        private Screen getParentScreen()
        {
            if (this is Screen) return (Screen)this;
            return Parent.getParentScreen();
        }

        public Screen Screen { get; private set; }
        public Control Parent { get { return parent; } protected set { parent = value; } }
        public bool Hovered { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public bool ContentLoaded { get; protected set; }

        //Position/Size Info
        public Vector2 Position { get { return getAbsolutePosition(); } set { relativePosition = value; } }
        public Vector2 Size { get { return size; } set { size = value; } }
        public float X { get { return getAbsolutePosition().X; } set { relativePosition.X = value; } }
        public float Y { get { return getAbsolutePosition().Y; } set { relativePosition.Y = value; } }
        public float Width { get { return size.X; } set { size.X = value; } }
        public float Height { get { return size.Y; } set { size.Y = value; } }

        public float Left { get { return getAbsolutePosition().X; } }
        public float Top { get { return getAbsolutePosition().Y; } }
        public float Right { get { return getAbsolutePosition().X + size.X; } }
        public float Bottom { get { return getAbsolutePosition().Y + size.Y; } }

        public List<Control> Children { get { return children; } }
    }
}

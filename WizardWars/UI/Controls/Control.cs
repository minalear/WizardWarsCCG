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

            children = new List<Control>();
        }

        public Control(Control parent) : this(parent, Vector2.Zero, Vector2.Zero) { }
        public Control(Control parent, Vector2 position, Vector2 size)
        {
            this.relativePosition = position;
            this.size = size;

            children = new List<Control>();

            this.parent = parent;
            this.parent.Children.Add(this);
        }

        public bool Contains(Vector2 point)
        {
            return (point.X > Left && point.X < Right && point.Y > Top && point.Y < Bottom);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Control control in children)
                control.Update(gameTime);
        }
        public virtual void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            foreach (Control control in children)
                control.Draw(gameTime, renderer);
        }

        public virtual void MouseMove(MouseMoveEventArgs e)
        {
            Vector2 mousePos = new Vector2(e.X, e.Y);
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Control control = Children[i];

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
        public virtual void MouseDown(MouseButtonEventArgs e)
        {
            foreach (Control control in Children)
                control.MouseDown(e);
        }
        public virtual void MouseUp(MouseButtonEventArgs e)
        {
            foreach (Control control in Children)
                control.MouseUp(e);
        }

        public virtual void MouseEnter(MouseMoveEventArgs e) { }
        public virtual void MouseLeave(MouseMoveEventArgs e) { }

        protected virtual Vector2 getAbsolutePosition()
        {
            return relativePosition + parent.Position;
        }

        public Control Parent { get { return parent; } protected set { parent = value; } }
        public bool Hovered { get; set; }

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

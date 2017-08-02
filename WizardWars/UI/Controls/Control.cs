using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class Control
    {
        private Vector2 position, size;
        private List<Control> children;

        public Control()
        {
            position = Vector2.Zero;
            size = Vector2.Zero;

            children = new List<Control>();
        }
        public Control(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;

            children = new List<Control>();
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

        public virtual void MouseMove(MouseMoveEventArgs e) { }
        public virtual void MouseDown(MouseButtonEventArgs e) { }
        public virtual void MouseUp(MouseButtonEventArgs e) { }

        //Position/Size Info
        public Vector2 Position { get { return position; } set { position = value; } }
        public Vector2 Size { get { return size; } set { size = value; } }
        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }
        public float Width { get { return size.X; } set { size.X = value; } }
        public float Height { get { return size.Y; } set { size.Y = value; } }

        public float Left { get { return position.X; } }
        public float Top { get { return position.Y; } }
        public float Right { get { return position.X + size.X; } }
        public float Bottom { get { return position.Y + size.Y; } }

        public List<Control> Children { get { return children; } }
    }
}

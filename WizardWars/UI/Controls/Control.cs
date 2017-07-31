using System;
using System.Collections.Generic;
using OpenTK;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class Control
    {
        private Vector2 position;
        private List<Control> children;

        public Control()
        {
            position = Vector2.Zero;
            children = new List<Control>();
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }

        //Position Info
        public Vector2 Position { get { return position; } set { position = value; } }
        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }

        public List<Control> Children { get { return children; } }
    }
}

using System;
using OpenTK;

namespace Minalear
{
    public struct RectangleF
    {
        private Vector2 position;
        private Vector2 size;

        public RectangleF(float x, float y, float width, float height)
        {
            position = new Vector2(x, y);
            size = new Vector2(width, height);
        }
        public RectangleF(Vector2 position, Vector2 size)
        {
            this.position = position;
            this.size = size;
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }
        public bool Contains(float x, float y)
        {
            return (x > Left && x < Right && y > Top && y < Bottom); 
        }

        public void Abs()
        {
            Width = Math.Abs(Width);
            Height = Math.Abs(Height);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}) ({2}x{3})", X, Y, Width, Height);
        }

        public Vector2 Position { get { return position; } set { position = value; } }
        public Vector2 Size { get { return size; } set { size = value; } }
        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }
        public float Width { get { return size.X; } set { size.X = value; } }
        public float Height { get { return size.Y; } set { size.Y = value; } }
        public float Left { get { return position.X; } set { position.X = value; } }
        public float Right { get { return position.X + size.X; } set { position.X = value - size.X; } }
        public float Top { get { return position.Y; } set { position.Y = value; } }
        public float Bottom { get { return position.Y + size.Y; } set { position.Y = value - size.Y; } }

        public static RectangleF Empty = new RectangleF(0f, 0f, 0f, 0f);
    }
}

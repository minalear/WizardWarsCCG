using System;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class Button : TextBox
    {
        public Color4 TextColorNormal { get; set; }
        public Color4 TextColorHovered { get; set; }

        public Button(Control parent, string text)
            : base(parent, text)
        {
            TextColorNormal = Color4.Black;
            TextColorHovered = Color4.White;
        }

        public override void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            renderer.Draw(texture, Position, 1f, textColor);
        }
        public override void MouseUp(MouseButtonEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public override void MouseEnter(MouseMoveEventArgs e)
        {
            textColor = TextColorHovered;
            base.MouseEnter(e);
        }
        public override void MouseLeave(MouseMoveEventArgs e)
        {
            textColor = TextColorNormal;
            base.MouseLeave(e);
        }

        public event ButtonPressEvent Click;
        public delegate void ButtonPressEvent(object sender, MouseButtonEventArgs e);
    }
}

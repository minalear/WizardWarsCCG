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
        public Button(Control parent, string text)
            : base(parent, text) { }

        public override void MouseUp(MouseButtonEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public event ButtonPressEvent Click;
        public delegate void ButtonPressEvent(object sender, MouseButtonEventArgs e);
    }
}

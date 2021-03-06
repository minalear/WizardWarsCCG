﻿using System;
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

        public Button(Control parent, string text, float fontSize)
            : base(parent, text, fontSize)
        {
            TextColorNormal = Color4.Black;
            TextColorHovered = Color4.White;
        }

        public override void Click(MouseButtonEventArgs e)
        {
            OnClick?.Invoke(e);
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

        public event Action<MouseButtonEventArgs> OnClick;
    }
}

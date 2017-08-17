using System;
using OpenTK;
using OpenTK.Graphics;
using Minalear;
using OpenTK.Input;

namespace WizardWars.UI.Controls
{
    public class Screen : Control
    {
        private ContextMenu contextMenu;

        public Screen(Game game)
            : base(Vector2.Zero, new Vector2(game.Window.Width, game.Window.Height))
        {
            Game = game;
            contextMenu = new ContextMenu(this);
        }

        public override void MouseMove(MouseMoveEventArgs e)
        {
            Hovered = true;
            base.MouseMove(e);
        }

        protected override Vector2 getAbsolutePosition()
        {
            return this.relativePosition;
        }

        public Game Game { get; private set; }
        public ContextMenu Menu { get { return contextMenu; } }
    }
}

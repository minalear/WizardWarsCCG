using System;
using OpenTK;
using OpenTK.Graphics;
using Minalear;

namespace WizardWars.UI.Controls
{
    public class Screen : Control
    {
        private GeoRenderer renderer;

        public Screen(Game game)
            : base(Vector2.Zero, new Vector2(game.Window.Width, game.Window.Height))
        {
            renderer = new GeoRenderer(
                new Shader("Content/Shaders/geo.vert", "Content/Shaders/geo.frag"),
                game.Window.Width, game.Window.Height);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}

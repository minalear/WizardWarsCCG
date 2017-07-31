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
            renderer.Begin();
            renderer.DrawRect(new Vector2(50, 50), new Vector2(120, 75), Color4.Red);

            float r = (float)Math.Sqrt(60.0 * 60.0 + 37.5 * 37.5);
            renderer.DrawCircle(new Vector2(110, 87.5f), r, 32, Color4.Blue);
            renderer.End();
        }
    }
}

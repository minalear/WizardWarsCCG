using Minalear;
using OpenTK;
using WizardWars.UI.Controls;

namespace WizardWars.Core
{
    public class MainGame : Game
    {
        private Screen screen;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            screen = new Screen(this);

            for (int i = 0; i < 5; i++)
            {
                Vector2 position = new Vector2(
                    RNG.NextFloat(0f, Window.Width),
                    RNG.NextFloat(0f, Window.Height));
                Vector2 size = new Vector2(
                    RNG.NextFloat(50f, 250f),
                    RNG.NextFloat(50f, 250f));

                screen.Children.Add(new Control(position, size));
            }
        }

        public override void LoadContent()
        {
            GeoRenderer = new GeoRenderer(
                new Shader("Content/Shaders/geo.vert", "Content/Shaders/geo.frag"),
                Window.Width, Window.Height);
        }

        public override void Draw(GameTime gameTime)
        {
            GeoRenderer.Begin();
            screen.Draw(gameTime);
            GeoRenderer.End();
        }

        public static GeoRenderer GeoRenderer;
    }
}

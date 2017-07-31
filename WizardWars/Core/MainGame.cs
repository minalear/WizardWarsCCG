using Minalear;
using WizardWars.UI.Controls;

namespace WizardWars.Core
{
    public class MainGame : Game
    {
        private Screen screen;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            screen = new Screen(this);
        }

        public override void LoadContent()
        {

        }

        public override void Draw(GameTime gameTime)
        {
            screen.Draw(gameTime);
        }
    }
}

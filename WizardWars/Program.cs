using System;

namespace WizardWars
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (Core.MainGame game = new Core.MainGame())
            {
                game.Run();
            }
        }
    }
}

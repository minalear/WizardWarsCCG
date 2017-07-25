using System.IO;
using WizardWars;

namespace Prototyping
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }
        }
    }
}

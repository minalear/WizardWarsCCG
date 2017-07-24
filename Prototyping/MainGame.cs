using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Minalear;
using WizardWars;

namespace Prototyping
{
    public class MainGame : Game
    {
        private SpriteBatch spriteBatch;
        private Collection CardCollection;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {

        }

        public override void LoadContent()
        {
            Shader shader = new Shader();
            shader.LoadSource(ShaderType.VertexShader, System.IO.File.ReadAllText("Content/Shaders/vertex.glsl"));
            shader.LoadSource(ShaderType.FragmentShader, System.IO.File.ReadAllText("Content/Shaders/fragment.glsl"));
            shader.LinkProgram();

            spriteBatch = new SpriteBatch(shader, Window.Width, Window.Height);

            CardCollection = new Collection(null, CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json")));
            foreach (Card card in CardCollection)
            {
                card.LoadCardArt();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw Hand
            float cardWidth = CardCollection[0].Art.Width * 0.35f;
            float cardHeight = CardCollection[0].Art.Height * 0.35f;
            for (int i = 0; i < CardCollection.Count; i++)
            {
                spriteBatch.Draw(CardCollection[i].Art, new Vector2(i * cardWidth, Window.Height - cardHeight), new Vector2(0.35f), new Color4(0.7f, 1f, 0.3f, 0.5f));
            }
        }
    }
}

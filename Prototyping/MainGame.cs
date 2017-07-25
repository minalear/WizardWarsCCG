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
        private GameState gameState;
        private Card reference;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            gameState = new GameState();
        }

        public override void LoadContent()
        {
            Shader shader = new Shader();
            shader.LoadSource(ShaderType.VertexShader, System.IO.File.ReadAllText("Content/Shaders/vertex.glsl"));
            shader.LoadSource(ShaderType.FragmentShader, System.IO.File.ReadAllText("Content/Shaders/fragment.glsl"));
            shader.LinkProgram();

            spriteBatch = new SpriteBatch(shader, Window.Width, Window.Height);

            var cardList = CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json"));
            foreach (Card card in cardList)
            {
                card.LoadCardArt();
            }

            reference = cardList[0];

            gameState.PlayerOne.Deck.AddCards(cardList, Location.Random);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Random);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Random);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Random);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Random);

            gameState.PlayerOne.DrawCards(7);
        }

        public override void Draw(GameTime gameTime)
        {
            float scale = 0.25f;
            float cardWidth = reference.Art.Width * scale;
            float cardHeight = reference.Art.Height * scale;

            //Draw Field
            for (int i = 0; i < gameState.PlayerOne.Deck.Count; i++)
            {
                spriteBatch.Draw(gameState.PlayerOne.Deck[i].Art,
                    new Vector2(i * cardWidth + 10, Window.Height - cardHeight * 3),
                    new Vector2(scale), Color4.White);
            }

            //Draw Hand
            scale = 0.4f;
            cardWidth = reference.Art.Width * scale;
            cardHeight = reference.Art.Height * scale;

            float start = (Window.Width / 2.0f) - (gameState.PlayerOne.Hand.Count * cardWidth) / 2.0f;

            for (int i = 0; i < gameState.PlayerOne.Hand.Count; i++)
            {
                spriteBatch.Draw(gameState.PlayerOne.Hand[i].Art,
                    new Vector2(start + i * cardWidth, Window.Height - cardHeight),
                    new Vector2(scale), Color4.White);
            }
        }
    }
}

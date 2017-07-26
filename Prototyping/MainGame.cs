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

        private StateRenderer renderer;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            gameState = new GameState();

            renderer = new StateRenderer(this);
        }
        
        public override void LoadContent()
        {
            Shader shader = new Shader();
            shader.LoadSource(ShaderType.VertexShader, System.IO.File.ReadAllText("Content/Shaders/vertex.glsl"));
            shader.LoadSource(ShaderType.FragmentShader, System.IO.File.ReadAllText("Content/Shaders/fragment.glsl"));
            shader.LinkProgram();

            spriteBatch = new SpriteBatch(shader, Window.Width, Window.Height);

            var cardList = CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json"));
            foreach (CardInfo card in cardList)
            {
                card.LoadCardArt();

                //Add 4 copies of each card to our deck
                for (int i = 0; i < 4; i++)
                {
                    Card instance = new Card(card);
                    gameState.PlayerOne.Deck.AddCard(instance, Location.Bottom);
                }
            }
            gameState.PlayerOne.AllCards.AddCards(gameState.PlayerOne.Deck, Location.Top);

            gameState.PlayerOne.Deck.Shuffle();
            gameState.PlayerOne.DrawCards(7);

            renderer.SetGameState(gameState);
        }

        public override void Draw(GameTime gameTime)
        {
            renderer.Draw(gameTime, spriteBatch);
        }
        public override void Update(GameTime gameTime)
        {
            renderer.Update(gameTime);
        }
    }
}

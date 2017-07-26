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
        private Vector2 mousePos;
        private bool mouseUp = false;

        private StateRenderer renderer;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            gameState = new GameState();
            Window.MouseUp += Window_MouseUp;
            Window.MouseMove += Window_MouseMove;

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
            foreach (Card card in cardList)
            {
                card.LoadCardArt();
            }

            reference = cardList[0];

            gameState.PlayerOne.Field.AddCard(cardList[0]);

            gameState.PlayerOne.Deck.AddCards(cardList, Location.Top);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Top);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Top);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Top);
            gameState.PlayerOne.Deck.AddCards(cardList, Location.Top);

            //gameState.PlayerOne.Deck.Shuffle();

            gameState.PlayerOne.AllCards.AddCards(gameState.PlayerOne.Deck, Location.Top);

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

        private void Window_MouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            mouseUp = true;
        }
        private void Window_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            mousePos = new Vector2(e.X, e.Y);
        }
    }
}

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

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            gameState = new GameState();
            Window.MouseUp += Window_MouseUp;
            Window.MouseMove += Window_MouseMove;
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
            for (int i = 0; i < gameState.PlayerOne.Field.Count; i++)
            {
                bool highlight = false;

                Vector2 pos = new Vector2(i * cardWidth + 10, Window.Height - cardHeight * 3);
                if (mousePos.X >= pos.X && mousePos.X <= pos.X + cardWidth && mousePos.Y >= pos.Y && mousePos.Y <= pos.Y + cardHeight)
                {
                    highlight = true;
                }

                Color4 color = (highlight) ? Color4.Green : Color4.White;

                spriteBatch.Draw(gameState.PlayerOne.Field[i].Art, pos, new Vector2(scale), color);
            }

            //Draw Hand
            scale = 0.4f;
            cardWidth = reference.Art.Width * scale;
            cardHeight = reference.Art.Height * scale;

            float start = (Window.Width / 2.0f) - (gameState.PlayerOne.Hand.Count * cardWidth) / 2.0f;

            for (int i = 0; i < gameState.PlayerOne.Hand.Count; i++)
            {
                bool highlight = false;

                Vector2 pos = new Vector2(start + i * cardWidth, Window.Height - cardHeight);
                if (mousePos.X >= pos.X && mousePos.X <= pos.X + cardWidth && mousePos.Y >= pos.Y && mousePos.Y <= pos.Y + cardHeight)
                {
                    highlight = true;

                    if (mouseUp)
                    {
                        mouseUp = false;
                        Card card = gameState.PlayerOne.Hand.RemoveCard(i);
                        gameState.PlayCard(gameState.PlayerOne, card);

                        i--;

                        continue;
                    }
                }

                Color4 color = (highlight) ? Color4.Green : Color4.White;

                spriteBatch.Draw(gameState.PlayerOne.Hand[i].Art, pos, new Vector2(scale), color);
            }

            mouseUp = false;
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

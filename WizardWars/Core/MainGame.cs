using System.Collections.Generic;
using Minalear;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using WizardWars.UI.Controls;

namespace WizardWars.Core
{
    public class MainGame : Game
    {
        private TextureRenderer renderer;
        private Texture2D playFieldTexture;

        private GameState gameState;

        private Screen screen;
        private CardGroup playerOneDeck;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            gameState = new GameState();

            Window.MouseMove += (sender, e) => screen.MouseMove(e);
            Window.MouseUp += (sender, e) => screen.MouseUp(e);
            Window.MouseDown += (sender, e) => screen.MouseDown(e);
        }

        public override void LoadContent()
        {
            renderer = new TextureRenderer(
                new Shader("Content/Shaders/tex.vert", "Content/Shaders/tex.frag"),
                Window.Width, Window.Height);

            playFieldTexture = Texture2D.LoadFromSource("Content/Art/Playfield.png");
            CardGroup.CardbackArt = Texture2D.LoadFromSource("Content/Art/Cardback.png");

            var cardList = CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json"));
            foreach (var card in cardList)
            {
                card.LoadCardArt();

                for (int i = 0; i < 8; i++)
                {
                    Card instance = new Card(card);
                    gameState.PlayerOne.Deck.AddCard(instance, Location.Random);
                }
            }

            gameState.PlayerOne.DrawCards(7);

            screen = new Screen(this);

            playerOneDeck = new CardGroup(gameState.PlayerOne.Hand);
            playerOneDeck.DrawScale = 0.39f;
            playerOneDeck.Position = new Vector2(193, 555);
            playerOneDeck.Size = new Vector2(927, 150);

            screen.Children.Add(playerOneDeck);
        }

        public override void Draw(GameTime gameTime)
        {
            renderer.Draw(playFieldTexture, Vector2.Zero, Vector2.One, Color4.White);
            screen.Draw(gameTime, renderer);
        }
    }
}
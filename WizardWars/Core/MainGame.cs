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
        private CardGroup playerOneHand, playerOneField;

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

            playerOneHand = new CardGroup(screen, gameState.PlayerOne.Hand);
            playerOneHand.DrawScale = 0.39f;
            playerOneHand.Position = new Vector2(193f, 555f);
            playerOneHand.Size = new Vector2(927f, 150f);

            playerOneHand.CardSelected += (sender, e) =>
            {
                playerOneHand.Collection.RemoveCardID(e.SelectedCard.ID);
                gameState.PlayerOne.Field.AddCard(e.SelectedCard, Location.Bottom);
            };

            playerOneField = new CardGroup(screen, gameState.PlayerOne.Field);
            playerOneField.DrawScale = 0.45f;
            playerOneField.Position = new Vector2(193f, 360f);
            playerOneField.Size = new Vector2(927f, 190f);
        }

        public override void Draw(GameTime gameTime)
        {
            renderer.Draw(playFieldTexture, Vector2.Zero, Vector2.One, Color4.White);
            screen.Draw(gameTime, renderer);
        }

        public override void Update(GameTime gameTime)
        {
            screen.Update(gameTime);
        }
    }
}
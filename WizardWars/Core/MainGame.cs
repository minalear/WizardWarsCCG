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
        private TextBox promptText;
        private CardGroup playerOneField, playerOneElysium, playerOneHand;

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

            #region Load Card Data
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
            #endregion

            gameState.PlayerOne.DrawCards(7);

            screen = new Screen(this);

            playerOneField =   new CardGroup(screen, new Vector2(204, 348), new Vector2(945, 114), 0.282f, gameState.PlayerOne.Field);
            playerOneElysium = new CardGroup(screen, new Vector2(204, 472), new Vector2(945, 114), 0.282f, gameState.PlayerOne.Elysium);
            playerOneHand =    new CardGroup(screen, new Vector2(204, 596), new Vector2(945, 114), 0.282f, gameState.PlayerOne.Hand);

            playerOneHand.CardSelected += (sender, e) =>
            {
                if (e.Button == MouseButton.Right)
                {
                    gameState.PlayerOne.Elysium.AddCard(gameState.PlayerOne.Hand.RemoveCardID(e.SelectedCard.ID));
                }
                else if (gameState.CanCastCard(gameState.PlayerOne, e.SelectedCard))
                {
                    gameState.StageCard(e.SelectedCard);
                }
            };
            playerOneField.CardSelected += (sender, e) =>
            {
                /*if (gameState.IsCasting)
                {
                    gameState.SubmitTarget(gameState.PlayerOne, new Target(e.SelectedCard, Target.Zones.Field));
                }*/
            };
            playerOneElysium.CardSelected += (sender, e) =>
            {
                e.SelectedCard.Tapped = !e.SelectedCard.Tapped;
            };

            gameState.PlayerOne.PromptPayCastingCost += (sender, e) =>
            {
                promptText.SetText(string.Format("Pay ({0}) to cast {1}.", e.Cost, e.Card));
            };

            promptText = new TextBox(screen, "Test Text");
            promptText.Position = new Vector2(209f, 254f);

            screen.LoadContent();
        }
        public override void UnloadContent()
        {
            screen.UnloadContent();
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

        public GameState GameState { get { return gameState; } }
    }
}
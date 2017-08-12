﻿using Minalear;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using WizardWars.UI.Controls;
using WizardWars.UI.Screens;

namespace WizardWars.Core
{
    public class MainGame : Game
    {
        private TextureRenderer renderer;

        private GameState gameState;

        private Screen screen;
        private TextBox promptText;
        private Button continueButton;
        private CardGroup playerOneField, playerOneElysium, playerOneHand;
        
        private Duel duelScreen;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            Window.WindowBorder = WindowBorder.Fixed;

            Window.MouseMove += (sender, e) => screen.MouseMove(e);
            Window.MouseUp += (sender, e) => screen.MouseUp(e);
            Window.MouseDown += (sender, e) => screen.MouseDown(e);
        }

        public override void Initialize()
        {
            gameState = new GameState();
            screen = new Screen(this);

            duelScreen = new Duel(this, gameState);

            playerOneField = new CardGroup(screen, new Vector2(204, 348), new Vector2(945, 114), gameState.PlayerOne.Field);
            playerOneElysium = new CardGroup(screen, new Vector2(204, 472), new Vector2(945, 114), gameState.PlayerOne.Elysium);
            playerOneHand = new CardGroup(screen, new Vector2(204, 596), new Vector2(945, 114), gameState.PlayerOne.Hand);

            playerOneHand.CardSelected += (sender, e) =>
            {
                if (gameState.CanCastCard(gameState.PlayerOne, e.SelectedCard))
                {
                    gameState.PlayerOne.Hand.RemoveCardID(e.SelectedCard.ID);
                    gameState.AddStateAction(new CardCastAction(e.SelectedCard, gameState.PlayerOne));
                    gameState.ContinueGame();
                }
            };
            playerOneField.CardSelected += (sender, e) =>
            {
                if (gameState.RequiresTarget && e.SelectedCard.Highlighted)
                {
                    gameState.SubmitTargets(e.SelectedCard);
                }
                else if (gameState.CurrentPhase == Phases.DeclareAttack)
                {
                    if (e.SelectedCard.Meta.IsType(Types.Creature))
                    {
                        e.SelectedCard.Tapped = !e.SelectedCard.Tapped;
                        e.SelectedCard.Attacking = e.SelectedCard.Tapped;
                    }
                }
            };
            playerOneElysium.CardSelected += (sender, e) =>
            {

            };

            promptText = new TextBox(screen, "Test Text");
            promptText.Position = new Vector2(209f, 254f);

            gameState.PhaseChange += (sender, e) =>
            {
                promptText.Text = string.Format("Phase: {0}", e);
            };

            continueButton = new Button(screen, "OK");
            continueButton.Position = new Vector2(83, 550);

            continueButton.Click += (sender, e) =>
            {
                if (gameState.HasPriority(gameState.PlayerOne))
                {
                    System.Console.WriteLine("Player #{0}: Passing on Action ({1})", gameState.PlayerOne.ID + 1, gameState.CurrentAction);
                    gameState.PassPriority();
                }
            };
        }
        public override void LoadContent()
        {
            renderer = new TextureRenderer(
                new Shader("Content/Shaders/tex.vert", "Content/Shaders/tex.frag"),
                Window.Width, Window.Height);

            CardInfo.CardBack = Texture2D.LoadFromSource("Content/Art/cardback.png");

            var cardList = CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json"));
            var deckList = CardFactory.LoadDeckFile(gameState.PlayerOne, "Content/Decks/test_deck.dek", cardList);
            var oppoList = CardFactory.LoadDeckFile(gameState.PlayerTwo, "Content/Decks/opponent_deck.dek", cardList);

            gameState.AllCards.AddRange(deckList);
            gameState.PlayerOne.Deck.AddCards(deckList, Location.Random);
            gameState.PlayerTwo.Deck.AddCards(oppoList, Location.Random);

            screen.LoadContent();
            duelScreen.LoadContent();

            gameState.StartGame();
        }
        
        public override void UnloadContent()
        {
            duelScreen.UnloadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            duelScreen.Draw(gameTime, renderer);
        }
        public override void Update(GameTime gameTime)
        {
            duelScreen.Update(gameTime);
        }

        public GameState GameState { get { return gameState; } }
    }
}
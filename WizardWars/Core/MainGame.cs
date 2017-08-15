using Minalear;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using WizardWars.UI.Controls;
using WizardWars.UI.Screens;

namespace WizardWars.Core
{
    public class MainGame : Game
    {
        private RenderEngine renderEngine;

        private GameState gameState;
        private AI gameAI;
        
        private CardGroup playerOneField;
        
        private Duel duelScreen;

        public MainGame() : base(1280, 720, "Wizard Wars CCG")
        {
            Window.WindowBorder = WindowBorder.Fixed;

            Window.MouseMove += (sender, e) => duelScreen.MouseMove(e);
            Window.MouseUp += (sender, e) => duelScreen.MouseUp(e);
            Window.MouseDown += (sender, e) => duelScreen.MouseDown(e);
        }

        public override void Initialize()
        {
            gameState = new GameState();
            gameAI = new AI(gameState.PlayerTwo);
            duelScreen = new Duel(this, gameState);

            return;

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
                        e.SelectedCard.IsTapped = !e.SelectedCard.IsTapped;
                        e.SelectedCard.Attacking = e.SelectedCard.IsTapped;
                    }
                }
            };
        }
        public override void LoadContent()
        {
            renderEngine = new RenderEngine(this, 2);

            CardInfo.CardBack = Texture2D.LoadFromSource("Content/Art/Assets/cardback.png");

            var cardList = CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json"));
            var deckList = CardFactory.LoadDeckFile(gameState.PlayerOne, "Content/Decks/test_deck.dek", cardList);
            var oppoList = CardFactory.LoadDeckFile(gameState.PlayerTwo, "Content/Decks/opponent_deck.dek", cardList);

            gameState.AllCards.AddRange(deckList);
            gameState.PlayerOne.Deck.AddCards(deckList, Location.Random);
            gameState.PlayerTwo.Deck.AddCards(oppoList, Location.Random);
            
            duelScreen.LoadContent();

            gameState.StartGame();
        }
        
        public override void UnloadContent()
        {
            duelScreen.UnloadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            duelScreen.Draw(gameTime, renderEngine);

            renderEngine.ProcessRenderCalls();
        }
        public override void Update(GameTime gameTime)
        {
            duelScreen.Update(gameTime);
            gameAI.Update(gameTime);
        }

        public GameState GameState { get { return gameState; } }
    }
}
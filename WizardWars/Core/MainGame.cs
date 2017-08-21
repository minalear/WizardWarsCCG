using Minalear;
using OpenTK;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using WizardWars.UI.Screens;

namespace WizardWars.Core
{
    public class MainGame : Game
    {
        private RenderEngine renderEngine;
        private ScriptEngine scriptEngine;

        private GameState gameState;
        private AI gameAI;
        
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
        }
        public override void LoadContent()
        {
            renderEngine = new RenderEngine(this, 2);
            scriptEngine = Python.CreateEngine();
            scriptEngine.Runtime.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(CardInfo)));
            scriptEngine.Runtime.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Types)));
            scriptEngine.Runtime.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(SubTypes)));

            CardInfo.CardBack = Texture2D.LoadFromSource("Content/Art/Assets/cardback.png");

            CardFactory.LoadCards(scriptEngine);
            var cardList = CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json"));
            var deckList = CardFactory.LoadDeckFile(gameState.PlayerOne, "Content/Decks/test_deck.dek", cardList);
            var oppoList = CardFactory.LoadDeckFile(gameState.PlayerTwo, "Content/Decks/opponent_deck.dek", cardList);

            gameState.AllCards.AddRange(deckList);
            gameState.AllCards.AddRange(oppoList);
            gameState.AllCards.Add(gameState.PlayerOne.PlayerCard);
            gameState.AllCards.Add(gameState.PlayerTwo.PlayerCard);

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
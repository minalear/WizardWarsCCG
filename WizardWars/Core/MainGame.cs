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
        private AI gameAI;

        private Screen screen;
        private TextBox promptText;
        private Button continueButton, cancelButton;
        private CardGroup playerOneField, playerOneElysium, playerOneHand;

        private Single castingCard;

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
            gameAI = new AI(gameState.PlayerTwo);
            screen = new Screen(this);

            playerOneField = new CardGroup(screen, new Vector2(204, 348), new Vector2(945, 114), 0.282f, gameState.PlayerOne.Field);
            playerOneElysium = new CardGroup(screen, new Vector2(204, 472), new Vector2(945, 114), 0.282f, gameState.PlayerOne.Elysium);
            playerOneHand = new CardGroup(screen, new Vector2(204, 596), new Vector2(945, 114), 0.282f, gameState.PlayerOne.Hand);

            castingCard = new Single(screen, null);
            castingCard.Position = new Vector2(10, 10);
            castingCard.Scale = new Vector2(0.6f, 0.6f);

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
            
            playFieldTexture = Texture2D.LoadFromSource("Content/Art/Playfield.png");
            CardGroup.CardbackArt = Texture2D.LoadFromSource("Content/Art/Cardback.png");

            var cardList = CardFactory.LoadCards(System.IO.File.ReadAllText("Content/cards.json"));
            var deckList = CardFactory.LoadDeckFile(gameState.PlayerOne, "Content/Decks/test_deck.dek", cardList);
            var oppoList = CardFactory.LoadDeckFile(gameState.PlayerTwo, "Content/Decks/opponent_deck.dek", cardList);

            gameState.AllCards.AddRange(deckList);
            gameState.PlayerOne.Deck.AddCards(deckList, Location.Random);
            gameState.PlayerTwo.Deck.AddCards(oppoList, Location.Random);

            gameState.PlayerOne.PlayerCard = cardList[0].CreateInstance(gameState.PlayerOne);
            gameState.PlayerTwo.PlayerCard = cardList[0].CreateInstance(gameState.PlayerTwo);

            screen.LoadContent();
            castingCard.Card = gameState.PlayerOne.Deck[0];

            gameState.StartGame();
        }
        
        public override void UnloadContent()
        {
            screen.UnloadContent();
            playFieldTexture.Dispose();
        }
        public override void Draw(GameTime gameTime)
        {
            renderer.Draw(playFieldTexture, Vector2.Zero, Vector2.One, Color4.White);
            screen.Draw(gameTime, renderer);
        }
        public override void Update(GameTime gameTime)
        {
            screen.Update(gameTime);
            gameAI.Update();

            if (GameState.PlayerOne.Deck.Count == 0)
                castingCard.Visible = false;
            else if (castingCard.Card.ID != gameState.PlayerOne.Deck[0].ID)
                castingCard.Card = gameState.PlayerOne.Deck[0];
        }

        public GameState GameState { get { return gameState; } }
    }
}
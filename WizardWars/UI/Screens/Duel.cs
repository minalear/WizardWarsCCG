using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using Minalear;
using WizardWars.UI.Controls;

namespace WizardWars.UI.Screens
{
    public class Duel : Screen
    {
        private GameState gameState;
        private Texture2D playfieldTexture;

        //Player One
        private CardStack playerOneDeck;
        private CardStack playerOneGraveyard;
        private CardGroup playerOneHand;
        private CardGroup playerOneElysium;
        private CardGroup playerOneBattlefield;

        private Controls.Single playerOneHero;
        private Display playerOneHealthDisplay;

        //Player Two
        private CardStack playerTwoDeck;
        private CardStack playerTwoGraveyard;
        private CardGroup playerTwoElysium;
        private CardGroup playerTwoBattlefield;

        private Controls.Single playerTwoHero;
        private Display playerTwoHealthDisplay;
        private HandCounter playerTwoHandCounter;

        //Other
        private GameStack gameStackControl;
        private PhaseTracker phaseTracker;
        private Controls.Single previewCard;
        private TextBox promptBox;
        private Button continueButton;
        private Button endTurnButton;

        //Mechanics
        private Card castedCard;
        private bool isCasting = false;
        private int costPaid = 0;

        public Duel(Game game, GameState gameState) : base(game)
        {
            this.gameState = gameState;
            this.gameState.ActionResolved += GameState_ActionResolved;
            Vector2 cardSize = new Vector2(100, 126);
            Vector2 zoneSize = new Vector2(854, 126);

            /* PLAYER ONE */
            playerOneHero = new Controls.Single(this, gameState.PlayerOne.PlayerCard);
            playerOneHero.Position = new Vector2(196, 584);
            playerOneHero.Size = cardSize;

            playerOneHealthDisplay = new Display(playerOneHero);
            playerOneHealthDisplay.Text = "HP: 20";
            playerOneHealthDisplay.Alignment = System.Drawing.ContentAlignment.BottomRight;

            playerOneDeck = new CardStack(this, gameState.PlayerOne.Deck, new Vector2(1170, 584), cardSize);
            playerOneGraveyard = new CardStack(this, gameState.PlayerOne.Graveyard, new Vector2(1170, 448), cardSize);
            playerOneGraveyard.IsFaceUp = true;

            playerOneHand = new CardGroup(this, new Vector2(306, 584), zoneSize, gameState.PlayerOne.Hand);
            playerOneElysium = new CardGroup(this, new Vector2(306, 448), zoneSize, gameState.PlayerOne.Elysium);
            playerOneBattlefield = new CardGroup(this, new Vector2(306, 312), zoneSize, gameState.PlayerOne.Field);

            playerOneHand.CardSelected += PlayerOneHand_CardSelected;
            playerOneHand.CardHovered += CardHovered;
            playerOneElysium.CardSelected += PlayerOneElysium_CardSelected;
            playerOneElysium.CardHovered += CardHovered;
            playerOneBattlefield.CardSelected += PlayerOneBattlefield_CardSelected;
            playerOneBattlefield.CardHovered += CardHovered;

            /* PLAYER TWO */
            playerTwoHero = new Controls.Single(this, gameState.PlayerTwo.PlayerCard);
            playerTwoHero.Position = new Vector2(196, 10);
            playerTwoHero.Size = cardSize;

            playerTwoHealthDisplay = new Display(playerTwoHero);
            playerTwoHealthDisplay.Text = "HP: 20";
            playerTwoHealthDisplay.Alignment = System.Drawing.ContentAlignment.BottomRight;

            playerTwoDeck = new CardStack(this, gameState.PlayerTwo.Deck, new Vector2(1170, 10), cardSize);
            playerTwoGraveyard = new CardStack(this, gameState.PlayerTwo.Graveyard, new Vector2(1170, 146), cardSize);
            playerTwoGraveyard.IsFaceUp = true;

            playerTwoElysium = new CardGroup(this, new Vector2(306, 10), zoneSize, gameState.PlayerTwo.Elysium);
            playerTwoBattlefield = new CardGroup(this, new Vector2(306, 146), zoneSize, gameState.PlayerTwo.Field);

            playerTwoHandCounter = new HandCounter(this, "Content/Art/Assets/hand_count_symbol.png", new Vector2(206f, 12f));
            gameState.PlayerTwo.Hand.CollectionChanged += (sender, e) =>
            {
                playerTwoHandCounter.Text = gameState.PlayerTwo.Hand.Count.ToString();
            };

            /* OTHER */
            gameStackControl = new GameStack(this, new Vector2(10f, 10f), new Vector2(175f, 470f));
            gameState.NewStateAction += (sender, e) => gameStackControl.AddGameStack(e);

            phaseTracker = new PhaseTracker(this, new Vector2(304f, 280f), gameState);

            previewCard = new Controls.Single(this, null);
            previewCard.Position = new Vector2(10f, 490f);
            previewCard.Size = new Vector2(175f, 220f);
            previewCard.IgnoreTappedState = true;

            promptBox = new TextBox(this, string.Empty, 12f);
            promptBox.Position = new Vector2(1175f, 287f);

            continueButton = new Button(this, "Continue", 12f);
            continueButton.Position = new Vector2(1170, 388);
            continueButton.Click += ContinueButton_Click;

            endTurnButton = new Button(this, "End Turn", 12f);
            endTurnButton.Position = new Vector2(1170, 418);
            endTurnButton.Click += EndTurnButton_Click;
        }

        public override void LoadContent()
        {
            playfieldTexture = Texture2D.LoadFromSource("Content/Art/Assets/playfield.png");

            playerOneHero.Card = gameState.PlayerOne.PlayerCard;
            playerTwoHero.Card = gameState.PlayerTwo.PlayerCard;

            playerOneHealthDisplay.Text = string.Format("HP: {0}", gameState.PlayerOne.Heatlh);
            playerTwoHealthDisplay.Text = string.Format("HP: {0}", gameState.PlayerTwo.Heatlh);

            base.LoadContent();
        }
        public override void UnloadContent()
        {
            playfieldTexture.Dispose();

            base.UnloadContent();
        }
        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(playfieldTexture, Vector2.Zero, Color4.White);

            base.Draw(gameTime, renderer);
        }

        /* EVENTS */
        private void GameState_ActionResolved(object sender, StateAction action)
        {
            gameStackControl.RemoveGameStack(action);
        }

        private void PlayerOneHand_CardSelected(object sender, CardSelectionArgs e)
        {
            //Normal action
            if (e.Button == MouseButton.Left)
            {
                if (!isCasting && gameState.CanCastCard(gameState.PlayerOne, e.SelectedCard))
                {
                    isCasting = true;
                    castedCard = gameState.PlayerOne.Hand.RemoveCardID(e.SelectedCard.ID);
                    gameState.PlayerOne.PromptPlayerPayCastingCost(castedCard, castedCard.Cost);

                    promptBox.Text = string.Format("Pay ({0}).", castedCard.Cost);
                    endTurnButton.Text = "Cancel";
                }
            }
            //Advanced action
            else if (e.Button == MouseButton.Right)
            {
                //Eventually have a popup box signifying you want to devote, and have other options if they're available
                if (gameState.PlayerOne.TryDevoteCard(e.SelectedCard))
                {
                    gameState.PlayerOne.Hand.RemoveCardID(e.SelectedCard.ID);
                }
            }
        }
        private void PlayerOneElysium_CardSelected(object sender, CardSelectionArgs e)
        {
            //Can't get mana out of Mana Drained Elysium cards
            if (!e.SelectedCard.IsManaDrained)
            {
                //You may untap cards if they're not drained
                e.SelectedCard.IsTapped = !e.SelectedCard.IsTapped;
                int mod = (e.SelectedCard.IsTapped) ? 1 : -1;

                //If the player is casting a spell, add the mana directly to the cost paid, otherwise add it to their mana pool
                if (isCasting)
                {
                    costPaid += e.SelectedCard.Meta.ManaValue * mod;
                    promptBox.Text = string.Format("Pay ({0}).", castedCard.Cost - costPaid);
                }
                else
                {
                    gameState.PlayerOne.Mana += e.SelectedCard.Meta.ManaValue * mod;
                }
            }
        }
        private void PlayerOneBattlefield_CardSelected(object sender, CardSelectionArgs e)
        {

        }

        private void CardHovered(object sender, CardHoveredArgs e)
        {
            previewCard.Card = e.Card;
        }

        private void ContinueButton_Click(object sender, MouseButtonEventArgs e)
        {
            //Continue state
            if (!isCasting && gameState.HasPriority(gameState.PlayerOne))
            {
                gameState.PassPriority();
            }
            else if (isCasting)
            {
                //Count the mana both intentionally paid towards the spell and the mana in the mana pool
                if (costPaid + gameState.PlayerOne.Mana >= castedCard.Cost)
                {
                    //Drain the mana required from the mana pool to cover the cost
                    int manaDrain = castedCard.Cost - costPaid;
                    gameState.PlayerOne.Mana -= manaDrain;
                    costPaid = 0;

                    //set IsManaDrained true to every card tapped in Elysium (redundant)
                    foreach (Card card in gameState.PlayerOne.Elysium)
                    {
                        if (card.IsTapped)
                            card.IsManaDrained = true;
                    }

                    //Finally cast the card and update the UI
                    isCasting = false;
                    gameState.AddStateAction(new CardCastAction(castedCard, gameState.PlayerOne));
                    gameState.ContinueGame();
                    endTurnButton.Text = "End Turn";
                    promptBox.Text = string.Empty;
                }
            }
        }
        private void EndTurnButton_Click(object sender, MouseButtonEventArgs e)
        {
            //Acts as the cancel button
            if (isCasting)
            {
                endTurnButton.Text = "End Turn";
                promptBox.Text = string.Empty;
                gameState.PlayerOne.Hand.AddCard(castedCard);
                castedCard = null;
                costPaid = 0;

                isCasting = false;
            }
        }
    }
}

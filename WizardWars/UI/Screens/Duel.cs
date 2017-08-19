using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using Minalear;
using WizardWars.UI.Controls;

namespace WizardWars.UI.Screens
{
    public class Duel : Screen
    {
        #region Private Members
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
        #endregion

        public Duel(Game game, GameState gameState) : base(game)
        {
            this.gameState = gameState;
            this.gameState.ActionResolved += GameState_ActionResolved;
            Vector2 cardSize = new Vector2(100, 126);
            Vector2 zoneSize = new Vector2(854, 126);

            /* PLAYER ONE */
            playerOneHero = new Single(this, gameState.PlayerOne.PlayerCard);
            playerOneHero.Position = new Vector2(196, 584);
            playerOneHero.Size = cardSize;

            playerOneHealthDisplay = new Display(playerOneHero);
            playerOneHealthDisplay.Text = "HP: 20";
            playerOneHealthDisplay.Alignment = System.Drawing.ContentAlignment.BottomRight;
            gameState.PlayerOne.HealthChanged += (sender, e) => playerOneHealthDisplay.SetText(string.Format("HP: {0}", e));

            playerOneDeck = new CardStack(this, gameState.PlayerOne.Deck, new Vector2(1170, 584), cardSize);
            playerOneGraveyard = new CardStack(this, gameState.PlayerOne.Graveyard, new Vector2(1170, 448), cardSize);
            playerOneGraveyard.IsFaceUp = true;

            playerOneHand = new CardGroup(this, new Vector2(306, 584), zoneSize, gameState.PlayerOne.Hand);
            playerOneElysium = new CardGroup(this, new Vector2(306, 448), zoneSize, gameState.PlayerOne.Elysium);
            playerOneBattlefield = new CardGroup(this, new Vector2(306, 312), zoneSize, gameState.PlayerOne.Field);

            playerOneHero.Clicked += PlayerOneHero_Clicked;
            playerOneHand.CardSelected += PlayerOneHand_CardSelected;
            playerOneHand.CardContextSelected += PlayerOneHand_CardContextSelected;
            playerOneHand.CardHovered += CardHovered;
            playerOneElysium.CardSelected += PlayerOneElysium_CardSelected;
            playerOneElysium.CardContextSelected += PlayerOneElysium_CardContextSelected;
            playerOneElysium.CardHovered += CardHovered;
            playerOneBattlefield.CardSelected += PlayerOneBattlefield_CardSelected;
            playerOneBattlefield.CardHovered += CardHovered;

            /* PLAYER TWO */
            playerTwoHero = new Single(this, gameState.PlayerTwo.PlayerCard);
            playerTwoHero.Position = new Vector2(196, 10);
            playerTwoHero.Size = cardSize;
            playerTwoHero.Clicked += PlayerTwoHero_Clicked;

            playerTwoHealthDisplay = new Display(playerTwoHero);
            playerTwoHealthDisplay.Text = "HP: 20";
            playerTwoHealthDisplay.Alignment = System.Drawing.ContentAlignment.BottomRight;
            gameState.PlayerTwo.HealthChanged += (sender, e) => playerTwoHealthDisplay.SetText(string.Format("HP: {0}", e));

            playerTwoDeck = new CardStack(this, gameState.PlayerTwo.Deck, new Vector2(1170, 10), cardSize);
            playerTwoGraveyard = new CardStack(this, gameState.PlayerTwo.Graveyard, new Vector2(1170, 146), cardSize);
            playerTwoGraveyard.IsFaceUp = true;

            playerTwoElysium = new CardGroup(this, new Vector2(306, 10), zoneSize, gameState.PlayerTwo.Elysium);
            playerTwoBattlefield = new CardGroup(this, new Vector2(306, 146), zoneSize, gameState.PlayerTwo.Field);

            playerTwoBattlefield.CardSelected += PlayerTwoBattlefield_CardSelected;

            playerTwoHandCounter = new HandCounter(this, "Content/Art/Assets/hand_count_symbol.png", new Vector2(206f, 12f));
            gameState.PlayerTwo.Hand.CollectionChanged += (sender, e) =>
            {
                playerTwoHandCounter.Text = gameState.PlayerTwo.Hand.Count.ToString();
            };

            /* OTHER */
            gameStackControl = new GameStack(this, new Vector2(10f, 10f), new Vector2(175f, 470f));
            gameState.NewStateAction += (sender, e) => gameStackControl.AddGameStack(e);

            phaseTracker = new PhaseTracker(this, new Vector2(304f, 280f), gameState);

            previewCard = new Single(this, null);
            previewCard.Position = new Vector2(10f, 490f);
            previewCard.Size = new Vector2(175f, 220f);
            previewCard.IgnoreCardStates = true;

            promptBox = new TextBox(this, string.Empty, 12f);
            promptBox.Position = new Vector2(1175f, 287f);
            gameState.PlayerOne.Prompt += (sender, e) => promptBox.SetText(e);

            continueButton = new Button(this, "Continue", 12f);
            continueButton.Position = new Vector2(1170, 388);
            continueButton.OnClick += ContinueButton_Click;

            endTurnButton = new Button(this, "End Turn", 12f);
            endTurnButton.Position = new Vector2(1170, 418);
            endTurnButton.OnClick += EndTurnButton_Click;

            Menu.ItemSelected += Menu_ItemSelected;
        }

        public override void LoadContent()
        {
            playfieldTexture = Texture2D.LoadFromSource("Content/Art/Assets/playfield.png");

            playerOneHero.Card = gameState.PlayerOne.PlayerCard;
            playerTwoHero.Card = gameState.PlayerTwo.PlayerCard;

            playerOneHealthDisplay.Text = string.Format("HP: {0}", gameState.PlayerOne.Heatlh);
            playerTwoHealthDisplay.Text = string.Format("HP: {0}", gameState.PlayerTwo.Heatlh);

            Menu.SetMenuOptions(new string[] { "Test Item #1", "Test Item #2", "Test Item #3" });

            base.LoadContent();
        }
        public override void UnloadContent()
        {
            playfieldTexture.Dispose();

            base.UnloadContent();
        }
        public override void Draw(GameTime gameTime, RenderEngine renderer)
        {
            renderer.AddRenderTask(playfieldTexture, Vector2.Zero, Color4.White, 1f);

            base.Draw(gameTime, renderer);
        }

        public override void MouseUp(MouseButtonEventArgs e)
        {
            //Disable context menu if it's not relevant
            if (e.Button == MouseButton.Left && !Menu.Hovered)
            {
                Menu.SetDisplay(Vector2.Zero, false);
            }

            base.MouseUp(e);
        }

        private void attemptCastCard(Card card)
        {
            if (!isCasting && gameState.CanCastCard(gameState.PlayerOne, card))
            {
                isCasting = true;
                castedCard = gameState.PlayerOne.Hand.RemoveCardID(card.ID);
                gameState.PlayerOne.PromptPlayerPayCastingCost(castedCard, castedCard.Cost);

                promptBox.Text = string.Format("Pay ({0}).", castedCard.Cost);
                endTurnButton.Text = "Cancel";
            }
        }
        private void attemptDevoteCard(Card card, bool facedown)
        {
            if (gameState.PlayerOne.TryDevoteCard(card))
            {
                card.IsFaceDown = facedown;
                gameState.PlayerOne.Hand.RemoveCardID(card.ID);
            }
        }

        /* EVENTS */
        private void GameState_ActionResolved(object sender, StateAction action)
        {
            gameStackControl.RemoveGameStack(action);
        }

        //Player One zones
        private void PlayerOneHero_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (gameState.RequiresTarget)
            {
                gameState.SubmitTarget(gameState.PlayerOne.PlayerCard);
            }
        }
        private void PlayerOneHand_CardSelected(object sender, CardSelectionArgs e)
        {
            //Normal action
            if (e.Button == MouseButton.Left)
            {
                attemptCastCard(e.SelectedCard);
            }
        }
        private void PlayerOneHand_CardContextSelected(object sender, CardSelectionArgs e)
        {
            Menu.SetDisplay(e.MousePosition, true);

            ContextInfo cast = new ContextInfo("Cast", ContextInfoTypes.Cast, e.SelectedCard);
            ContextInfo devoteUp = new ContextInfo("Devote - Face Up", ContextInfoTypes.DevoteUp, e.SelectedCard);
            ContextInfo devoteDown = new ContextInfo("Devote - Face Down", ContextInfoTypes.DevoteDown, e.SelectedCard);

            Menu.SetMenuOptions(cast, devoteUp, devoteDown);
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
                    costPaid += e.SelectedCard.GetManaValue() * mod;
                    promptBox.Text = string.Format("Pay ({0}).", castedCard.Cost - costPaid);
                }
                else
                {
                    gameState.PlayerOne.Mana += e.SelectedCard.Meta.ManaValue * mod;
                }
            }
        }
        private void PlayerOneElysium_CardContextSelected(object sender, CardSelectionArgs e)
        {
            Menu.SetDisplay(e.MousePosition, true);

            List<ContextInfo> contextOptions = new List<ContextInfo>();
            if (e.SelectedCard.IsFaceDown)
                contextOptions.Add(new ContextInfo("Turn Face Up", ContextInfoTypes.ElysiumTurnFaceUp, e.SelectedCard));

            Menu.SetMenuOptions(contextOptions.ToArray());
        }
        private void PlayerOneBattlefield_CardSelected(object sender, CardSelectionArgs e)
        {
            if (gameState.RequiresTarget)
            {
                gameState.SubmitTarget(e.SelectedCard);
            }
        }

        //Player Two zones
        private void PlayerTwoHero_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (gameState.RequiresTarget)
            {
                gameState.SubmitTarget(gameState.PlayerTwo.PlayerCard);
            }
        }
        private void PlayerTwoBattlefield_CardSelected(object sender, CardSelectionArgs e)
        {
            if (gameState.RequiresTarget)
            {
                gameState.SubmitTarget(e.SelectedCard);
            }
        }

        private void CardHovered(object sender, CardHoveredArgs e)
        {
            previewCard.Card = e.Card;
        }
        private void Menu_ItemSelected(object sender, ContextMenuItemSelectedArgs e)
        {
            //Hand Options
            if (e.Type == ContextInfoTypes.Cast)
            {
                Card card = (Card)e.Info.Args[0];
                attemptCastCard(card);
            }
            else if (e.Type == ContextInfoTypes.DevoteUp)
            {
                Card card = (Card)e.Info.Args[0];
                attemptDevoteCard(card, false);
            }
            else if (e.Type == ContextInfoTypes.DevoteDown)
            {
                Card card = (Card)e.Info.Args[0];
                attemptDevoteCard(card, true);
            }

            //Elysium Options
            if (e.Type == ContextInfoTypes.ElysiumTurnFaceUp)
            {
                gameState.PlayerOne.TryTurnElysiumCardUp((Card)e.Info.Args[0]);
            }
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

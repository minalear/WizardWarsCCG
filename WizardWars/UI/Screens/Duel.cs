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

        private Single playerOneHero;
        private Display playerOneHealthDisplay;

        //Player Two
        private CardStack playerTwoDeck;
        private CardStack playerTwoGraveyard;
        private CardGroup playerTwoElysium;
        private CardGroup playerTwoBattlefield;

        private Single playerTwoHero;
        private Display playerTwoHealthDisplay;
        private HandCounter playerTwoHandCounter;

        //Other
        private GameStack gameStackControl;
        private PhaseTracker phaseTracker;
        private Single previewCard;
        private TextBox promptBox;
        private Button continueButton;
        private Button endTurnButton;

        //Mechanic
        CastingController castingController;
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
            gameState.PlayerOne.HealthChanged += (e) => playerOneHealthDisplay.SetText(string.Format("HP: {0}", e));

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
            playerOneBattlefield.CardContextSelected += PlayerOneBattlefield_CardContextSelected;

            /* PLAYER TWO */
            playerTwoHero = new Single(this, gameState.PlayerTwo.PlayerCard);
            playerTwoHero.Position = new Vector2(196, 10);
            playerTwoHero.Size = cardSize;
            playerTwoHero.Clicked += PlayerTwoHero_Clicked;

            playerTwoHealthDisplay = new Display(playerTwoHero);
            playerTwoHealthDisplay.Text = "HP: 20";
            playerTwoHealthDisplay.Alignment = System.Drawing.ContentAlignment.BottomRight;
            gameState.PlayerTwo.HealthChanged += (e) => playerTwoHealthDisplay.SetText(string.Format("HP: {0}", e));

            playerTwoDeck = new CardStack(this, gameState.PlayerTwo.Deck, new Vector2(1170, 10), cardSize);
            playerTwoGraveyard = new CardStack(this, gameState.PlayerTwo.Graveyard, new Vector2(1170, 146), cardSize);
            playerTwoGraveyard.IsFaceUp = true;

            playerTwoElysium = new CardGroup(this, new Vector2(306, 10), zoneSize, gameState.PlayerTwo.Elysium);
            playerTwoBattlefield = new CardGroup(this, new Vector2(306, 146), zoneSize, gameState.PlayerTwo.Field);

            playerTwoBattlefield.CardSelected += PlayerTwoBattlefield_CardSelected;

            playerTwoHandCounter = new HandCounter(this, "Content/Art/Assets/hand_count_symbol.png", new Vector2(206f, 12f));
            gameState.PlayerTwo.Hand.CollectionChanged += () =>
            {
                playerTwoHandCounter.Text = gameState.PlayerTwo.Hand.Count.ToString();
            };

            /* OTHER */
            gameStackControl = new GameStack(this, new Vector2(10f, 10f), new Vector2(175f, 470f));
            gameState.NewStateAction += (e) => gameStackControl.AddGameStack(e);

            phaseTracker = new PhaseTracker(this, new Vector2(304f, 280f), gameState);

            previewCard = new Single(this, null);
            previewCard.Position = new Vector2(10f, 490f);
            previewCard.Size = new Vector2(175f, 220f);
            previewCard.IgnoreCardStates = true;

            promptBox = new TextBox(this, string.Empty, 12f);
            promptBox.Position = new Vector2(1175f, 287f);
            gameState.PlayerOne.Prompt += (e) => promptBox.SetText(e);

            continueButton = new Button(this, "Continue", 12f);
            continueButton.Position = new Vector2(1170, 388);
            continueButton.OnClick += ContinueButton_Click;

            endTurnButton = new Button(this, "End Turn", 12f);
            endTurnButton.Position = new Vector2(1170, 418);
            endTurnButton.OnClick += EndTurnButton_Click;

            Menu.ItemSelected += Menu_ItemSelected;

            castingController = new CastingController(gameState, gameState.PlayerOne);
            castingController.CostPaid += CastingController_CostPaid;
            castingController.Canceled += CastingController_Canceled;
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
            if (castingController.CastingState == CastingState.None && gameState.CanCastCard(gameState.PlayerOne, card))
            {
                Card castedCard = gameState.PlayerOne.Hand.RemoveCardID(card.ID);
                gameState.PlayerOne.PromptPlayerPayCastingCost(castedCard, castedCard.Cost);

                promptBox.Text = string.Format("Pay ({0}).", castedCard.Cost);
                endTurnButton.Text = "Cancel";

                castingController.StageCard(castedCard);
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
        private void attemptActivateAbility(Card card, Ability ability)
        {
            if (castingController.CastingState == CastingState.None)
            {
                gameState.PlayerOne.PromptPlayerPayCastingCost(card, ability.Cost);

                promptBox.Text = string.Format("Pay ({0}).", ability.Cost);
                endTurnButton.Text = "Cancel";

                castingController.StageAbility(card, ability);
            }
        }
        private void CastingController_CostPaid(CastingEventArgs obj)
        {
            //Add the refunded mana to your mana pool
            gameState.PlayerOne.Mana += obj.Refund;

            if (castingController.CastingState == CastingState.CardCast)
            {
                gameState.AddStateAction(new CardCastAction(obj.Card, gameState.PlayerOne));
                gameState.ContinueGame();
                endTurnButton.Text = "End Turn";
                promptBox.Text = string.Empty;
            }
            else if (castingController.CastingState == CastingState.Ability)
            {
                gameState.AddStateAction(new AbilityAction(obj.Card, gameState.PlayerOne, obj.Ability));
                gameState.ContinueGame();
                endTurnButton.Text = "End Turn";
                promptBox.Text = string.Empty;
            }
        }

        /* EVENTS */
        private void GameState_ActionResolved(StateAction action)
        {
            gameStackControl.RemoveGameStack(action);
        }

        //Player One zones
        private void PlayerOneHero_Clicked(MouseButtonEventArgs e)
        {
            if (castingController.CastingState != CastingState.None && gameState.RequiresTarget)
            {
                gameState.SubmitTarget(gameState.PlayerOne.PlayerCard);
            }
            else if (e.Button == MouseButton.Right)
            {
                List<ContextInfo> contextOptions = new List<ContextInfo>();
                foreach (Ability ability in gameState.PlayerOne.PlayerCard.Abilities)
                {
                    contextOptions.Add(new ContextInfo(ability.Name, ContextInfoTypes.Ability, gameState.PlayerOne.PlayerCard, ability));
                }
                Menu.SetMenuOptions(contextOptions.ToArray());
                Menu.SetDisplay(new Vector2(e.X, e.Y), true);
            }
        }
        private void PlayerOneHand_CardSelected(CardSelectionArgs e)
        {
            //Normal action
            if (e.Button == MouseButton.Left)
            {
                attemptCastCard(e.SelectedCard);
            }
        }
        private void PlayerOneHand_CardContextSelected(CardSelectionArgs e)
        {
            Menu.SetDisplay(e.MousePosition, true);

            ContextInfo cast = new ContextInfo("Cast", ContextInfoTypes.Cast, e.SelectedCard);
            ContextInfo devoteUp = new ContextInfo("Devote - Face Up", ContextInfoTypes.DevoteUp, e.SelectedCard);
            ContextInfo devoteDown = new ContextInfo("Devote - Face Down", ContextInfoTypes.DevoteDown, e.SelectedCard);

            Menu.SetMenuOptions(cast, devoteUp, devoteDown);
        }
        private void PlayerOneElysium_CardSelected(CardSelectionArgs e)
        {
            //Can't get mana out of Mana Drained Elysium cards
            if (!e.SelectedCard.IsManaDrained)
            {
                //You may untap cards if they're not drained
                e.SelectedCard.IsTapped = !e.SelectedCard.IsTapped;
                int mod = (e.SelectedCard.IsTapped) ? 1 : -1;

                //If the player is casting a spell, add the mana directly to the cost paid, otherwise add it to their mana pool
                if (castingController.CastingState != CastingState.None)
                {
                    castingController.PayMana(e.SelectedCard.ManaValue * mod);
                    promptBox.Text = string.Format("Pay ({0}).", castingController.RemainingManaCost);
                }
                else
                {
                    gameState.PlayerOne.Mana += e.SelectedCard.ManaValue * mod;
                }
            }
        }
        private void PlayerOneElysium_CardContextSelected(CardSelectionArgs e)
        {
            Menu.SetDisplay(e.MousePosition, true);

            List<ContextInfo> contextOptions = new List<ContextInfo>();
            if (e.SelectedCard.IsFaceDown)
                contextOptions.Add(new ContextInfo("Turn Face Up", ContextInfoTypes.ElysiumTurnFaceUp, e.SelectedCard));

            Menu.SetMenuOptions(contextOptions.ToArray());
        }
        private void PlayerOneBattlefield_CardSelected(CardSelectionArgs e)
        {
            if (gameState.RequiresTarget)
            {
                gameState.SubmitTarget(e.SelectedCard);
            }
        }
        private void PlayerOneBattlefield_CardContextSelected(CardSelectionArgs e)
        {

        }

        //Player Two zones
        private void PlayerTwoHero_Clicked(MouseButtonEventArgs e)
        {
            if (gameState.RequiresTarget)
            {
                gameState.SubmitTarget(gameState.PlayerTwo.PlayerCard);
            }
        }
        private void PlayerTwoBattlefield_CardSelected(CardSelectionArgs e)
        {
            if (gameState.RequiresTarget)
            {
                gameState.SubmitTarget(e.SelectedCard);
            }
        }

        private void CardHovered(CardHoveredArgs e)
        {
            previewCard.Card = e.Card;
        }
        private void Menu_ItemSelected(ContextMenuItemSelectedArgs e)
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

            //Battlefield Options
            if (e.Type == ContextInfoTypes.Ability)
            {
                attemptActivateAbility((Card)e.Info.Args[0], (Ability)e.Info.Args[1]);
            }
        }

        private void ContinueButton_Click(MouseButtonEventArgs e)
        {
            //Continue state
            if (castingController.CastingState == CastingState.None && gameState.HasPriority(gameState.PlayerOne))
            {
                gameState.PassPriority();
            }
            else if (castingController.CastingState != CastingState.None)
            {
                //Dump all of the mana we currently have into the castingController
                castingController.PayMana(gameState.PlayerOne.Mana);
                gameState.PlayerOne.Mana = 0;
            }
        }
        private void EndTurnButton_Click(MouseButtonEventArgs e)
        {
            //Acts as the cancel button
            if (castingController.CastingState != CastingState.None)
            {
                endTurnButton.Text = "End Turn";
                promptBox.Text = string.Empty;

                castingController.Cancel();
            }
        }

        private void CastingController_Canceled(CastingEventArgs obj)
        {
            if (castingController.CastingState == CastingState.CardCast)
                gameState.PlayerOne.Hand.AddCard(obj.Card);

            //Untap all lands used to cast the casting card
            foreach (Card card in gameState.PlayerOne.Elysium)
            {
                if (card.IsTapped && !card.IsManaDrained)
                    card.IsTapped = false;
            }
        }
    }
}

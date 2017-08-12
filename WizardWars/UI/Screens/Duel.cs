using System;
using OpenTK;
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

        //Player Two
        private CardStack playerTwoDeck;
        private CardStack playerTwoGraveyard;
        private CardGroup playerTwoElysium;
        private CardGroup playerTwoBattlefield;

        private Controls.Single playerTwoHero;

        //Other
        private Button continueButton;
        private Button endTurnButton;

        //Mechanics
        private Card castedCard;

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

            playerOneDeck = new CardStack(this, gameState.PlayerOne.Deck, new Vector2(1170, 584), cardSize);
            playerOneGraveyard = new CardStack(this, gameState.PlayerOne.Graveyard, new Vector2(1170, 448), cardSize);
            playerOneGraveyard.IsFaceUp = true;

            playerOneHand = new CardGroup(this, new Vector2(306, 584), zoneSize, gameState.PlayerOne.Hand);
            playerOneElysium = new CardGroup(this, new Vector2(306, 448), zoneSize, gameState.PlayerOne.Elysium);
            playerOneBattlefield = new CardGroup(this, new Vector2(306, 312), zoneSize, gameState.PlayerOne.Field);

            playerOneHand.CardSelected += PlayerOneHand_CardSelected;

            /* PLAYER TWO */
            playerTwoHero = new Controls.Single(this, gameState.PlayerTwo.PlayerCard);
            playerTwoHero.Position = new Vector2(196, 10);
            playerTwoHero.Size = cardSize;

            playerTwoDeck = new CardStack(this, gameState.PlayerTwo.Deck, new Vector2(1170, 10), cardSize);
            playerTwoGraveyard = new CardStack(this, gameState.PlayerTwo.Graveyard, new Vector2(1170, 146), cardSize);
            playerTwoGraveyard.IsFaceUp = true;

            playerTwoElysium = new CardGroup(this, new Vector2(306, 10), zoneSize, gameState.PlayerTwo.Elysium);
            playerTwoBattlefield = new CardGroup(this, new Vector2(306, 146), zoneSize, gameState.PlayerTwo.Field);

            /* CONTINUE */
            continueButton = new Button(this, "Continue");
            continueButton.Position = new Vector2(1170, 388);
            continueButton.Click += (sender, e) =>
            {
                if (gameState.HasPriority(gameState.PlayerOne))
                {
                    Console.WriteLine("Player #{0}: Passing on Action ({1})", gameState.PlayerOne.ID + 1, gameState.CurrentAction);
                    gameState.PassPriority();
                }
            };

            endTurnButton = new Button(this, "End Turn");
            endTurnButton.Position = new Vector2(1170, 418);
        }

        private void GameState_ActionResolved(object sender, StateAction action)
        {
            if (action is CardCastAction)
            {
                //Card resolved, so send it to the graveyard
                CardCastAction castAction = (CardCastAction)action;
                if (castedCard != null && castAction.Card.ID == castedCard.ID)
                {
                    castedCard.Owner.Graveyard.AddCard(castedCard);
                    castedCard = null;
                }
            }
        }
        private void PlayerOneHand_CardSelected(object sender, CardSelectionArgs e)
        {
            if (gameState.CanCastCard(gameState.PlayerOne, e.SelectedCard))
            {
                castedCard = gameState.PlayerOne.Hand.RemoveCardID(e.SelectedCard.ID);
                gameState.AddStateAction(new CardCastAction(e.SelectedCard, gameState.PlayerOne));
                gameState.ContinueGame();
            }
        }

        public override void LoadContent()
        {
            playfieldTexture = Texture2D.LoadFromSource("Content/Art/playfield.png");

            playerOneHero.Card = gameState.PlayerOne.PlayerCard;
            playerTwoHero.Card = gameState.PlayerTwo.PlayerCard;

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime, TextureRenderer renderer)
        {
            renderer.Draw(playfieldTexture, Vector2.Zero, 1f, Color4.White);

            base.Draw(gameTime, renderer);
        }
    }
}

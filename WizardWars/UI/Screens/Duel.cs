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

        public Duel(Game game, GameState gameState) : base(game)
        {
            this.gameState = gameState; 
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

            endTurnButton = new Button(this, "End Turn");
            endTurnButton.Position = new Vector2(1170, 418);
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

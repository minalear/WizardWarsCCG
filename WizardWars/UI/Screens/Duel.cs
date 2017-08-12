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

        private Controls.Single playerOneHero;

        //Player Two
        private CardStack playerTwoDeck;
        private CardStack playerTwoGraveyard;

        private Controls.Single playerTwoHero;

        public Duel(Game game, GameState gameState) : base(game)
        {
            this.gameState = gameState; 
            Vector2 cardSize = new Vector2(100, 126);

            /* PLAYER ONE */
            playerOneHero = new Controls.Single(this, gameState.PlayerOne.PlayerCard);
            playerOneHero.Position = new Vector2(196, 584);
            playerOneHero.Size = cardSize;

            playerOneDeck = new CardStack(this, gameState.PlayerOne.Deck, new Vector2(1170, 584), cardSize);
            playerOneGraveyard = new CardStack(this, gameState.PlayerOne.Graveyard, new Vector2(1170, 448), cardSize);
            playerOneGraveyard.IsFaceUp = true;
            playerOneHand = new CardGroup(this, new Vector2(306, 584), new Vector2(854, 126), gameState.PlayerOne.Hand);

            /* PLAYER TWO */
            playerTwoHero = new Controls.Single(this, gameState.PlayerTwo.PlayerCard);
            playerTwoHero.Position = new Vector2(196, 10);
            playerTwoHero.Size = cardSize;

            playerTwoDeck = new CardStack(this, gameState.PlayerTwo.Deck, new Vector2(1170, 10), cardSize);
            playerTwoGraveyard = new CardStack(this, gameState.PlayerTwo.Graveyard, new Vector2(1170, 146), cardSize);
            playerTwoGraveyard.IsFaceUp = true;
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

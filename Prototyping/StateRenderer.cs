using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using Minalear;
using WizardWars;

namespace Prototyping
{
    public class StateRenderer
    {
        private Game game;
        private GameState gameState;

        private Color4 highlightColor = Color4.Green;

        private List<CardImage> hand, field;
        private CardImage zoomedCard;

        public StateRenderer(Game game)
        {
            this.game = game;
            game.Window.MouseUp += Window_MouseUp;
            game.Window.KeyUp += Window_KeyUp;

            hand = new List<CardImage>();
            field = new List<CardImage>();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (CardImage card in field)
            {
                Color4 color = (card.Highlight) ? highlightColor : Color4.White;
                spriteBatch.Draw(card.Card.Art, card.Position, card.Scale, color);
            }
            foreach (CardImage card in hand)
            {
                Color4 color = (card.Highlight) ? highlightColor : Color4.White;
                spriteBatch.Draw(card.Card.Art, card.Position, card.Scale, color);
            }

            if (zoomedCard != null)
            {
                spriteBatch.Draw(zoomedCard.Card.Art, new Vector2(40, 40), new Vector2(0.8f), Color4.White);
            }
        }
        public void Update(GameTime gameTime)
        {

        }

        public void HighlightCards(Zones zone)
        {
            //Remove highlight flag from all cards
            /*foreach (Card card in gameState.PlayerOne.AllCards)
            {
                card.Highlight = false;
            }

            //Highlight cards that are in the specific zones
            if (zone == Zones.Hand || zone == Zones.All)
            {
                foreach (Card card in gameState.PlayerOne.Hand)
                    card.Highlight = true;
            }
            if (zone == Zones.Field || zone == Zones.All)
            {
                foreach (Card card in gameState.PlayerOne.Field)
                    card.Highlight = true;
            }*/
        }
        public void SetGameState(GameState state)
        {
            if (gameState != null)
                gameState.InvalidTarget -= GameState_InvalidTarget;

            gameState = state;
            gameState.InvalidTarget += GameState_InvalidTarget;

            updateImageLists();
        }

        private void updateImageLists()
        {
            hand.Clear();
            field.Clear();

            //Hand
            for (int i = 0; i < gameState.PlayerOne.Hand.Count; i++)
            {
                Card card = gameState.PlayerOne.Hand[i];
                float width = card.Meta.Art.Width * 0.35f;
                float height = card.Meta.Art.Height * 0.35f;

                float start = (game.Window.Width / 2.0f) - (gameState.PlayerOne.Hand.Count * width) / 2.0f;
                Vector2 pos = new Vector2(start + i * width, game.Window.Height - height);

                hand.Add(new CardImage(card.Meta, pos, new Vector2(width, height), new Vector2(0.35f)));
            }

            //Field
            for (int i = 0; i < gameState.PlayerOne.Field.Count; i++)
            {
                Card card = gameState.PlayerOne.Field[i];
                float width = card.Meta.Art.Width * 0.25f;
                float height = card.Meta.Art.Height * 0.25f;

                Vector2 pos = new Vector2(i * width + 10, game.Window.Height - height * 3);

                field.Add(new CardImage(card.Meta, pos, new Vector2(width, height), new Vector2(0.25f)));
            }
        }

        private void Window_MouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            Vector2 mousePos = new Vector2(e.X, e.Y);

            //Left mouse press
            if (e.Button == OpenTK.Input.MouseButton.Left)
            {
                for (int i = 0; i < hand.Count; i++)
                {
                    if (hand[i].Contains(mousePos))
                    {
                        Card card = gameState.PlayerOne.Hand.RemoveCard(i);
                        if (!gameState.StageCard(gameState.PlayerOne, card))
                        {
                            gameState.PlayerOne.Hand.AddCard(card, i);
                        }

                        updateImageLists();
                        return;
                    }
                }

                //Canceling the spell, add it back to hand
                if (gameState.IsCasting)
                {
                    gameState.PlayerOne.Hand.AddCard(gameState.StagedCard, Location.Top);
                    gameState.CancelCard(gameState.PlayerOne);
                }
            }
            //Middle mouse press
            else if (e.Button == OpenTK.Input.MouseButton.Middle)
            {
                foreach (CardImage card in hand)
                {
                    if (card.Contains(mousePos))
                    {
                        zoomedCard = card;
                        return;
                    }
                }
                foreach (CardImage card in field)
                {
                    if (card.Contains(mousePos))
                    {
                        zoomedCard = card;
                        return;
                    }
                }

                zoomedCard = null;
                
            }

            updateImageLists();
        }
        private void Window_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.P)
            {
                gameState.SubmitTarget(gameState.PlayerOne, new Target(gameState.PlayerOne, true));
            }
            else if (e.Key == OpenTK.Input.Key.C)
            {
                gameState.SubmitTarget(gameState.PlayerOne, new Target(gameState.PlayerOne.Field[0]));
            }

            updateImageLists();
        }
        private void GameState_InvalidTarget(object sender, System.EventArgs e)
        {
            //Add card back to our hand incase we select an invalid target
            gameState.PlayerOne.Hand.AddCard(gameState.StagedCard, Location.Top);
            updateImageLists();
        }
    }

    public enum Zones
    {
        Hand,
        Field,
        All,
        None
    }
    public class CardImage
    {
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Scale;
        public CardInfo Card;

        public bool Highlight = false;

        public CardImage(CardInfo card, Vector2 pos, Vector2 size, Vector2 scale)
        {
            Card = card;
            Position = pos;
            Size = size;
            Scale = scale;
        }

        public bool Contains(Vector2 pos)
        {
            return (pos.X >= Position.X && pos.X <= Position.X + Size.X && pos.Y >= Position.Y && pos.Y <= Position.Y + Size.Y);
        }
    }
}

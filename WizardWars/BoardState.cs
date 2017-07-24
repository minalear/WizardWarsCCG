using System;

namespace WizardWars
{
    public class BoardState
    {
        public Player PlayerOne, PlayerTwo;
        public Collection DeckOne, DeckTwo;
        public Collection FieldOne, FieldTwo;
        public Collection HandOne, HandTwo;

        public BoardState()
        {
            PlayerOne = new Player();
            PlayerTwo = new Player();

            DeckOne = new Collection(PlayerOne);
            FieldOne = new Collection(PlayerOne);
            HandOne = new Collection(PlayerOne);
        }

        public void PlayCard(Card card)
        {
            Types type = card.Types[0];

            if (type == Types.Creature)
            {
                FieldOne.AddCard(card);
            }
        }
    }
}

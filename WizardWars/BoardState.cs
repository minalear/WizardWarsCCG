using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardWars
{
    public class BoardState
    {
        public Player PlayerOne, PlayerTwo;
        public List<Card> DeckOne, DeckTwo;
        public List<Card> FieldOne, FieldTwo;
        public List<Card> HandOne, HandTwo;

        public BoardState()
        {
            PlayerOne = new Player();
            PlayerTwo = new Player();

            DeckOne = new List<Card>();
            DeckTwo = new List<Card>();

            FieldOne = new List<Card>();
            FieldTwo = new List<Card>();

            HandOne = new List<Card>();
            HandTwo = new List<Card>();
        }

        public void ShuffleDecks()
        {
            Random random = new Random();
            for (int i = 0; i < DeckOne.Count; i++)
            {
                int a = random.Next(0, DeckOne.Count);
                int b = random.Next(0, DeckOne.Count);

                Card temp = DeckOne[a];
                DeckOne[a] = DeckOne[b];
                DeckOne[b] = temp;
            }

            for (int i = 0; i < DeckTwo.Count; i++)
            {
                int a = random.Next(0, DeckTwo.Count);
                int b = random.Next(0, DeckTwo.Count);

                Card temp = DeckTwo[a];
                DeckTwo[a] = DeckTwo[b];
                DeckTwo[b] = temp;
            }
        }

        public void DrawCards(int num)
        {
            for (int i = 0; i < num; i++)
            {
                if (DeckOne.Count > 0)
                {
                    HandOne.Add(DeckOne[0]);
                    DeckOne.RemoveAt(0);
                }
                if (DeckTwo.Count > 0)
                {
                    HandTwo.Add(DeckTwo[0]);
                    DeckTwo.RemoveAt(0);
                }
            }
        }
    }
}

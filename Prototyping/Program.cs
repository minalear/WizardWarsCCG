using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using WizardWars;

namespace Prototyping
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }

            var cardList = CardFactory.LoadCards(File.ReadAllText("Content/cards.json"));

            BoardState state = new BoardState();
            state.HandOne.SetList(cardList);
            state.DeckOne.AddCards(cardList, Location.Random);
            state.DeckOne.AddCards(cardList, Location.Random);
            state.DeckOne.AddCards(cardList, Location.Random);

            for (int i = 0; i < state.HandOne.Count; i++)
            {
                Card card = state.HandOne[i];
                state.PlayCard(state.PlayerOne, card);
            }
        }
    }
}

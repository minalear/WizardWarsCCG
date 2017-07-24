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
            var cardList = CardFactory.LoadCards(File.ReadAllText("Content/cards.json"));

            BoardState state = new BoardState();
            state.HandOne.SetList(cardList);
            
            state.PlayCard(state.HandOne.RemoveCard(Location.Top));
        }
    }
}

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
            string json = File.ReadAllText("Content/cards.json");
            List<Card> cardList = JsonConvert.DeserializeObject<List<Card>>(json);

            foreach (Card card in cardList)
            {
                PrintCardInfo(card);
            }

            Console.ReadLine();
        }

        static void PrintCardInfo(Card card)
        {
            Console.WriteLine("{0} - {1}", card.Name, card.Cost);

            foreach (string type in card.Types)
            {
                Console.Write("{0} ", type);
            }

            if (card.SubTypes.Length > 0)
            {
                Console.Write(" - ");
                foreach (string subType in card.SubTypes)
                {
                    Console.Write("{0} ", subType);
                }
            }

            Console.Write("\n");

            Console.WriteLine(" --- \n{0}\n --- \n{1}", card.RulesText, card.FlavorText);

            Console.Write("\n\n");
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using WizardWars;

namespace Prototyping
{
    class Program
    {
        static BoardState ActiveState;
        static bool Quit = false;

        static void Main(string[] args)
        {
            string json = File.ReadAllText("Content/cards.json");
            List<Card> cardList = JsonConvert.DeserializeObject<List<Card>>(json);

            ActiveState = new BoardState();
            ActiveState.DeckOne.AddRange(cardList);
            ActiveState.DeckOne.AddRange(cardList);
            ActiveState.DeckOne.AddRange(cardList);

            ActiveState.DeckTwo.AddRange(cardList);
            ActiveState.DeckTwo.AddRange(cardList);
            ActiveState.DeckTwo.AddRange(cardList);

            ActiveState.ShuffleDecks();
            ActiveState.ShuffleDecks();
            ActiveState.ShuffleDecks();

            ActiveState.DrawCards(5);

            string input = string.Empty;
            while (!Quit)
            {
                Console.Write(">> ");
                input = Console.ReadLine();
                Console.Clear();

                input = input.Trim().ToLower();
                string[] commands = input.Split(' ');
                switch (commands[0])
                {
                    case "quit":
                        Quit = true;
                        break;
                    case "decks":
                        Console.WriteLine("-- Deck One --");
                        foreach (Card card in ActiveState.DeckOne)
                        {
                            Console.WriteLine(card.Name);
                        }

                        Console.WriteLine("\n==============\n");

                        Console.WriteLine("-- Deck Two --");
                        foreach (Card card in ActiveState.DeckTwo)
                        {
                            Console.WriteLine(card.Name);
                        }
                        break;
                    case "hands":
                        Console.WriteLine("-- Hand One --");
                        foreach (Card card in ActiveState.HandOne)
                        {
                            Console.WriteLine(card.Name);
                        }

                        Console.WriteLine("\n==============\n");

                        Console.WriteLine("-- Hand Two --");
                        foreach (Card card in ActiveState.HandTwo)
                        {
                            Console.WriteLine(card.Name);
                        }
                        break;
                    case "board":
                        PrintStateInfo();
                        break;
                    case "hand":
                        if (commands[1] == "one")
                        {
                            PrintCardList(ActiveState.HandOne);
                        }
                        else
                        {
                            PrintCardList(ActiveState.HandTwo);
                        }
                        break;
                    default:
                        Console.Clear();
                        break;
                }
            }
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
        static void PrintStateInfo()
        {
            Console.WriteLine("============");
            for (int i = 0; i < ActiveState.HandOne.Count; i++)
            {
                Console.Write("[ ]");
            }
            Console.WriteLine("\n------------");
            for (int i = 0; i < ActiveState.FieldOne.Count; i++)
            {
                Console.Write("[ ]");
            }

            Console.Write("\n\n");
            Console.WriteLine("============");
            Console.Write("\n\n");

            
            for (int i = 0; i < ActiveState.FieldTwo.Count; i++)
            {
                Console.Write("[ ]");
            }
            Console.WriteLine("\n------------");
            
            for (int i = 0; i < ActiveState.HandTwo.Count; i++)
            {
                Console.Write("[ ]");
            }
            Console.WriteLine("\n============");
        }
        static void PrintCardList(List<Card> cardList)
        {
            foreach (Card card in cardList)
            {
                Console.WriteLine("{0} - {1}", card.Name, card.Cost);
            }
        }
    }
}

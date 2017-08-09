using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WizardWars
{
    public static class CardFactory
    {
        public static List<CardInfo> LoadCards(string jsonText)
        {
            return JsonConvert.DeserializeObject<List<CardInfo>>(jsonText);
        }

        public static List<Card> LoadDeckFile(Player player, string path, List<CardInfo> allCards)
        {
            List<Card> deck = new List<Card>();
            string[] lines = File.ReadAllLines(path);

            //Only read lines from -MAINBOARD- into the deck
            bool readIntoDeck = false;
            foreach (string line in lines)
            {
                if (line.Length == 0)
                    continue;

                if (line[0] == '-')
                {
                    readIntoDeck = (line == "-MAINBOARD-");
                    continue;
                }

                if (readIntoDeck)
                {
                    int sep = line.IndexOf(' ');
                    string numStr = line.Substring(0, sep);
                    string name = line.Substring(sep + 1, line.Length - sep - 1);

                    int num = int.Parse(numStr);
                    CardInfo info = getCardFromList(allCards, name);
                    info.LoadCardArt();

                    for (int i = 0; i < num; i++)
                    {
                        Card instance = new Card(info);
                        instance.Owner = player;
                        instance.Controller = player;

                        deck.Add(instance);
                    }
                }
            }

            return deck;
        }

        private static CardInfo getCardFromList(List<CardInfo> list, string title)
        {
            foreach (CardInfo card in list)
            {
                if (card.Name == title)
                    return card;
            }

            throw new ArgumentException(string.Format("No card with the name ({0}) found!", title));
        }
    }
}

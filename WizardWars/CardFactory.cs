using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Newtonsoft.Json;
using Minalear;

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
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Length == 0)
                    continue;

                if (line[0] == '-')
                {
                    readIntoDeck = (line == "-MAINBOARD-");
                    if (!readIntoDeck && line == "-HERO-")
                    {
                        //Hero card
                        CardInfo info = getCardFromList(allCards, lines[i + 1]);
                        foreach (Effect effect in info.Effects)
                            effect.Card = info;

                        info.LoadCardArt();

                        player.PlayerCard = info.CreateInstance(player);
                    }
                    else
                        continue;
                }

                if (readIntoDeck)
                {
                    int sep = line.IndexOf(' ');
                    string numStr = line.Substring(0, sep);
                    string name = line.Substring(sep + 1, line.Length - sep - 1);

                    int num = int.Parse(numStr);
                    CardInfo info = getCardFromList(allCards, name);
                    info.LoadCardArt(createCardTexture(info));

                    for (int k = 0; k < num; k++)
                    {
                        deck.Add(info.CreateInstance(player));
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
        private static Texture2D createCardTexture(CardInfo card)
        {
            //Inefficient, but it doesn't really matter right now
            Bitmap bmp = new Bitmap(284, 357);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            graphics.Clear(Color.Silver);
            //Only load files that exist
            if (File.Exists(card.ImagePath))
            {
                Image cardArt = Image.FromFile(card.ImagePath);
                graphics.DrawImage(cardArt, 0f, 35f, cardArt.Width, cardArt.Height);
            }

            Image cardFace = Image.FromFile("Content/Art/Assets/cardface.png");
            graphics.DrawImage(cardFace, 0f, 0f, cardFace.Width, cardFace.Height);

            Font titleFont = new Font("Tahoma", 18f, FontStyle.Bold);
            Font cardTextFont = new Font("Tahoma", 12f, FontStyle.Bold);

            graphics.DrawString(card.Name, titleFont, Brushes.Black, 10f, 4f);
            graphics.DrawString(card.Cost.ToString(), titleFont, Brushes.Black, 245f, 12f);

            graphics.DrawString(card.Types[0].ToString(), cardTextFont, Brushes.Black, 25f, 204f);

            graphics.DrawString(card.RulesText, cardTextFont, Brushes.Black, 5f, 228f);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Texture2D texture = new Texture2D(bmp.Width, bmp.Height, data.Scan0);
            bmp.UnlockBits(data);

            titleFont.Dispose();
            cardTextFont.Dispose();
            graphics.Dispose();
            bmp.Dispose();

            return texture;
        }
    }
}

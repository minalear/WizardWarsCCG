using System;
using System.Collections.Generic;

namespace WizardWars
{
    public class Collection
    {
        public List<Card> RawList;

        public Collection()
        {
            RawList = new List<Card>();
        }

        public void AddCard(Card card)
        {
            AddCard(card, Location.Top);
        }
        public void AddCard(Card card, Location location)
        {
            if (location == Location.Top)
                AddCard(card, 0);
            else if (location == Location.Bottom)
                AddCard(card, RawList.Count);
            else
                AddCard(card, RNG.Next(0, RawList.Count));
        }
        public void AddCard(Card card, int index)
        {
            RawList.Insert(index, card);
        }

        public Card GetCard(Location location)
        {
            if (location == Location.Top)
                return GetCard(0);
            else if (location == Location.Bottom)
                return GetCard(RawList.Count - 1);
            return GetCard(RNG.Next(0, RawList.Count));
        }
        public Card GetCard(int index)
        {
            if (index < 0 || index >= RawList.Count)
                throw new ArgumentOutOfRangeException(string.Format("{0} is out of range of this card collection.", index));
            return RawList[index];
        }
    }

    public enum Location { Top, Bottom, Random }
}

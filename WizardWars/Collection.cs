using System;
using System.Collections;
using System.Collections.Generic;

namespace WizardWars
{
    public class Collection : IEnumerable<Card>
    {
        public Player Owner;
        public int Count { get { return RawList.Count; } }

        private List<Card> RawList;

        public Collection(Player owner)
        {
            Owner = owner;
            RawList = new List<Card>();
        }
        public Collection(Player owner, List<Card> cards)
        {
            Owner = owner;
            SetList(cards);
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
        public void AddCards(List<Card> cards, Location location)
        {
            foreach (Card card in cards)
                AddCard(card, location);
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

        public Card RemoveCard(Location location)
        {
            if (location == Location.Top)
                return RemoveCard(0);
            else if (location == Location.Bottom)
                return RemoveCard(RawList.Count - 1);
            
            return RemoveCard(RNG.Next(0, RawList.Count - 1));
        }
        public Card RemoveCard(int index)
        {
            Card card = RawList[index];
            RawList.RemoveAt(index);

            return card;
        }
        public List<Card> RemoveCards(int num, Location location)
        {
            List<Card> cards = new List<Card>();

            for (int i = 0; i < num; i++)
            {
                cards.Add(RemoveCard(location));
            }

            return cards;
        }

        public void SetList(List<Card> cards)
        {
            RawList = cards;
        }
        
        public IEnumerator<Card> GetEnumerator()
        {
            return ((IEnumerable<Card>)this.RawList).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Card>)this.RawList).GetEnumerator();
        }

        public Card this[int i]
        {
            get { return GetCard(i); }
            set { RawList[i] = value; }
        }

        public override string ToString()
        {
            return string.Format("Count: {0}", RawList.Count);
        }
    }

    public enum Location { Top, Bottom, Random }
}

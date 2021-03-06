﻿using System;
using System.Collections;
using System.Collections.Generic;
using Minalear;

namespace WizardWars
{
    public class Collection : IEnumerable<Card>
    {
        public string Name;
        public Player Owner;
        public int Count { get { return RawList.Count; } }

        private List<Card> RawList;

        public Collection(string name)
        {
            Name = name;
            RawList = new List<Card>();
        }
        public Collection(string name, Player owner)
        {
            Name = name;
            Owner = owner;
            RawList = new List<Card>();
        }
        public Collection(string name, Player owner, List<Card> cards)
        {
            Name = name;
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
            card.Zone = this;
            RawList.Insert(index, card);
            collectionChanged();
        }
        public void AddCards(IEnumerable<Card> cards, Location location)
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
        public bool GetCard(string name, out Card requestedCard)
        {
            foreach (Card card in RawList)
            {
                if (card.Name == name)
                {
                    requestedCard = card;
                    return true;
                }
            }

            requestedCard = null;
            return false;
        }

        public bool HasCard(int id)
        {
            foreach (Card card in RawList)
            {
                if (card.ID == id)
                    return true;
            }
            return false;
        }
        public bool HasCard(string name)
        {
            foreach (Card card in RawList)
            {
                if (card.Name == name)
                    return true;
            }
            return false;
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
        public Card RemoveCardID(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (RawList[i].ID == id)
                {
                    Card card = RawList[i];
                    RawList.RemoveAt(i);

                    collectionChanged();

                    return card;
                }
            }

            throw new ArgumentException(string.Format("Card with ID {0} does not exist in this collection."));
        }
        public List<Card> RemoveCards(int num, Location location)
        {
            List<Card> cards = new List<Card>();

            for (int i = 0; i < num; i++)
            {
                cards.Add(RemoveCard(location));
                collectionChanged();
            }

            return cards;
        }

        public int CountTypes(Types cardType)
        {
            int num = 0;
            foreach (Card instance in this)
            {
                CardInfo card = instance.MetaInfo;
                foreach (Types type in card.Types)
                {
                    if (type == cardType)
                        num++;
                }
            }

            return num;
        }
        public int CountTypes(SubTypes cardSubType)
        {
            int num = 0;
            foreach (Card instance in this)
            {
                CardInfo card = instance.MetaInfo;
                foreach (SubTypes type in card.SubTypes)
                    num++;
            }

            return num;
        }

        public void SetList(List<Card> cards)
        {
            RawList = cards;
            collectionChanged();
        }
        public void Shuffle()
        {
            for (int i = Count - 1; i > 0; i--)
            {
                int n = RNG.Next(i + 1);
                Card temp = this[n];
                this[n] = this[i];
                this[i] = temp;
            }

            collectionChanged();
        }

        private void collectionChanged()
        {
            CollectionChanged?.Invoke();
        }
        
        public IEnumerator<Card> GetEnumerator()
        {
            return ((IEnumerable<Card>)this.RawList).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Card>)this.RawList).GetEnumerator();
        }

        public event Action CollectionChanged;

        public Card this[int i]
        {
            get { return GetCard(i); }
            set { RawList[i] = value; }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} cards.", Name, RawList.Count);
        }
    }

    public enum Location { Top, Bottom, Random }
}

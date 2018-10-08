using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UDR.Game.Exceptions;

namespace UDR.Game
{
    public class CardCollection : IEnumerable<GameCard>
    {
        protected List<GameCard> cards;
        protected bool _isSorted = false;

        private GameCardComparator _comparator = new GameCardComparator();

        public CardCollection(List<GameCard> cards)
        {
            this.cards = cards;
        }
        public CardCollection()
        {
            cards = new List<GameCard>();
        }

        public int Count => cards.Count;

        public GameCard GetCard(int idx) => cards[idx];

        internal void Shuffle()
        {
            var random = new System.Random((int)System.DateTime.Now.Ticks);
            var interations = cards.Count * 10;
            while (--interations > 0)
            {
                var card = cards[random.Next(cards.Count)];
                RemoveCard(card);
                AddCard(card, random.Next(cards.Count));
            }   
        }

        internal virtual void AddCard(GameCard card, int idx = -1)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            if (cards.Contains(card))
                throw new CardTransferException("Card already exists in collection", card);
            if (idx < 0)
                cards.Add(card);
            else
                cards.Insert(idx, card);
            if (_isSorted) cards.Sort(_comparator);
        }

        internal virtual void RemoveCard(GameCard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            if (!cards.Contains(card))
                throw new CardTransferException("Card does not exist in collection", card);
            cards.Remove(card);
        }

        internal virtual void TransferTo(CardCollection cardCollection, int number = -1)
        {
            int count = number == -1 ? cards.Count : number;
            while (count-- > 0)
            {
                var card = cards[0];
                RemoveCard(card);
                cardCollection.AddCard(card);
            }
        }

        #region IEnumerable
        IEnumerator<GameCard> IEnumerable<GameCard>.GetEnumerator()
        {
            // just return the internal collection's enumerable
            return cards.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            // just return the internal collection's enumerable
            return ((IEnumerable)cards).GetEnumerator();
        }
        #endregion

        private class GameCardComparator : IComparer<GameCard>
        {
            public int Compare(GameCard x, GameCard y)
            {
                if (x.Suit != y.Suit)
                    return (x.Suit > y.Suit) ? 1 : -1;
                if (x.Rank != y.Rank)
                    return (x.Rank > y.Rank) ? 1 : -1;
                return 0;
            }
        }
    }
}

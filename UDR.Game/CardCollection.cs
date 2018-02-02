using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UDR.Game.Exceptions;

namespace UDR.Game
{
    public abstract class CardCollection : IEnumerable<GameCard>
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

        protected virtual void AddCard(GameCard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            if (cards.Contains(card))
                throw new CardTransferException("Card already exists in collection", card);
            cards.Add(card);
            if (_isSorted) cards.Sort(_comparator);
        }

        protected virtual void RemoveCard(GameCard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));
            if (!cards.Contains(card))
                throw new CardTransferException("Card does not exist in collection", card);
            cards.Remove(card);
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

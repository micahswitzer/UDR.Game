using System.Collections.Generic;

namespace UDR.Game
{
    internal class TableCardCollection : CardCollection
    {
        public TableCardCollection(Card trumpCard) : base(sorted: true)
        {
            this.Comparator = new TrickCardComparer(trumpCard);
        }

        private class TrickCardComparer : IComparer<Card>
        {
            private readonly Card TrumpCard;

            public TrickCardComparer(Card trumpCard)
            {
                TrumpCard = trumpCard;
            }

            public int Compare(Card x, Card y)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
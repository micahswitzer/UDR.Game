using System;
using System.Collections.Generic;
using System.Text;

namespace UDR.Game
{
    public class GameCard : Card
    {
        public ICardOwner Owner { get; protected set; }

        public GameCard(Rank rank, Suit suit, ICardOwner owner) : base(rank, suit)
        {
            Owner = owner;
        }
    }
}

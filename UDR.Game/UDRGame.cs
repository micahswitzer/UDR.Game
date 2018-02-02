using System;
using System.Collections.Generic;
using System.Text;

namespace UDR.Game
{
    public class UDRGame
    {
        public IReadOnlyList<Player> Players { get; private set; }
        protected CardCollection Deck { get; set; }
        public Card TrumpCard { get; protected set; }
    }
}

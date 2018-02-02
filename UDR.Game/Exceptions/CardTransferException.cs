using System;

namespace UDR.Game.Exceptions
{
    public class CardTransferException : Exception
    {
        public GameCard Card { get; private set; }

        public CardTransferException(string message, GameCard card) : base(message)
        {
            Card = card;
        }

        public CardTransferException(GameCard card) : base()
        {
            Card = card;
        }
    }
}

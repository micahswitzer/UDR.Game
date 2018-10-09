using System;

namespace UDR.Game
{
    public abstract class Player
    {
        public UDRGame Game { get; private set;  }
        internal CardCollection Hand { get; set; }

        /// <summary>
        /// Sets the current player's game instance
        /// </summary>
        /// <param name="game">The game instance to use</param>
        /// <returns>The current player instance so that the call can be chained</returns>
        internal Player SetGame(UDRGame game)
        {
            if (Game != null)
                throw new InvalidOperationException(
                    "The game object has already been set");
            Game = game;
            return this;
        }

        public Player()
        {
            Hand = new CardCollection();
        }
    }
}
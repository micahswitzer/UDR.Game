using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UDR.Game.Exceptions;

namespace UDR.Game
{
    public class UDRGame
    {
        public IReadOnlyList<Player> Players { get; protected set; }
        protected CardCollection Deck { get; set; }
        public GameCard TrumpCard { get; protected set; }
        public int RoundNumber { get; protected set; }
        public int CardsPerPlayer =>
            RoundNumber > 7 ? 15 - RoundNumber : RoundNumber;
        public bool CanPlayersSeeOwnCards => CardsPerPlayer != 1;
        public bool CanPlayersSeeOthersCards =>
            !CanPlayersSeeOwnCards;
        public GameState GameState { get; protected set; } = GameState.NotStarted;

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Starts the game from the <code>GameState.NotStarted</code> state
        /// </summary>
        public void StartGame()
        {
            if (GameState != GameState.NotStarted)
                throw new InvalidGameStateException("The game has already started");
            // TODO: handle concurrency and protect other code from executing during this region
            //_semaphore.Wait();
            CollectCards();
            Deck.Shuffle();
            RoundNumber = 1;
            DealRound();
            GameState = GameState.InProgress;
            //_semaphore.Release();
        }

        /// <summary>
        /// Collects all cards from the game and places them in the deck
        /// </summary>
        protected void CollectCards()
        {
            foreach (var player in Players)
            { // transfer the entire player's hand to the deck
                player.Hand.TransferTo(Deck);
            }
            // move the trump card back to the deck
            Deck.AddCard(TrumpCard);
            TrumpCard = null;
        }

        /// <summary>
        /// Deals the current round 
        /// </summary>
        protected void DealRound()
        {
            if (GameState != GameState.InProgress)
                throw new InvalidGameStateException("Game must be in progress to deal a round");
            for (int i = CardsPerPlayer; i >= 0; ++i)
            {
                foreach (var player in Players)
                {
                    Deck.TransferTo(player.Hand, 1);
                }
            }
            TrumpCard = Deck.GetCard(0);
            Deck.RemoveCard(TrumpCard);
        }
    }
}

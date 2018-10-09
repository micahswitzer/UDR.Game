using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UDR.Game.Exceptions;

namespace UDR.Game
{
    public class UDRGame
    {
        public IReadOnlyList<Player> Players { get; protected set; }
        internal CardCollection Deck { get; set; }
        public GameCard TrumpCard { get; protected set; }
        public int RoundNumber { get; protected set; }
        public int CardsPerPlayer =>
            RoundNumber > 7 ? 15 - RoundNumber : RoundNumber;
        public bool CanPlayersSeeOwnCards => CardsPerPlayer != 1;
        public bool CanPlayersSeeOthersCards =>
            !CanPlayersSeeOwnCards;
        public GameState GameState { get; protected set; } = GameState.NotStarted;

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public void SetPlayers(IEnumerable<Player> players)
        {
            if (players == null) // nulls are a no no
                throw new ArgumentNullException(nameof(players));
            if (GameState != GameState.NotStarted) // the game must not have been started
                throw new InvalidGameStateException("The game has already started");
            if (Players != null) // we must only be able to set the players exactly once
                throw new InvalidGameStateException("The players have already been set");
            // we want to make a copy of the list
            Players = players.Select(x => x is null ?
                throw new ArgumentException("Players cannot be null", nameof(players)) : x).ToList();
            // check if we have a valid number of players
            if (Players.Count >= 2) return; // return here to avoid nesting
            // reset the players variable if we don't
            Players = null;
            throw new ArgumentException("There must be at least 2 players", nameof(players));
        }

        /// <summary>
        /// Starts the game from the <code>GameState.NotStarted</code> state
        /// </summary>
        public void StartGame()
        {
            if (GameState != GameState.NotStarted)
                throw new InvalidGameStateException("The game has already started");
            if (Players == null)
                throw new InvalidGameStateException("The players have not been set");
            // TODO: handle concurrency and protect other code from executing during this region
            //_semaphore.Wait();
            BuildDeck();
            Deck.Shuffle();
            RoundNumber = 1;
            GameState = GameState.InProgress;
            DealRound();
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
            if (TrumpCard == null) return;
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
            for (int i = CardsPerPlayer; i > 0; --i)
            {
                foreach (var player in Players)
                {
                    Deck.TransferTo(player.Hand, 1);
                }
            }
            TrumpCard = Deck.GetCard(0);
            Deck.RemoveCard(TrumpCard);
        }

        protected void BuildDeck()
        {
            if (Deck != null)
                throw new InvalidGameStateException("BuildDeck() has already been called");
            Deck = new CardCollection();
            for (int suit = 0; suit < 4; suit++)
                for (int rank = 1; rank <= 13; rank++)
                    Deck.AddCard(new GameCard((Rank)rank, (Suit)suit, null));
        }

        protected void AdvanceRound()
        {
            // some cool logic to make sure everyone has player their turn
            CollectCards(); // why does this method even exist?
            
        }
    }
}

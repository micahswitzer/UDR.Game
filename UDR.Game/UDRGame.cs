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

        private Dictionary<int, Dictionary<Player, int>> _roundPlayerBids;
        public IReadOnlyDictionary<Player, int> CurrentPlayerBids => RoundNumber > 0 ?
            _roundPlayerBids[RoundNumber] :
            throw new InvalidGameStateException("No round is currently in progress");

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
            Players = players.Select(x => x is null ? // check if the instance is null
                // throw an exception if it is
                throw new ArgumentException("Players cannot be null", nameof(players)) :
                // otherwise set the game object and build a list
                x.SetGame(this)).ToList();
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
            for (int i = CardsPerPlayer; i > 0; --i)
                foreach (var player in Players)
                    Deck.TransferTo(player.Hand, 1);
            TrumpCard = Deck.GetCard(0);
            Deck.RemoveCard(TrumpCard);
            GameState = GameState.PlacingBids;
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
            // some cool logic to make sure everyone has played their turn
            CollectCards(); // why does this method even exist?
            
        }

        internal void PlaceBid(Player player, int bid)
        {
            if (GameState != GameState.PlacingBids)
                throw new InvalidGameStateException("Placing bids is not allowed at this time.");
            if (!Players.Contains(player))
                throw new ArgumentException("Player is not present in player collection.", nameof(player));
            var playerBids = _roundPlayerBids[RoundNumber];
            if (playerBids.ContainsKey(player))
                throw new InvalidGameStateException("This player has already placed a bid.");
            if (bid < 0 || bid > CardsPerPlayer)
                throw new ArgumentException(
                    "Invalid value for bid. The bid must be non-negative and " +
                    "less than or equal to the number of cards per player.", nameof(bid));
            playerBids[player] = bid;
            if (playerBids.Count == Players.Count) // all players have placed bids
                // TODO: notify players of new game state
                GameState = GameState.PlayingCards;
        }
    }
}

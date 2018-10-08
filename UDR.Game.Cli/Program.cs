using System;
using UDR.Game;

namespace UDR.Game.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new UDRGame();
            var players = new[] { new CliPlayer(), new CliPlayer() };
            game.SetPlayers(players);
            game.StartGame();
            foreach (var card in game.Deck)
            {
                Console.WriteLine($"{card.Rank} {card.Suit}");
            }
            Console.WriteLine("Player 1");
            foreach (var card in players[0].Hand)
            {
                Console.WriteLine($"{card.Rank} {card.Suit}");
            }
            Console.WriteLine("Player 2");
            foreach (var card in players[1].Hand)
            {
                Console.WriteLine($"{card.Rank} {card.Suit}");
            }
            Console.WriteLine(game.Deck.Count);
        }
    }
}

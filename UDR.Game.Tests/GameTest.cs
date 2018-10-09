using System;
using System.Collections.Generic;
using UDR.Game.Exceptions;
using Xunit;

namespace UDR.Game.Tests
{
    public class GameTest
    {
        private readonly UDRGame Game;

        public GameTest()
        {
            Game = new UDRGame();
        }

        private IEnumerable<Player> SetPlayersAndAssert()
        {
            var myPlayers = new[] { new TestPlayer(), new TestPlayer() };
            Game.SetPlayers(myPlayers);
            Assert.Equal(myPlayers, Game.Players);
            return myPlayers;
        }

        [Fact]
        public void GameSate_NotStarted_AfterConstructed()
        {
            Assert.Equal(GameState.NotStarted, Game.GameState);
        }

        [Fact]
        public void SetPlayers_Allowed_FirstTime()
        {
            SetPlayersAndAssert();
        }

        [Fact]
        public void SetPlayers_PlayersHaveGameObject_Always()
        {
            SetPlayersAndAssert();
            foreach (var player in Game.Players)
                Assert.Same(Game, player.Game);
        }

        [Fact]
        public void SetPlayers_Throws_TooFewPlayers()
        {
            var myPlayers = new[] { new TestPlayer() };
            Assert.Throws<ArgumentException>(
                () => Game.SetPlayers(myPlayers));
            Assert.Null(Game.Players);
        }

        [Fact]
        public void SetPlayers_Throws_NullParameter()
        {
            Assert.Throws<ArgumentNullException>(
                () => Game.SetPlayers(null));
        }

        [Fact]
        public void SetPlayers_Throws_NullPlayer() 
        {
            var myPlayers = new[] { new TestPlayer(), (Player)null };
            Assert.Throws<ArgumentException>(
                () => Game.SetPlayers(myPlayers));
            Assert.Null(Game.Players);
        }

        [Fact]
        public void SetPlayers_Throws_CalledTwice()
        {
            var myPlayers = SetPlayersAndAssert();
            Assert.Throws<InvalidGameStateException>(
                () => Game.SetPlayers(myPlayers));
        }

        [Fact]
        public void SetPlayers_Throws_AfterStart() 
        {
            var myPlayers = SetPlayersAndAssert();
            Game.StartGame();
            Assert.Throws<InvalidGameStateException>(
                () => Game.SetPlayers(myPlayers));
        }

        [Fact]
        public void StartGame_Throws_NoPlayers()
        {
            Assert.Throws<InvalidGameStateException>(
                () => Game.StartGame());
        }

        [Fact]
        public void StartGame_InProgress_AfterRun()
        {
            SetPlayersAndAssert();
            Game.StartGame();
            Assert.Equal(GameState.PlacingBids, Game.GameState);
        }

        [Fact]
        public void StartGame_RoundOne_AfterRun()
        {
            SetPlayersAndAssert();
            Game.StartGame();
            Assert.Equal(1, Game.RoundNumber);
        }
    }
}

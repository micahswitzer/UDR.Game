using System;
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

        [Fact]
        public void GameSate_NotStarted_AfterConstructed()
        {
            Assert.Equal(GameState.NotStarted, Game.GameState);
        }

        [Fact]
        public void SetPlayers_Allowed_FirstTime()
        {
            var myPlayers = new[] { new TestPlayer(), new TestPlayer() };
            Game.SetPlayers(myPlayers);
            Assert.Equal(myPlayers, Game.Players);
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
        public void SetPlayers_Throws_CalledTwice()
        {
            var myPlayers = new[] { new TestPlayer(), new TestPlayer() };
            Game.SetPlayers(myPlayers);
            Assert.Equal(myPlayers, Game.Players);
            Assert.Throws<InvalidGameStateException>(
                () => Game.SetPlayers(myPlayers));
        }

        [Fact]
        public void SetPlayers_Throws_AfterStart() 
        {
            var myPlayers = new[] { new TestPlayer(), new TestPlayer() };
            Game.SetPlayers(myPlayers);
            Assert.Equal(myPlayers, Game.Players);
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
    }
}

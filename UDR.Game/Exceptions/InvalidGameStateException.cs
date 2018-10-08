namespace UDR.Game.Exceptions
{
    public class InvalidGameStateException : System.Exception
    {
        public InvalidGameStateException()
        {
        }

        public InvalidGameStateException(string message) : base(message)
        {
        }
    }
}
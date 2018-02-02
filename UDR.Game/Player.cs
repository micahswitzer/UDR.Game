namespace UDR.Game
{
    public abstract class Player
    {
        protected CardCollection Hand { get; private set; }

        public Player()
        {
            Hand = new CardCollection()
        }
    }
}
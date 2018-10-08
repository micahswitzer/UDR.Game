namespace UDR.Game
{
    public abstract class Player
    {
        internal CardCollection Hand { get; set; }

        public Player()
        {
            Hand = new CardCollection();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace UDR.Game
{
    public interface ICardOwner
    {
        void OnCardRemoving(GameCard card);
    }
}

using CardsAgainstHyurmanity.Model.CAHData;
using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstHyurmanity.Model.Game
{
    public class LoadedCahCards
    {
        public bool NotProperlyLoaded => !(WhiteCards.Any() || BlackCards.Any());

        public string[] WhiteCards = [];
        public BlackCard[] BlackCards = [];
        public string[] LoadedPackNames = [];

        public int WhiteDrawIndex = 0;
        public int BlackDrawIndex = 0;

        public List<BlackCard> DrawBlack(int amount)
        {
            if (BlackDrawIndex + amount >= BlackCards.Length)
            {
                BlackDrawIndex = 0;
            }

            var result = new List<BlackCard>(BlackCards.Skip(BlackDrawIndex).Take(amount));
            BlackDrawIndex += amount;
            return result;
        }

        public List<string> DrawWhite(int amount)
        {
            if (WhiteDrawIndex + amount >= WhiteCards.Length)
            {
                WhiteDrawIndex = 0;
            }

            var result = new List<string>(WhiteCards.Skip(WhiteDrawIndex).Take(amount));
            WhiteDrawIndex += amount;
            return result;
        }
    }
}

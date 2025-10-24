using DalamudBasics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstHyurmanity.Modules.WhiteCardFitting
{
    internal class CombinedCardFitter
    {
        public ILogService Log { get; }

        internal CombinedCardFitter(ILogService log)
        {
            Log = log;
        }
        public string FitCombinedCard(string combinedCard)
        {
            combinedCard = RemoveMarkersFromBlackCard(combinedCard);
            combinedCard = RemoveDuplicateArticles(combinedCard);
            combinedCard = CapitalizeStart(combinedCard);

            return combinedCard;
        }
        private string RemoveMarkersFromBlackCard(string combinedCard)
        {
            if (combinedCard.Length < 3) return combinedCard;
            if (combinedCard[combinedCard.Length - 2] == '@')
            {
                return combinedCard.Substring(0, combinedCard.Length - 2);
            }

            return combinedCard;
        }

        private string RemoveDuplicateArticles(string combinedCard)
        {
            combinedCard = combinedCard.Replace("the the", "the", StringComparison.OrdinalIgnoreCase);
            combinedCard = combinedCard.Replace("a the", "a", StringComparison.OrdinalIgnoreCase);
            combinedCard = combinedCard.Replace("the a", "the", StringComparison.OrdinalIgnoreCase);
            combinedCard = combinedCard.Replace("a a", "a", StringComparison.OrdinalIgnoreCase);

            return combinedCard;
        }

        private string CapitalizeStart(string combinedCard)
        {
            if (combinedCard == null || combinedCard == string.Empty)
            {
                return combinedCard;
            }

            return combinedCard[0].ToString().ToUpper() + combinedCard.Substring(1);
        }
    }
}

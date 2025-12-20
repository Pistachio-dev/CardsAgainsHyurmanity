using System;
using System.Collections.Generic;
using System.Text;

namespace CardsAgainstHyurmanity.Modules.Extensions
{
    public static class PlayerNameExtensions
    {
        public static string GetNameOnly(this string fullPlayerName)
        {
            if (fullPlayerName == null) return string.Empty;
            var split = fullPlayerName.Split(' ');

            return split[0];
        }
    }
}

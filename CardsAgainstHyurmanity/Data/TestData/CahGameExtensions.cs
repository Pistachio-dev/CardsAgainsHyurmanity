using CardsAgainstHyurmanity.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstHyurmanity.Data.TestData
{
    internal static class CahGameExtensions
    {
        internal static CahGame AddTestPlayers(this CahGame game)
        {
            game.Players.Add(new Player() { FullName = "Pistachio Herald@Omega" });
            game.Players.Add(new Player() { FullName = "Macalania Nut@Louisoix" });
            game.Players.Add(new Player() { FullName = "Perilous Peanut@Louisoix" });

            return game;
        }
    }
}

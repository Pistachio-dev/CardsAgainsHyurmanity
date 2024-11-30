using CardsAgainsHyurmanity.Model.CAHData;
using System.Collections.Generic;
using System.Linq;

namespace CardsAgainsHyurmanity.Model.Game
{
    public class CahGame
    {
        public List<Player> Players { get; set; } = new();
        public Player? Tzar { get; set; }
        public LoadedCahCards Deck { get; set; }
        public BlackCard BlackCard { get; set; }
        public GameStage Stage { get; set; }

        public Player[] GetNonTzarActivePlayers()
        {
            return Players.Where(p => p != Tzar && !p.AFK).ToArray();
        }
    }
}

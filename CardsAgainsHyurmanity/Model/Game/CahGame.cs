using CardsAgainsHyurmanity.Model.CAHData;
using System.Collections.Generic;

namespace CardsAgainsHyurmanity.Model.Game
{
    public class CahGame
    {
        public List<Player> Players { get; set; } = new();
        public Player? Tzar { get; set; }
        public LoadedCahCards Deck { get; set; }
        public BlackCard BlackCard { get; set; }
        public GameStage Stage { get; set; }
    }
}

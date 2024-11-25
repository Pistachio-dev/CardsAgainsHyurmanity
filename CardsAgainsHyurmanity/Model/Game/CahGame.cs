using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainsHyurmanity.Model.Game
{
    public class CahGame
    {
        public List<Player> Players { get; set; } = new();
        public Player? Tzar { get; set; }
        public LoadedCahCards Deck { get; set; }        
    }
}

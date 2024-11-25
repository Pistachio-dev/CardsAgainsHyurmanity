using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainsHyurmanity.Model.Game
{
    public class Player
    {
        public string FullName { get; set; }
        public int AwesomePoints { get; set; }
        public List<string> WhiteCards { get; set; } = new();
        public List<string> Picks { get; set; } = new();
        public int AssignedNumberForTzarPick { get; set; } = 0;
    }
}

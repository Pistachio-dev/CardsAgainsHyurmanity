using System.Collections.Generic;

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

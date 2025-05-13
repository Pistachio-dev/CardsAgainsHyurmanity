using System.Collections.Generic;

namespace CardsAgainstHyurmanity.Model.Game
{
    public class Player
    {
        public string FullName { get; set; }
        public bool AFK { get; set; } = false;
        public int AwesomePoints { get; set; }
        public List<string> WhiteCards { get; set; } = new();
        public List<string> Picks { get; set; } = new();
        public int AssignedNumberForTzarPick { get; set; } = 0;

        public void Reset()
        {
            AwesomePoints = 0;
            WhiteCards.Clear();
            Picks.Clear();
            AssignedNumberForTzarPick = 0;
        }
    }
}

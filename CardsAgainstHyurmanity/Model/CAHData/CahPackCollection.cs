using CardsAgainstHyurmanity.Model.CAHData;
using System.Collections.Generic;

namespace CardsAgainstHyurmanity.Model.CAH
{
    public class CahPackCollection
    {
        public List<string> white { get; set; }
        public List<BlackCard> black { get; set; }
        public List<CahPack> packs { get; set; }
    }
}

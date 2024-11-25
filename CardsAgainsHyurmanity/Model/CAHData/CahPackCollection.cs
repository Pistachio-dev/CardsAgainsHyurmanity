using CardsAgainsHyurmanity.Model.CAHData;
using System.Collections.Generic;

namespace CardsAgainsHyurmanity.Model.CAH
{
    public class CahPackCollection
    {
        public string[] white {  get; set; }
        public BlackCard[] black { get; set; }
        public CahPack[] packs { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainsHyurmanity.Model.CAH
{
    public class CahPackCollection
    {
        public string[] White {  get; set; }
        public string[] Black { get; set; }
        public Dictionary<string, CahPack> CahPacks { get; set; }
    }
}

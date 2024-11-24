using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainsHyurmanity.Model.CAH
{
    public class CahPack
    {
        public string Name {  get; set; }
        public string Description { get; set; }
        public bool Official { get; set; }
        public int[] White {  get; set; }
        public int[] Black { get; set; }
    }
}

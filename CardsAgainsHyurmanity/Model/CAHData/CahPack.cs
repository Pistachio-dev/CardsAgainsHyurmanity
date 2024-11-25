using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainsHyurmanity.Model.CAH
{
    public class CahPack
    {
        public string name {  get; set; }
        public string description { get; set; }
        public bool official { get; set; }
        public int[] white {  get; set; }
        public int[] black { get; set; }
    }
}

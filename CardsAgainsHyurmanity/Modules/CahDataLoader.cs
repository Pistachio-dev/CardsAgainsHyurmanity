using CardsAgainsHyurmanity.Data;
using CardsAgainsHyurmanity.Model.CAH;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CardsAgainsHyurmanity.Modules
{
    public class CahDataLoader
    {
        public CahPackCollection Load()
        {
            CahPackCollection? collection = JsonSerializer.Deserialize<CahPackCollection>(CompactJsonData.Data);
            if (collection == null)
            {
                throw new Exception("Cah data was null somehow. Weird.");
            }
            return collection;
        }
    }
}

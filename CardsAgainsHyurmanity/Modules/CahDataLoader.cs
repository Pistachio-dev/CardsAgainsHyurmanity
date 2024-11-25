using CardsAgainsHyurmanity.Data;
using CardsAgainsHyurmanity.Model.CAH;
using CardsAgainsHyurmanity.Model.CAHData;
using CardsAgainsHyurmanity.Model.Game;
using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CardsAgainsHyurmanity.Modules
{
    public class CahDataLoader
    {
        private readonly IConfigurationService<Configuration> configService;
        private readonly ILogService logService;
        private CahPackCollection? data = null;

        public CahDataLoader(IConfigurationService<Configuration> configService, ILogService logService)
        {
            this.configService = configService;
            this.logService = logService;
        }

        public CahPackCollection GetFullData()
        {
            if (data == null)
            {
                Load();
            }

            return data!;
        }

        public LoadedCahCards BuildDeck(List<CahPackSettings> settings)
        {
            var indexes = settings.Where(p => p.Enabled).Select(p => p.IndexInData).ToList();
            return BuildDeck(indexes);
        }

        public LoadedCahCards BuildDeck(int packIndex)
        {
            return BuildDeck(new List<int> { packIndex });
        }

        private LoadedCahCards BuildDeck(List<int> packIndexes)
        {
            var deck = new LoadedCahCards();
            var data = GetFullData();
            (int blackCount, int whiteCount) = GetTotalCardAmount(packIndexes);
            deck.BlackCards = new BlackCard[blackCount];
            deck.WhiteCards = new string[whiteCount];

            return Populate(packIndexes, ref deck);
        }

        // Arrays must have been sized already.
        private LoadedCahCards Populate(List<int> enabledPackIndexes, ref LoadedCahCards deck)
        {
            var data = GetFullData();
            HashSet<int> whiteIndexes = new();
            HashSet<int> blackIndexes = new();

            int blackIndex = 0;
            int whiteIndex = 0;
            foreach (var i in enabledPackIndexes)
            {
                var packData = data.packs[i];
                foreach (var cardIndex in packData.white)
                {
                    if (!whiteIndexes.Contains(cardIndex))
                    {
                        whiteIndexes.Add(cardIndex);
                        deck.WhiteCards[whiteIndex] = data.white[cardIndex];
                        whiteIndex++;
                    }
                }

                foreach (var cardIndex in packData.black)
                {
                    if (!blackIndexes.Contains(cardIndex))
                    {
                        blackIndexes.Add(cardIndex);
                        deck.BlackCards[blackIndex] = data.black[cardIndex];
                        blackIndex++;
                    }
                }
            }

            deck.LoadedPackNames = enabledPackIndexes.Select(index => data.packs[index].name).ToArray();

            return deck;
        }

        private (int blackAmount, int whiteAmount) GetTotalCardAmount(List<int> enabledPackIndexes)
        {
            var data = GetFullData();
            HashSet<int> whiteIndexes = new();
            HashSet<int> blackIndexes = new();
            foreach (var i in enabledPackIndexes)
            {
                var packData = data.packs[i];
                foreach (var cardIndex in packData.black)
                {
                    blackIndexes.Add(cardIndex);
                }
                foreach (var cardIndex in packData.white)
                {
                    whiteIndexes.Add(cardIndex);
                }
            }

            return (blackIndexes.Count, whiteIndexes.Count);
        }

        public LoadedCahCards RandomizeDeck(LoadedCahCards deck)
        {
            var rng = new Random();
            rng.Shuffle(deck.BlackCards);
            rng.Shuffle(deck.WhiteCards);

            return deck;
        }

        private CahPackCollection Load()
        {
            CahPackCollection? collection = JsonSerializer.Deserialize<CahPackCollection>(CompactJsonData.Data);
            if (collection == null)
            {
                throw new Exception("Cah data was null somehow. Weird.");
            }

            data = collection;
            return data;
        }

        public void InitializeConfigIfNeeded()
        {
            var configuration = configService.GetConfiguration();
            if (configuration.PackSelections.Any())
            {
                return;
            }
            var fullCollection = Load();
            var packDictionary = new List<CahPackSettings>();
            for(int i = 0; i < fullCollection.packs.Length; i++)
            {
                var pack = fullCollection.packs[i];
                packDictionary.Add(new CahPackSettings()
                {
                    Name = pack.name,
                    Enabled = pack.name == "CAH Base Set" ? true : false,
                    IndexInData = i
                });

            }

            configuration.PackSelections = packDictionary;
            configService.SaveConfiguration();
        }
    }
}

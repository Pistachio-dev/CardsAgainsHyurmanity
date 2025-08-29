using CardsAgainstHyurmanity.Data;
using CardsAgainstHyurmanity.Model.CAH;
using CardsAgainstHyurmanity.Model.CAHData;
using CardsAgainstHyurmanity.Model.Game;
using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CardsAgainstHyurmanity.Modules.DataLoader
{
    public class CahDataLoader
    {
        private readonly IConfigurationService<Configuration> configService;
        private readonly ILogService logService;
        private CahPackCollection? data = null;
        private string[] enabledByDefaultPacks = ["Final Fantasy XIV"];

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
            (var blackCount, var whiteCount) = GetTotalCardAmount(packIndexes);
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

            var blackIndex = 0;
            var whiteIndex = 0;
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
            var collection = JsonSerializer.Deserialize<CahPackCollection>(CompactJsonData.Data);
            if (collection == null)
            {
                throw new Exception("Cah data was null somehow. Weird.");
            }

            AppendCustomPack(collection, new FFXIVCahPack());
#if DEBUG
            AppendCustomPack(collection, new LoadTestPack());
#endif
            data = collection;

            return data;
        }

        private void AppendCustomPack(CahPackCollection collection, ICustomCahPack customPack)
        {
            var package = new CahPack()
            {
                name = customPack.Name,
                description = customPack.Description,
                official = customPack.Official,
            };

            var blackIndexes = new List<int>(customPack.Black.Length);
            var blackIndex = collection.black.Count;
            foreach (var blackCardText in customPack.Black)
            {
                var pick = blackCardText.Count(c => c == '_');
                collection.black.Add(new BlackCard() { pick = pick, text = blackCardText });
                blackIndexes.Add(blackIndex);
                blackIndex++;
            }

            var whiteIndexes = new List<int>(customPack.White.Length);
            var whiteIndex = collection.white.Count;
            foreach (var whiteCardText in customPack.White)
            {
                collection.white.Add(whiteCardText);
                whiteIndexes.Add(whiteIndex);
                whiteIndex++;
            }

            package.white = whiteIndexes.ToArray();
            package.black = blackIndexes.ToArray();
            collection.packs = new CahPack[1] { package }.Concat(collection.packs).ToList();
        }

        public void InitializeConfigIfNeeded()
        {
            var configuration = configService.GetConfiguration();
            var fullCollection = Load();
            if (configuration.PackSelections.Count == fullCollection.packs.Count)
            {
                return;
            }

            HashSet<string> enabledPackNames = new HashSet<string>(configuration.PackSelections.Where(p => p.Enabled).Select(p => p.Name));
            var packList = new List<CahPackSettings>();
            for (var i = 0; i < fullCollection.packs.Count; i++)
            {
                var pack = fullCollection.packs[i];
                bool enabled = enabledPackNames.Contains(fullCollection.packs[i].name);
                packList.Add(new CahPackSettings()
                {
                    Name = pack.name,
                    Enabled = enabledByDefaultPacks.Contains(pack.name) ? true : enabled,
                    IndexInData = i
                });
            }

            configuration.PackSelections = packList;
            configService.SaveConfiguration();
        }
    }
}

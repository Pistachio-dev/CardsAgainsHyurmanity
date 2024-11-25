using CardsAgainsHyurmanity.Model.Game;
using DalamudBasics.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainsHyurmanity.Modules
{
    public class GameActions
    {
        private readonly CahGame game;
        private readonly CahDataLoader loader;
        private readonly Configuration configuration;

        public GameActions(CahGame game, CahDataLoader loader, IConfigurationService<Configuration> configService)
        {
            this.game = game;
            this.loader = loader;
            this.configuration = configService.GetConfiguration();
        }

        public void ReloadDeck()
        {
            game.Deck = loader.RandomizeDeck(loader.BuildDeck(configuration.PackSelections));
        }
    }
}

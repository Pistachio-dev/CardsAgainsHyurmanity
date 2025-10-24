using CardsAgainstHyurmanity.Model.Game;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using System.Collections.Generic;
using System.Text;

namespace CardsAgainstHyurmanity.Modules
{
    public class CahChatOutput : ChatOutput
    {
        private const int DelayBetweenTells = 2000;
        private readonly IClientChatGui _chatGui;
        private readonly Configuration _configuration;

        public CahChatOutput(IConfigurationService<Configuration> configurationService, ILogService logService, IClientChatGui chatGui, IClientState clientState, ITargetingService targetingService)
            : base(configurationService.GetConfiguration(), logService, chatGui, clientState, targetingService)
        {
            _chatGui = chatGui;
            _configuration = configurationService.GetConfiguration();
        }

        public void TellPlayerWhiteCards(string fullPlayerName, List<string> whiteCards)
        {
            StringBuilder s = new StringBuilder($"{Plugin.Watermark}Your white cards: ");
            for (int i = 0; i < whiteCards.Count; i++)
            {
                if ((s.Length + whiteCards[i].Length) > 420)
                {
                    SendTell(s.ToString(), fullPlayerName, GetOutputTypeForTell(), DelayBetweenTells);
                    s.Clear();
                }
                s.Append($"({i + 1}) {whiteCards[i]}");                
            }

            SendTell(s.ToString(), fullPlayerName, GetOutputTypeForTell(), DelayBetweenTells);

            if (_configuration.RemoveOutgoingCardsChat)
            {
                _chatGui.Print($"{fullPlayerName.GetFirstName()} received their {whiteCards.Count} cards");
                return;
            }
        }

        public void TellYouAreTzar(Player player)
        {
            SendTell($"{Plugin.Watermark}Look at you. You are the card Tzar now.", player.FullName, GetOutputTypeForTell(), DelayBetweenTells);

            if (_configuration.RemoveOutgoingCardsChat)
            {
                _chatGui.Print($"{player.FullName.GetFirstName()} is the new Tzar.");
                return;
            }
        }
    }
}

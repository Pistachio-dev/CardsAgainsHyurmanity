using CardsAgainsHyurmanity.Model.Game;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Output;
using DalamudBasics.Configuration;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using System.Text;

namespace CardsAgainsHyurmanity.Modules
{
    public class CahChatOutput : ChatOutput
    {
        private const int DelayBetweenTells = 2000;

        public CahChatOutput(IConfiguration configuration, ILogService logService, IClientChatGui chatGui, IClientState clientState, ITargetingService targetingService)
            : base(configuration, logService, chatGui, clientState, targetingService)
        {
        }

        public void TellPlayerWhiteCards(Player player)
        {
            StringBuilder s = new StringBuilder("Your white cards: ");
            for (int i = 0; i < player.WhiteCards.Count; i++)
            {
                s.Append($"({i + 1}) {player.WhiteCards[i]}");
            }

            SendTell(s.ToString(), player.FullName, GetOutputTypeForTell(), DelayBetweenTells);
        }

        public void TellYouAreTzar(Player player)
        {
            SendTell("Look at you. You are the card Tzar now.", player.FullName, GetOutputTypeForTell(), DelayBetweenTells);
        }
    }
}

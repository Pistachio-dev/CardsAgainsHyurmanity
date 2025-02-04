using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using System;

namespace CardsAgainsHyurmanity.Modules
{
    public class ReceivedChatMuter
    {
        private readonly IClientState clientState;
        private readonly IChatGui chatGui;
        private readonly ILogService logService;
        private Configuration configuration;

        public ReceivedChatMuter(IClientState clientState, IChatGui chatGui, IConfigurationService<Configuration> configurationService, ILogService logService)
        {
            this.clientState = clientState;
            this.chatGui = chatGui;
            this.logService = logService;
            this.configuration = configurationService.GetConfiguration();
        }

        public void AddOutgoingChatMuter()
        {
            chatGui.ChatMessage += RemoveCardSentMessage;
        }

        private void RemoveCardSentMessage(XivChatType type, int timestamp, ref SeString messageSender, ref SeString messageMessage, ref bool isHandled)
        {
            if (type != XivChatType.TellOutgoing)
            {
                return;
            }

            if (configuration.RemoveOutgoingCardsChat && messageMessage.TextValue.StartsWith(Plugin.Watermark, StringComparison.OrdinalIgnoreCase))
            {
                isHandled = true;
            }            
        }

    }
}

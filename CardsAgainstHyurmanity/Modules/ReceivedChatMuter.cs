using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using System;

namespace CardsAgainstHyurmanity.Modules
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

        private void RemoveCardSentMessage(IHandleableChatMessage messageRaw)
        {
            var type = messageRaw.LogKind;
            if (type != XivChatType.TellOutgoing)
            {
                return;
            }

            if (configuration.RemoveOutgoingCardsChat && messageRaw.Message.TextValue.StartsWith(Plugin.Watermark, StringComparison.OrdinalIgnoreCase))
            {
                messageRaw.PreventOriginal();
            }            
        }

    }
}

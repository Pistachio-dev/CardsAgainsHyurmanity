using Dalamud.Game.Text;
using DalamudBasics.Configuration;
using DalamudBasics.GUI.Forms;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using Dalamud.Bindings.ImGui;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Numerics;
using System.Linq;
using ECommons;

namespace CardsAgainstHyurmanity.Windows;

public class ConfigWindow : PluginWindowBase, IDisposable
{
    private ImGuiFormFactory<Configuration> formFactory;
    private IConfigurationService<Configuration> configurationService;
    private readonly Vector4 lightGreen = new Vector4(162 / 255f, 1, 153 / 255f, 1);

    public ConfigWindow(ILogService logService, IServiceProvider serviceProvider) : base(logService, "Configuration")
    {
        Size = new Vector2(500, 600);
        //SizeCondition = ImGuiCond.Appearing;
        this.configurationService = serviceProvider.GetRequiredService<IConfigurationService<Configuration>>();

        this.formFactory = new ImGuiFormFactory<Configuration>(() => configurationService.GetConfiguration(), (data) => configurationService.SaveConfiguration());
    }

    public void Dispose()
    { }

    public override void PreDraw()
    {
    }

    protected override void SafeDraw()
    {
        DrawSectionHeader("Chat");

        DrawChatChannelSelector();

        DrawSectionHeader("Game");
        formFactory.AddValidationText(formFactory.DrawIntInput("Starting white cards", nameof(Configuration.InitialWhiteCardsDrawnAmount), EnforcePositiveInt));
        formFactory.AddValidationText(formFactory.DrawIntInput("Awesome points to win", nameof(Configuration.AwesomePointsToWin), EnforcePositiveInt));
        formFactory.AddValidationText(formFactory.DrawIntInput("Delay between answers (in ms)", nameof(Configuration.AnswersRolloutDelayInMs), EnforcePositiveInt));
        formFactory.DrawCheckbox("Remove outgoing /tells", nameof(Configuration.RemoveOutgoingCardsChat));
        DrawTooltip("Removes the outgoing /tell for player cards, so your chatbox does not get spammed. This will not affect any other messaage.");
    }

    private void DrawSectionHeader(string title)
    {
        ImGui.Separator();
        ImGui.TextColored(lightGreen, title);
    }

    private string? EnforcePositiveInt(int number)
    {
        return number >= 0 ? null : "Number must be positive";
    }

    private void DrawChatChannelSelector()
    {
        XivChatType[] channels = { XivChatType.Echo, XivChatType.Party, XivChatType.Alliance, XivChatType.Say,
                XivChatType.Ls1, XivChatType.Ls2, XivChatType.Ls3, XivChatType.Ls4, XivChatType.Ls5, XivChatType.Ls6, XivChatType.Ls7, XivChatType.Ls8,
                XivChatType.CrossLinkShell1, XivChatType.CrossLinkShell2, XivChatType.CrossLinkShell3, XivChatType.CrossLinkShell4, XivChatType.CrossLinkShell5, XivChatType.CrossLinkShell6, XivChatType.CrossLinkShell7, XivChatType.CrossLinkShell8};
        string[] options = channels.Select(entry => entry.ToString()).ToArray();
        var local = channels.IndexOf(configurationService.GetConfiguration().DefaultOutputChatType);
        if (local == -1)
        {
            local = 0;
        }
        if (ImGui.Combo("Chat channel", ref local, options))
        {
            if (local >= channels.Length)
            {
                return;
            }
            configurationService.GetConfiguration().DefaultOutputChatType = channels[local];
            configurationService.SaveConfiguration();
        }        
    }
}

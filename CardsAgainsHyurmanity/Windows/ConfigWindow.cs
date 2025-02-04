using Dalamud.Game.Text;
using DalamudBasics.Configuration;
using DalamudBasics.GUI.Forms;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Numerics;

namespace CardsAgainsHyurmanity.Windows;

public class ConfigWindow : PluginWindowBase, IDisposable
{
    private Configuration configuration;
    private ImGuiFormFactory<Configuration> formFactory;
    private IConfigurationService<Configuration> configurationService;
    private readonly Vector4 lightGreen = new Vector4(162 / 255f, 1, 153 / 255f, 1);

    public ConfigWindow(ILogService logService, IServiceProvider serviceProvider) : base(logService, "Configuration")
    {
        Size = new Vector2(500, 600);
        SizeCondition = ImGuiCond.Appearing;
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
        ImGui.BeginGroup();
        ImGui.TextUnformatted("Write to: ");
        formFactory.DrawUshortRadio(nameof(Configuration.DefaultOutputChatType), sameLine: true,
            [("/echo", (ushort)XivChatType.Echo, null),
                ("/party", (ushort)XivChatType.Party, null),
                ("/alliance", (ushort)XivChatType.Alliance, null),
                ("/say", (ushort)XivChatType.Say, null)]);
        ImGui.EndGroup();

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
}

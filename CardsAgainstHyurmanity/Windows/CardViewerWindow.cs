using CardsAgainstHyurmanity.Model.CAH;
using CardsAgainstHyurmanity.Model.Game;
using CardsAgainstHyurmanity.Modules;
using CardsAgainstHyurmanity.Modules.DataLoader;
using DalamudBasics.Configuration;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using Humanizer;
using Dalamud.Bindings.ImGui;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace CardsAgainstHyurmanity.Windows;

public class CardViewerWindow : PluginWindowBase, IDisposable
{
    public LoadedCahCards CardsToView;
    private CahPackCollection fullData;
    private IConfigurationService<Configuration> configService;
    private Configuration configuration;
    private GameActions gameActions;
    private HashSet<int> changedIndexes = new();

    public CardViewerWindow(ILogService logService, IServiceProvider serviceProvider)
        : base(logService, "Card viewer")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, 900)
        };

        configService = serviceProvider.GetRequiredService<IConfigurationService<Configuration>>();
        configuration = configService.GetConfiguration();
        fullData = serviceProvider.GetRequiredService<CahDataLoader>().GetFullData();
        gameActions = serviceProvider.GetRequiredService<GameActions>();
    }

    public void Dispose()
    { }

    public void EnsureOpen()
    {
        ImGui.SetWindowCollapsed(false);
    }

    protected override void SafeDraw()
    {
        if (CardsToView == null)
        {
            return;
        }

        ImGui.TextUnformatted($"Packs: {CardsToView.LoadedPackNames.Humanize()}.");
        if (ImGui.CollapsingHeader($"Black cards ({CardsToView.BlackCards.Length})"))
        {
            foreach (var blackCard in CardsToView.BlackCards)
            {
                if (blackCard == null)
                {
                    continue;
                }
                ImGui.TextUnformatted(blackCard.text);
            }
        }

        if (ImGui.CollapsingHeader($"White cards ({CardsToView.WhiteCards.Length})"))
        {
            foreach (var whiteCard in CardsToView.WhiteCards)
            {
                ImGui.TextUnformatted(whiteCard);
            }
        }
    }
}

using CardsAgainstHyurmanity.Model.CAH;
using CardsAgainstHyurmanity.Model.CAHData;
using CardsAgainstHyurmanity.Model.Game;
using CardsAgainstHyurmanity.Modules;
using CardsAgainstHyurmanity.Modules.DataLoader;
using DalamudBasics.Configuration;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using Dalamud.Bindings.ImGui;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CardsAgainstHyurmanity.Windows;

public class PackSelectionWindow : PluginWindowBase, IDisposable
{
    private readonly Plugin plugin;
    private CahPackCollection fullData;
    private IConfigurationService<Configuration> configService;
    private Configuration configuration;
    private GameActions gameActions;
    private HashSet<int> changedIndexes = new();
    private CahDataLoader loader;

    public PackSelectionWindow(ILogService logService, IServiceProvider serviceProvider, Plugin plugin)
        : base(logService, "Card pack selector", ImGuiWindowFlags.AlwaysAutoResize)
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
        loader = serviceProvider.GetRequiredService<CahDataLoader>();
        this.plugin = plugin;
    }

    public void Dispose()
    { }

    protected override void SafeDraw()
    {
        bool changes = changedIndexes.Any();
        DrawWithinDisableBlock(changes, () => DrawActionButton(() =>
        {
            changedIndexes.Clear();
            configService.SaveConfiguration();
            gameActions.ReloadDeck();
        }, "Save and remake deck"));
        if (changes)
        {
            ImGui.SameLine();
            ImGui.TextColored(new Vector4(1, 0, 0, 1), $"You have unsaved changes");
        }

        DrawActionButton(() => plugin.ViewCards(loader.BuildDeck(configuration.PackSelections)),
            changes ? "View selected cards (save to update)" : "View selected cards");

        foreach (var entry in configService.GetConfiguration().PackSelections)
        {
            DrawCheckbox(entry);
            ImGui.SameLine();
            DrawActionButton(() => plugin.ViewCards(loader.BuildDeck(entry.IndexInData)), $"View cards##{entry.IndexInData}");
        }
    }

    private void DrawCheckbox(CahPackSettings packSettings)
    {
        var local = packSettings.Enabled;
        if (ImGui.Checkbox(packSettings.Name, ref local))
        {
            if (changedIndexes.Contains(packSettings.IndexInData))
            {
                changedIndexes.Remove(packSettings.IndexInData);
            }
            else
            {
                changedIndexes.Add(packSettings.IndexInData);
            }
        }

        packSettings.Enabled = local;
    }
}

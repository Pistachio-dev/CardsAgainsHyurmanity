using CardsAgainsHyurmanity.Model.CAH;
using CardsAgainsHyurmanity.Model.CAHData;
using CardsAgainsHyurmanity.Model.Game;
using CardsAgainsHyurmanity.Modules;
using DalamudBasics.Configuration;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CardsAgainsHyurmanity.Windows;

public class PackSelectionWindow : PluginWindowBase, IDisposable
{
    private CahPackCollection fullData;
    private IConfigurationService<Configuration> configService;
    private Configuration configuration;
    private GameActions gameActions;
    private CahGame game;
    private HashSet<int> changedIndexes = new();
    public PackSelectionWindow(ILogService logService, IServiceProvider serviceProvider)
        : base(logService, "Card pack selector", ImGuiWindowFlags.AlwaysAutoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, 900)
        };

        game = serviceProvider.GetRequiredService<CahGame>();
        configService = serviceProvider.GetRequiredService<IConfigurationService<Configuration>>();
        configuration = configService.GetConfiguration();
        fullData = serviceProvider.GetRequiredService<CahDataLoader>().GetFullData();
        gameActions = serviceProvider.GetRequiredService<GameActions>();
    }

    public void Dispose() { }

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
        foreach (var entry in configService.GetConfiguration().PackSelections)
        {
            DrawCheckbox(entry);
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

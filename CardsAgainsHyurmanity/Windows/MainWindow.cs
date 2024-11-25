using CardsAgainsHyurmanity.Model.Game;
using CardsAgainsHyurmanity.Modules;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using ImGuiNET;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Numerics;

namespace CardsAgainsHyurmanity.Windows;

public class MainWindow : PluginWindowBase, IDisposable
{
    private readonly Plugin plugin;
    private CahGame game;
    private GameActions gameActions;
    public MainWindow(ILogService logService, IServiceProvider serviceProvider, Plugin plugin)
        : base(logService, "CardsAgainsHyurmanity", ImGuiWindowFlags.AlwaysAutoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        game = serviceProvider.GetRequiredService<CahGame>();
        this.plugin = plugin;
        this.gameActions = serviceProvider.GetRequiredService<GameActions>();
    }

    public void Dispose() { }

    protected override void SafeDraw()
    {
        DrawPlayerTable();
        if (game.Players.Count >= 3 && game.Stage == GameStage.NotStarted)
        {
            DrawActionButton(() => gameActions.StartGame(), "Start game");
        }
        if (game.Stage == GameStage.PlayersPicking)
        {
            DrawActionButton(() => gameActions.PresentPicks(), "Someone is afk, skip to the round end");
        }
        if (game.Stage == GameStage.TzarPicking)
        {
            DrawActionButton(() => gameActions.NextRound(), "Tzar is afk, skip to next round");
        }

        DrawActionButton(() => gameActions.AddTargetPlayer(), "Add target player");
        ImGui.SameLine();
        DrawActionButton(() => plugin.TogglePackSelectorUI(), "Select card packs");
        ImGui.SameLine();
        DrawActionButton(() => plugin.ToggleConfigUI(), "Configuration");
    }

    private void DrawPlayerTable()
    {
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;

        if (ImGui.BeginTable("##PlayerTable", 2, flags))
        {
            ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.8f);
            ImGui.TableSetupColumn("A-points", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            DrawTooltip("Awesome poins");

            ImGui.TableHeadersRow();

            foreach (var player in game.Players)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                string playerNameText = player.FullName;
                if (game.Tzar == player)
                {
                    playerNameText += " (card tzar)";
                }

                ImGui.TextUnformatted(playerNameText);

                ImGui.TableNextRow();
                ImGui.TextUnformatted(player.AwesomePoints.ToString());
            }

            ImGui.EndTable();
        }
    }
}

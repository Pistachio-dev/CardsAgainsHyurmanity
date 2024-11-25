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
    }

    public void Dispose() { }

    protected override void SafeDraw()
    {
        DrawPlayerTable();
        DrawActionButton(() => plugin.TogglePackSelectorUI(), "Select card packs");
    }

    private void DrawPlayerTable()
    {
        const ImGuiTableFlags flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Borders;

        if (ImGui.BeginTable("##PlayerTable", 2, flags))
        {
            ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.8f);
            ImGui.TableSetupColumn("Awesome points", ImGuiTableColumnFlags.WidthStretch, 0.2f);

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

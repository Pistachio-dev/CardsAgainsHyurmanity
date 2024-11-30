using CardsAgainsHyurmanity.Model.Game;
using CardsAgainsHyurmanity.Modules;
using CardsAgainsHyurmanity.Modules.DataLoader;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
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
    private CahDataLoader dataLoader;
    private ITargetingService targetingService;
    private IClientChatGui chatGui;
    private Player? playerToRemove;

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
        this.dataLoader = serviceProvider.GetRequiredService<CahDataLoader>();
        this.targetingService = serviceProvider.GetRequiredService<ITargetingService>();
        this.chatGui = serviceProvider.GetRequiredService<IClientChatGui>();
    }

    public void Dispose()
    { }

    protected override void SafeDraw()
    {
        if (!plugin.CardsAreLoaded)
        {
            ImGui.TextUnformatted("Cards are not loaded");
            DrawActionButton(() =>
            {
                dataLoader.GetFullData();
                plugin.CardsAreLoaded = true;
            }, "Load cards");

            return;
        }

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

        if (ImGui.BeginTable("##PlayerTable", 3, flags))
        {
            ImGui.TableSetupColumn("Player", ImGuiTableColumnFlags.WidthStretch, 0.8f);
            ImGui.TableSetupColumn("A-points", ImGuiTableColumnFlags.WidthStretch, 0.2f);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthStretch, 0.2f);
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
                if (player.AFK)
                {
                    playerNameText += "(AFK)";
                }

                ImGui.TextUnformatted(playerNameText);

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    if (targetingService.TargetPlayer(player.FullName))
                    {
                        chatGui.Print($"Targeting {player.FullName}.");
                    }
                    else
                    {
                        chatGui.Print($"Could not target {player.FullName}.");
                    }
                }
                if (ImGui.IsItemClicked(ImGuiMouseButton.Right) && ImGui.GetIO().KeyShift)
                {
                    playerToRemove = player;
                }
                DrawTooltip("Click to target the player, shift + right click to remove them.");


                ImGui.TableNextColumn();
                ImGui.TextUnformatted(player.AwesomePoints.ToString());

                ImGui.TableNextColumn();
                if (ImGui.Button($"##{player.FullName}"))
                {
                    gameActions.ToggleAFK(player);
                }
                DrawTooltip(player.AFK ? "Unmark as AFK" : "Mark as AFK");
            }

            ImGui.EndTable();
        }

        if (playerToRemove != null)
        {
            gameActions.RemovePlayer(playerToRemove);
            playerToRemove = null;
        }
    }
}

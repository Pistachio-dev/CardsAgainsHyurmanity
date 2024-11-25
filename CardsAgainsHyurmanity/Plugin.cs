using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DalamudBasics.Chat.Listener;
using DalamudBasics.Debugging;
using DalamudBasics.DependencyInjection;
using DalamudBasics.Interop;
using DalamudBasics.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using CardsAgainsHyurmanity.Windows;
using CardsAgainsHyurmanity.Model.Game;
using CardsAgainsHyurmanity.Modules;
using System.Net;
using ImGuiNET;
using ECommons;

namespace CardsAgainsHyurmanity;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/cah";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Cards Agains Hyurmanity");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private PackSelectionWindow PackSelectionWindow { get; init; }
    private CardViewerWindow CardViewerWindow { get; init; }
    private IServiceProvider serviceProvider { get; init; }
    private ILogService logService { get; set; }

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this);

        serviceProvider = BuildServiceProvider(pluginInterface);
        logService = serviceProvider.GetRequiredService<ILogService>();        

        InitializeServices(serviceProvider);

        ConfigWindow = new ConfigWindow(logService, serviceProvider);
        MainWindow = new MainWindow(logService, serviceProvider, this);
        PackSelectionWindow = new PackSelectionWindow(logService, serviceProvider, this);
        CardViewerWindow = new CardViewerWindow(logService, serviceProvider);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(PackSelectionWindow);
        WindowSystem.AddWindow(CardViewerWindow);

        serviceProvider.GetRequiredService<ICommandManager>().AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Type /cah to start"
        });

        pluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        serviceProvider.GetRequiredService<CahDataLoader>().InitializeConfigIfNeeded();
    }

    public void Dispose()
    {
        serviceProvider.GetRequiredService<HookManager>().Dispose();
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        serviceProvider.GetRequiredService<ICommandManager>().RemoveHandler(CommandName);        
    }

    private IServiceProvider BuildServiceProvider(IDalamudPluginInterface pluginInterface)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddAllDalamudBasicsServices<Configuration>(pluginInterface);
        serviceCollection.AddSingleton<StringDebugUtils>();
        serviceCollection.AddSingleton<CahGame>();
        serviceCollection.AddSingleton<CahDataLoader>();
        serviceCollection.AddSingleton<GameActions>();
        serviceCollection.AddSingleton<CahChatOutput>();

        return serviceCollection.BuildServiceProvider();
    }

    private void InitializeServices(IServiceProvider serviceProvider)
    {
        IFramework framework = serviceProvider.GetRequiredService<IFramework>();
        serviceProvider.GetRequiredService<ILogService>().AttachToGameLogicLoop(framework);
        serviceProvider.GetRequiredService<IChatListener>().InitializeAndRun("[CAH]", ChatChannelSets.CommonChannelsAndLinkshells);
        serviceProvider.GetRequiredService<HookManager>();
        serviceProvider.GetRequiredService<CahChatOutput>().InitializeAndAttachToGameLogicLoop(framework, "[CaH]");
        serviceProvider.GetRequiredService<GameActions>().AddChatListeners();
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our main ui
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
    public void TogglePackSelectorUI() => PackSelectionWindow.Toggle();

    public void ViewCards(LoadedCahCards dataToView)
    {
        CardViewerWindow.CardsToView = dataToView;
        if (!CardViewerWindow.IsOpen)
        {
            CardViewerWindow.Toggle();
        }

        CardViewerWindow.EnsureOpen();
    }
}

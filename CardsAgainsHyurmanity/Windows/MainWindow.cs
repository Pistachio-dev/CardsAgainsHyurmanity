using CardsAgainsHyurmanity.Modules;
using DalamudBasics.GUI.Windows;
using DalamudBasics.Logging;
using ImGuiNET;
using System;
using System.Numerics;

namespace CardsAgainsHyurmanity.Windows;

public class MainWindow : PluginWindowBase, IDisposable
{
    public MainWindow(ILogService logService, IServiceProvider serviceProvider)
        : base(logService, "CardsAgainsHyurmanity", ImGuiWindowFlags.AlwaysAutoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public void Dispose() { }

    protected override void SafeDraw()
    {
        if (ImGui.Button("test"))
        {
            new CahDataLoader().Load();
        }
    }
}

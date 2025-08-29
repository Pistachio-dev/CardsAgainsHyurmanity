using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Plugin.Services;
using DalamudBasics.Logging;
using ECommons.ExcelServices;
using System;
using System.Linq;

namespace CardsAgainstHyurmanity.Modules
{
    internal class ContextMenuManager
    {
        private static readonly string[] ValidAddons =
        [
            null,
            "PartyMemberList",
            "FriendList",
            "FreeCompany",
            "LinkShell",
            "CrossWorldLinkshell",
            "_PartyList",
            "ChatLog",
            "LookingForGroup",
            "BlackList",
            "ContentMemberList",
            "SocialList",
            "ContactList",
        ];
        private readonly ILogService logService;
        private readonly GameActions gameActions;
        private readonly IContextMenu contextMenu;

        public ContextMenuManager(ILogService logService, GameActions gameActions, IContextMenu contextMenu)
        {
            this.logService = logService;
            this.gameActions = gameActions;
            this.contextMenu = contextMenu;
            contextMenu.OnMenuOpened += OpenContextMenu;
        }

        public void Dispose()
        {
            contextMenu.OnMenuOpened -= OpenContextMenu;
        }
        private void OpenContextMenu(IMenuOpenedArgs args)
        {
            logService.Warning("OnOpenContextMenu");
            if (ValidAddons.Contains(args.AddonName) && args.Target is MenuTargetDefault def 
                && def.TargetName != null && ExcelWorldHelper.Get(def.TargetHomeWorld.RowId, true) != null)
            {
                string worldName = ExcelWorldHelper.Get(def.TargetHomeWorld.RowId, true)!.Value.Name.ToString();
                args.AddMenuItem(new()
                {
                    OnClicked = (args) =>
                    {
                        gameActions.AddPlayerByName($"{def.TargetName}@{worldName}");
                    },
                    PrefixChar = 'C',
                    Priority = 420,
                    Name = "Add to CaH"
                });
            }
        }
    }
}

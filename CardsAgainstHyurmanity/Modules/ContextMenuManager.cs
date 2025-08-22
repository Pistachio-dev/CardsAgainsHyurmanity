using CardsAgainstHyurmanity.Model.Game;
using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Plugin.Services;
using DalamudBasics.Logging;
using ECommons.ExcelServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly CahGame cahGame;
        private readonly IChatGui chatGui;

        public ContextMenuManager(ILogService logService, GameActions gameActions, IContextMenu contextMenu, CahGame cahGame, IChatGui chatGui)
        {
            this.logService = logService;
            this.gameActions = gameActions;
            this.contextMenu = contextMenu;
            this.cahGame = cahGame;
            this.chatGui = chatGui;
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

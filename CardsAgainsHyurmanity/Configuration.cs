using CardsAgainsHyurmanity.Model.CAHData;
using Dalamud.Game.Text;
using DalamudBasics.Configuration;
using System;
using System.Collections.Generic;

namespace CardsAgainsHyurmanity;

[Serializable]
public class Configuration : IConfiguration
{
    public int Version { get; set; } = 0;
    public XivChatType DefaultOutputChatType { get; set; } = XivChatType.Party;
    public bool LogOutgoingChatOutput { get; set; } = true;
    public bool LogClientOnlyChatOutput { get; set; } = true;
    public int LimitedChatChannelsMessageDelayInMs { get; set; } = 1500;

    public List<CahPackSettings> PackSelections { get; set; } = new();
    public int InitialWhiteCardsDrawnAmount { get; set; } = 10;
    public int AwesomePointsToWin { get; set; } = 10;
}

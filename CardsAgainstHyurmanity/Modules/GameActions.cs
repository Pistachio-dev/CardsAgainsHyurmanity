using CardsAgainstHyurmanity.Model.Game;
using CardsAgainstHyurmanity.Modules.DataLoader;
using Dalamud.Game.Text;
using DalamudBasics.Chat.ClientOnlyDisplay;
using DalamudBasics.Chat.Listener;
using DalamudBasics.Configuration;
using DalamudBasics.Extensions;
using DalamudBasics.Logging;
using DalamudBasics.Targeting;
using ECommons;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardsAgainstHyurmanity.Modules
{
    public class GameActions
    {
        private const string bloodSacrificeText = "#BloodSacrifice";
        private readonly CahGame game;
        private readonly CahDataLoader loader;
        private readonly CahChatOutput chatOutput;
        private readonly ILogService logService;
        private readonly Configuration configuration;
        private readonly ITargetingService targetingService;
        private readonly IClientChatGui clientChatGui;
        private readonly IChatListener chatListener;

        public GameActions(CahGame game, CahDataLoader loader, IConfigurationService<Configuration> configService, CahChatOutput chatOutput,
            ILogService logService, ITargetingService targetingService, IClientChatGui clientChatGui, IChatListener chatListener)
        {
            this.game = game;
            this.loader = loader;
            this.chatOutput = chatOutput;
            this.logService = logService;
            this.configuration = configService.GetConfiguration();
            this.targetingService = targetingService;
            this.clientChatGui = clientChatGui;
            this.chatListener = chatListener;
        }

        public void ReloadDeck()
        {
            game.Deck = loader.RandomizeDeck(loader.BuildDeck(configuration.PackSelections));
        }

        public void RemovePlayer(Player player)
        {
            game.Players.Remove(player);
        }

        public void StartGame()
        {
            foreach (var player in game.Players)
            {
                player.AwesomePoints = 0;
            }

            if (game.Deck == null)
            {
                game.Deck = loader.BuildDeck(configuration.PackSelections);
            }

            loader.RandomizeDeck(game.Deck);
            SetOrAdvanceTzar();

            foreach (var player in game.Players)
            {
                player.WhiteCards = game.Deck.DrawWhite(configuration.InitialWhiteCardsDrawnAmount);
                player.Picks.Clear();
                SendWhiteCardsOrTzarNotice(player);
            }

            DrawNewBlackCard();
            PresentBlackCard();
            game.Stage = GameStage.PlayersPicking;
        }

        private void SendWhiteCardsOrTzarNotice(Player player)
        {
            if (player == game.Tzar)
            {
                chatOutput.TellYouAreTzar(player);
            }
            else
            {
                chatOutput.TellPlayerWhiteCards(player);
            }
        }

        private void DrawNewBlackCard()
        {
            game.BlackCard = game.Deck.DrawBlack(1)[0];
        }

        public void NextRound()
        {
            SetOrAdvanceTzar();
            foreach (var player in game.Players)
            {
                player.WhiteCards.AddRange(game.Deck.DrawWhite(player.Picks.Count));
                player.Picks.Clear();
                player.AssignedNumberForTzarPick = 0;
                SendWhiteCardsOrTzarNotice(player);
            }
            
            DrawNewBlackCard();
            PresentBlackCard();
            game.Stage = GameStage.PlayersPicking;
        }

        public void EndGame()
        {
            game.ResetButKeepPlayers();
        }

        private void RedealCards(Player player)
        {
            if (player.AwesomePoints == 0)
            {
                chatOutput.WriteChat($"The dark lord looks upon {player.FullName.GetFirstName()}'s lack of Awesome Points in disgust. Your sacrifice was not accepted.");
                return;
            }
            player.AwesomePoints -= 1;
            int currentCardAmount = player.WhiteCards.Count();
            player.WhiteCards.Clear();
            player.WhiteCards.AddRange(game.Deck.DrawWhite(currentCardAmount));

            chatOutput.WriteChat($"The dark lord looks upon {player.FullName.GetFirstName()}'s offering, satisfied, and waves their hand. Your sacrifice grants them a new set of white cards.");
            chatOutput.TellPlayerWhiteCards(player);
        }

        private void PresentBlackCard()
        {
            chatOutput.WriteChat($"Black card: {game.BlackCard.text}.");
            chatOutput.WriteChat($"Pick {"white card".ToQuantity(game.BlackCard.pick)} by writing their number");
            if (game.BlackCard.pick > 1)
            {
                chatOutput.WriteChat("Use a comma (,) to separate the card numbers.");
            }
            if (new Random().Next(100) <= 5)
            {
                chatOutput.WriteChat($"Hey, you. You can type {bloodSacrificeText} to sacrifice an Awesome Point to the dark lord and receive a new set of white cards. But you didn't hear it from me.");
            }
        }

        public void PresentPicks()
        {
            chatOutput.WriteChat("Everyone picked, let's see what they made.");
            game.Stage = GameStage.TzarPicking;
            var randomizedList = game.GetNonTzarActivePlayers();
            new Random().Shuffle<Player>(randomizedList);

            int playerNumber = 1;
            bool firstLine = true;
            foreach (var player in randomizedList)
            {
                var response = GetPlayerResponse(player);

                player.AssignedNumberForTzarPick = playerNumber;

                chatOutput.WriteChat($"({playerNumber}) {response} <se.14>", null, firstLine ? 1000 : configuration.AnswersRolloutDelayInMs);
                firstLine = false;

                playerNumber++;
            }

            chatOutput.WriteChat($"{game.Tzar.FullName.GetFirstName()}, write the number of your favorite", null, configuration.AnswersRolloutDelayInMs);
        }

        private string GetPlayerResponse(Player player)
        {
            var response = game.BlackCard.text;
            foreach (var pick in player.Picks)
            {
                if (response.Contains("_"))
                {
                    response = response.ReplaceFirst("_", $"{pick}");
                }
                else
                {
                    response = $"{response} {pick}";
                }
            }

            return response;
        }

        public void AddPlayerByName(string fullName)
        {

            AddPlayer(fullName);
        }

        public void AddTargetPlayer()
        {
            if (!targetingService.TrySaveTargetPlayerReference(out var targetReference) || targetReference == null)
            {
                clientChatGui.Print("Could not add target to the players. It's either nothing or not a player.");
                return;
            }

            var targetFullName = targetReference.GetFullName();
            AddPlayer(targetFullName);
        }

        private void AddPlayer(string fullName)
        {
            if (game.Players.Any(p => p.FullName == fullName))
            {
                clientChatGui.Print("Target player is already in the game.");
                return;
            }

            var player = new Player() { FullName = fullName };
            if (game.Stage != GameStage.NotStarted)
            {
                player.WhiteCards.AddRange(game.Deck.DrawWhite(configuration.InitialWhiteCardsDrawnAmount));
            }

            game.Players.Add(player);

            chatOutput.WriteChat($"{fullName} joins the game.");

            return;
        }

        public void WritePlayerScores()
        {
            foreach (var player in game.Players)
            {
                chatOutput.WriteChat($"{player.FullName.WithoutWorldName()}: {player.AwesomePoints} A-points");
            }
        }

        public void AddChatListeners()
        {
            chatListener.AddPreprocessedMessageListener(PlayerPicksChatListener);
            chatListener.AddPreprocessedMessageListener(TzarPickChatListener);
            chatListener.AddPreprocessedMessageListener(BloodSacrificeChatListener);
        }

        public void ToggleAFK(Player player)
        {
            player.AFK = !player.AFK;
            chatOutput.WriteChat($"{player.FullName} is {(player.AFK ? "gone, but not forgotten (AFK)" : "back. Maybe with a vengeance (no longer AFK)")}");
        }

        private bool HaveAllPlayersPicked()
        {
            return !game.GetNonTzarActivePlayers().Any(p => !p.Picks.Any());
        }

        private void ApplyPlayerPick(int[] numbersPicked, Player player)
        {
            player.Picks.Clear();
            List<string> whiteCardsToRemove = new();
            foreach (var number in numbersPicked)
            {
                var pick = player.WhiteCards[number - 1];
                player.Picks.Add(pick);
                whiteCardsToRemove.Add(pick);
            }

            foreach (var card in whiteCardsToRemove)
            {
                player.WhiteCards.Remove(card);
            }

            chatOutput.WriteChat($"{player.FullName.GetFirstName()} picked");
        }

        private void SetOrAdvanceTzar()
        {
            if (!game.Players.Any(p => !p.AFK))
            {
                clientChatGui.Print("Everyone is AFK! Unmarking them!");
                foreach (var player in game.Players)
                {
                    player.AFK = false;
                }
                return;
            }

            int tzarIndex;
            if (game.Tzar == null)
            {
                tzarIndex = new Random().Next(0, game.Players.Count);
            }
            else
            {
                chatOutput.WriteCommand("mk off", 1000, game.Tzar.FullName);
                tzarIndex = GetNextTzarIndex();
            }

            game.Tzar = game.Players[tzarIndex];
            while (game.Tzar.AFK)
            {
                game.Tzar = game.Players[GetNextTzarIndex()];
            }

            chatOutput.WriteCommand("mk attack1", 1000, game.Tzar.FullName);
            chatOutput.WriteChat($"{game.Tzar.FullName.WithoutWorldName()} is the card Tzar.");
        }

        private int GetNextTzarIndex()
        {
            return (game.Players.IndexOf(game.Tzar) + 1) % game.Players.Count;
        }

        private void ApplyTzarPick(int number)
        {
            var winner = game.Players.FirstOrDefault(player => player.AssignedNumberForTzarPick == number);
            if (winner == null)
            {
                logService.Error($"No player with assigned number {number} was found");
                return;
            }

            winner.AwesomePoints += 1;

            if (winner.AwesomePoints == configuration.AwesomePointsToWin)
            {
                FinishGame(winner);
                return;
            }

            chatOutput.WriteChat($"{winner.FullName.WithoutWorldName()} wins and gets one Awesome point! <se.15>");
            chatOutput.WriteChat($"Their answer was {GetPlayerResponse(winner)}", null, 1000);
            NextRound();
        }

        private void FinishGame(Player winner)
        {
            chatOutput.WriteChat($"{winner.FullName} wins! <se.15>");
            game.Stage = GameStage.NotStarted;
        }

        private void TzarPickChatListener(XivChatType type, string senderFullName, string message, DateTime receivedAt)
        {
            if (game.Stage == GameStage.TzarPicking)
            {
                if (senderFullName != game.Tzar.FullName)
                {
                    return;
                }

                if (!int.TryParse(message.Trim(), out int pick) || pick < 1 || pick > game.Players.Count - 1)
                {
                    logService.Info($"Picking for Tzar {game.Tzar.FullName} is invalid: can't be parsed or it's out of range.");
                    return;
                }

                ApplyTzarPick(pick);
            }
        }

        private void BloodSacrificeChatListener(XivChatType type, string senderFullName, string message, DateTime receivedAt)
        {
            if (message.Trim().Equals(bloodSacrificeText, StringComparison.OrdinalIgnoreCase))
            {
                var player = game.Players.Find(p => p.FullName == senderFullName);
                if (player == null)
                {
                    logService.Info($"Blood sacrifice received for {senderFullName}, but it's not on the player list.");
                    return;
                }

                logService.Info($"Blood sacrifice received for {senderFullName}.");
                RedealCards(player);
            }
        }

        private void PlayerPicksChatListener(XivChatType type, string senderFullName, string message, DateTime receivedAt)
        {
            if (game.Stage == GameStage.PlayersPicking)
            {
                var pickingPlayer = game.GetNonTzarActivePlayers().FirstOrDefault(p => p.FullName == senderFullName);
                if (pickingPlayer == null)
                {
                    return;
                }

                if (pickingPlayer.Picks.Any())
                {
                    logService.Info($"Skipping picking for player {pickingPlayer.FullName}, they already picked.");
                    return;
                }

                var numbers = message.Split(",").Select(m => m.Trim()).ToArray();
                if (numbers.Length != game.BlackCard.pick)
                {
                    logService.Info($"Picking for player {pickingPlayer.FullName} is invalid, expected {game.BlackCard.pick} numbers, got {numbers.Length}");
                    return;
                }

                int[] choices = new int[game.BlackCard.pick];
                for (int i = 0; i < numbers.Length; i++)
                {
                    var number = numbers[i];
                    if (!int.TryParse(number, out int choice))
                    {
                        logService.Info($"Picking for player {pickingPlayer.FullName} is invalid, can't parse {number}");
                        return;
                    }
                    if (choice < 1 || choice > pickingPlayer.WhiteCards.Count)
                    {
                        logService.Info($"Picking for player {pickingPlayer.FullName} is invalid, {number} is not in the white card list.");
                        return;
                    }

                    choices[i] = choice;
                }

                logService.Info($"Pick detected for {senderFullName} from message \"{message}\"");
                ApplyPlayerPick(choices, pickingPlayer);

                if (HaveAllPlayersPicked())
                {
                    PresentPicks();
                }
            }
        }
    }
}

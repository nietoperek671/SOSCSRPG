using Engine.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Engine.Models;
using Engine.Factories;

namespace Engine.Services
{
    public static class SaveGameService
    {
        private const string SAVE_GAME_FILE = "SOSCSRPG.json";

        public static void Save (GameSession gamesession)
        {
            File.WriteAllText(SAVE_GAME_FILE, JsonSerializer.Serialize(gamesession));
        }

        public static GameSession LoadLastSaveOrCreateNew()
        {
            if (!File.Exists(SAVE_GAME_FILE))
            {
                return new GameSession();
            }

            try
            {
                using (JsonDocument document = JsonDocument.Parse(File.ReadAllText(SAVE_GAME_FILE)))
                {
                    Player player = CreatePlayer(document);

                    int x = document.RootElement
                        .GetProperty(nameof(GameSession.CurrentLocation))
                        .GetProperty(nameof(Location.XCoordinate))
                        .GetInt32();
                    int y = document.RootElement
                        .GetProperty(nameof(GameSession.CurrentLocation))
                        .GetProperty(nameof(Location.YCoordinate))
                        .GetInt32();

                    return new GameSession(player, x, y);
                }
            }
            catch (Exception)
            {
                return new GameSession();
            }
        }

        private static Player CreatePlayer(JsonDocument document)
        {
            string fileVersion = FileVersion(document);

            Player player;

            switch (fileVersion)
            {
                case "0.1.000":
                    player =
                        new Player(
                            document.RootElement.GetProperty(nameof(GameSession.CurrentPlayer)).GetProperty(nameof(Player.Name)).GetString(),
                            document.RootElement.GetProperty(nameof(GameSession.CurrentPlayer)).GetProperty(nameof(Player.CharacterClass)).GetString(),
                            document.RootElement.GetProperty(nameof(GameSession.CurrentPlayer)).GetProperty(nameof(Player.ExperiencePoints)).GetInt32(),
                            document.RootElement.GetProperty(nameof(GameSession.CurrentPlayer)).GetProperty(nameof(Player.MaximumHitPoints)).GetInt32(),
                            document.RootElement.GetProperty(nameof(GameSession.CurrentPlayer)).GetProperty(nameof(Player.CurrentHitPoints)).GetInt32(),
                            document.RootElement.GetProperty(nameof(GameSession.CurrentPlayer)).GetProperty(nameof(Player.Dexterity)).GetInt32(),
                            document.RootElement.GetProperty(nameof(GameSession.CurrentPlayer)).GetProperty(nameof(Player.Gold)).GetInt32());
                    break;

                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized.");
            }

            PopulatePlayerInventory(document, player);
            PopulatePlayerQuests(document, player);
            PopulatePlayerRecipes(document, player);

            return player;
        }

        private static void PopulatePlayerRecipes(JsonDocument document, Player player)
        {
            string fileVersion = FileVersion(document);

            switch (fileVersion)
            {
                case "0.1.000":
                    foreach (var recipeToken in
                        document.RootElement
                        .GetProperty(nameof(GameSession.CurrentPlayer))
                        .GetProperty(nameof(Player.Recipes))
                        .EnumerateArray())
                    {
                        int recipeId = recipeToken.GetProperty(nameof(Recipe.ID)).GetInt32();

                        Recipe recipe = RecipeFactory.RecipeByID(recipeId);

                        player.Recipes.Add(recipe);
                    }

                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }
        }

        private static void PopulatePlayerQuests(JsonDocument document, Player player)
        {
            string fileVersion = FileVersion(document);

            switch (fileVersion)
            {
                case "0.1.000":
                    foreach (var jQuest in document.RootElement
                        .GetProperty(nameof(GameSession.CurrentPlayer))
                        .GetProperty(nameof(Player.Quests))
                        .EnumerateArray())
                    {
                        int questId = jQuest.GetProperty(nameof(QuestStatus.PlayerQuest)).GetProperty(nameof(QuestStatus.PlayerQuest.ID)).GetInt32();

                        Quest quest = QuestFactory.GetQuestByID(questId);
                        QuestStatus questStatus = new QuestStatus(quest);
                        questStatus.IsCompleted = jQuest.GetProperty(nameof(QuestStatus.IsCompleted)).GetBoolean();

                        player.Quests.Add(questStatus);
                    }
                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized.");
            }
        }

        private static void PopulatePlayerInventory(JsonDocument document, Player player)
        {
            string fileVersion = FileVersion(document);

            switch (fileVersion)
            {
                case "0.1.000":
                    foreach (var item in document.RootElement
                        .GetProperty(nameof(GameSession.CurrentPlayer))
                        .GetProperty(nameof(Player.Inventory))
                        .GetProperty(nameof(Inventory.Items))
                        .EnumerateArray())
                    {
                        int itemId = item.GetProperty(nameof(GameItem.ItemTypeID)).GetInt32();
                        player.AddItemToInventory(ItemFactory.CreateGameItem(itemId));
                    }
                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized.");
            }
        }

        private static string FileVersion(JsonDocument document)
        {
            return document.RootElement.GetProperty(nameof(GameSession.Version)).GetString();
        }
    }
}

using System.IO;
using System.Xml;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories
{
    internal static class WorldFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Locations.xml";

        internal static World CreateWorld()
        {
            var newWorld = new World();

            if (File.Exists(GAME_DATA_FILENAME))
            {
                var data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                var rootImgaePath =
                    data.SelectSingleNode("/Locations")
                        .AttributeAsString("RootImagePath");

                LoadLocationsFromNodes(newWorld, rootImgaePath, data.SelectNodes("/Locations/Location"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }

            return newWorld;
        }

        private static void LoadLocationsFromNodes(World newWorld, string rootImgaePath, XmlNodeList selectNodes)
        {
            if (selectNodes == null)
            {
                return;
            }

            foreach (XmlNode node in selectNodes)
            {
                var location =
                    new Location(node.AttributeAsInt("X"),
                        node.AttributeAsInt("Y"),
                        node.AttributeAsString("Name"),
                        node.SelectSingleNode("./Description")?.InnerText ?? "",
                        $".{rootImgaePath}{node.AttributeAsString("ImageName")}");

                AddMonsters(location, node.SelectNodes("./Monsters/Monster"));
                AddQuests(location, node.SelectNodes("./Quests/Quest"));
                AddTrader(location, node.SelectNodes("./Trader"));

                newWorld.AddLocation(location);
            }
        }

        private static void AddTrader(Location location, XmlNodeList traderNodes)
        {
            if (traderNodes == null)
            {
                return;
            }

            foreach (XmlNode traderNode in traderNodes)
            {
                location.TraderHere =
                    TraderFactory.GetTraderByID(traderNode.AttributeAsInt("ID"));
            }
        }

        private static void AddQuests(Location location, XmlNodeList questNodes)
        {
            if (questNodes == null)
            {
                return;
            }

            foreach (XmlNode questNode in questNodes)
            {
                location.QuestAvailableHere.Add(QuestFactory.GetQuestByID(questNode.AttributeAsInt("ID")));
            }
        }

        private static void AddMonsters(Location location, XmlNodeList monstersList)
        {
            if (monstersList == null)
            {
                return;
            }

            foreach (XmlNode MonsterNode in monstersList)
            {
                location.AddMonster(MonsterNode.AttributeAsInt("ID"),
                    MonsterNode.AttributeAsInt("Percent"));
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories
{
    public static class MonsterFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Monsters.xml";
        private static readonly List<Monster> _baseMonsters = new List<Monster>();

        static MonsterFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                var data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                var rootImagePath = data.SelectSingleNode("/Monsters")
                    .AttributeAsString("RootImagePath");

                LoadMonstersFromNodes(data.SelectNodes("/Monsters/Monster"), rootImagePath);
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadMonstersFromNodes(XmlNodeList nodes, string rootImagePath)
        {
            if (nodes == null)
            {
                return;
            }

            foreach (XmlNode xmlNode in nodes)
            {
                var monster = new Monster(xmlNode.AttributeAsInt("ID"),
                    xmlNode.AttributeAsString("Name"),
                    $"{rootImagePath}{xmlNode.AttributeAsString("ImageName")}",
                    xmlNode.AttributeAsInt("MaximumHitPoints"),
                    ItemFactory.CreateGameItem(xmlNode.AttributeAsInt("WeaponID")),
                    xmlNode.AttributeAsInt("RewardXP"),
                    xmlNode.AttributeAsInt("Gold")
                );

                var lootItemNodes = xmlNode.SelectNodes("./LootItems/LootItem");
                if (lootItemNodes != null)
                {
                    foreach (XmlNode lootItemNode in lootItemNodes)
                        monster.AddItemToLootTable(lootItemNode.AttributeAsInt("ID"),
                            lootItemNode.AttributeAsInt("Percentage"));
                }

                _baseMonsters.Add(monster);
            }
        }

        public static Monster GetMonster(int id)
        {
            return _baseMonsters.FirstOrDefault(m => m.ID == id)?.GetNewInstance();
        }
    }
}
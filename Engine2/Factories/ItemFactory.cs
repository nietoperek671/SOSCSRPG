using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Engine.Actions;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories
{
    public static class ItemFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\GameItems.xml";
        private static readonly List<GameItem> _standardGameItems = new List<GameItem>();

        static ItemFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                var data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadItemsFromNodes(data.SelectNodes("/GameItems/Weapons/Weapon"));
                LoadItemsFromNodes(data.SelectNodes("/GameItems/Scrolls/Scroll"));
                LoadItemsFromNodes(data.SelectNodes("/GameItems/HealingItems/HealingItem"));
                LoadItemsFromNodes(data.SelectNodes("/GameItems/MiscellaneousItems/MiscellaneousItem"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadItemsFromNodes(XmlNodeList xmlNodeList)
        {
            if (xmlNodeList == null)
            {
                return;
            }

            foreach (XmlNode node in xmlNodeList)
            {
                var itemCategory = DetermineItemCategory(node.Name);

                var gameItem =
                    new GameItem(itemCategory,
                        node.AttributeAsInt("ID"),
                        node.AttributeAsString("Name"),
                        node.AttributeAsInt("Price"),
                        GameItem.ItemCategory.Weapon == itemCategory);

                switch (itemCategory)
                {
                    case GameItem.ItemCategory.Weapon:
                        gameItem.Action =
                            new AttackWithWeapon(gameItem,
                                node.AttributeAsInt("MinimumDamage"),
                                node.AttributeAsInt("MaximumDamage"));
                        break;
                    case GameItem.ItemCategory.Consumable:
                        gameItem.Action =
                            new Heal(gameItem,
                                node.AttributeAsInt("HitPointsToHeal"));
                        break;
                    case GameItem.ItemCategory.AttackScroll:
                        gameItem.Action =
                            new AttackWithScroll(gameItem,
                                node.AttributeAsInt("Damage"));
                        break;
                }

                _standardGameItems.Add(gameItem);
            }
        }

        private static string GetXmlAttribute(XmlNode node, string attributeName)
        {
            var attribute = node.Attributes?[attributeName];

            if (attribute == null)
            {
                throw new ArgumentException($"The attribute '{attributeName}' does not exist");
            }

            return attribute.Value;
        }

        private static GameItem.ItemCategory DetermineItemCategory(string itemType)
        {
            switch (itemType)
            {
                case "Weapon":
                    return GameItem.ItemCategory.Weapon;
                case "HealingItem":
                    return GameItem.ItemCategory.Consumable;
                case "Scroll":
                    return GameItem.ItemCategory.AttackScroll;
                default:
                    return GameItem.ItemCategory.Miscellaneous;
            }
        }

        public static GameItem CreateGameItem(int itemTypeID)
        {
            return _standardGameItems.FirstOrDefault(item => item.ItemTypeID == itemTypeID)?.Clone();
        }

        public static string ItemName(int itemTypeID)
        {
            return _standardGameItems.FirstOrDefault(i => i.ItemTypeID == itemTypeID)?.Name ?? "";
        }
    }
}
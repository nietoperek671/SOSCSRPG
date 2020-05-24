using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Engine.Actions;
using Engine.Models;

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
                        GetXmlAttributeAsInt(node, "ID"),
                        GetXmlAttributeAsString(node, "Name"),
                        GetXmlAttributeAsInt(node, "Price"),
                        GameItem.ItemCategory.Weapon == itemCategory);

                switch (itemCategory)
                {
                    case GameItem.ItemCategory.Weapon:
                        gameItem.Action =
                            new AttackWithWeapon(gameItem,
                                GetXmlAttributeAsInt(node, "MinimumDamage"),
                                GetXmlAttributeAsInt(node, "MaximumDamage"));
                        break;
                    case GameItem.ItemCategory.Consumable:
                        gameItem.Action =
                            new Heal(gameItem,
                                GetXmlAttributeAsInt(node, "HitPointsToHeal"));
                        break;
                    case GameItem.ItemCategory.AttackScroll:
                        gameItem.Action =
                            new AttackWithScroll(gameItem,
                                GetXmlAttributeAsInt(node, "Damage"));
                        break;
                }

                _standardGameItems.Add(gameItem);
            }
        }

        private static string GetXmlAttributeAsString(XmlNode node, string v)
        {
            return GetXmlAttribute(node, v);
        }

        private static int GetXmlAttributeAsInt(XmlNode node, string v)
        {
            return Convert.ToInt32(GetXmlAttribute(node, v));
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
using Engine.Actions;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

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
                XmlDocument data = new XmlDocument();
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
                GameItem.ItemCategory itemCategory = DetermineItemCategory(node.Name);

                GameItem gameItem =
                    new GameItem(itemCategory,
                    GetXmlAttributeAsInt(node, "ID"),
                    GetXmlAttributeAsString(node, "Name"),
                    GetXmlAttributeAsInt(node, "Price"),
                    itemCategory==GameItem.ItemCategory.Weapon);

                if (itemCategory == GameItem.ItemCategory.Weapon)
                {
                    gameItem.Action =
                        new AttackWithWeapon(gameItem,
                        GetXmlAttributeAsInt(node, "MinimumDamage"),
                        GetXmlAttributeAsInt(node, "MaximumDamage"));
                }
                else if (itemCategory==GameItem.ItemCategory.Consumable)
                {
                    gameItem.Action =
                        new Heal(gameItem,
                        GetXmlAttributeAsInt(node, "HitPointsToHeal"));
                }
                else if (itemCategory==GameItem.ItemCategory.AttackScroll)
                {
                    gameItem.Action =
                        new AttackWithScroll(gameItem,
                        GetXmlAttributeAsInt(node, "Damage"));
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
            XmlAttribute attribute = node.Attributes?[attributeName];

            if (attribute==null)
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

        private static void BuildHealingItem(int id, string name, int price, int pointsToHeal)
        {
            GameItem item = new GameItem(GameItem.ItemCategory.Consumable, id, name, price);

            item.Action = new Heal(item, pointsToHeal);

            _standardGameItems.Add(item);
        }

        private static void BuildMiscellaneousItem(int id, string name, int price)
        {
            _standardGameItems.Add(new GameItem(GameItem.ItemCategory.Miscellaneous, id, name, price));
        }

        private static void BuildWeapon(int id, string name, int price, int minimuDamage, int maximumDamage)
        {
            GameItem weapon = new GameItem(GameItem.ItemCategory.Weapon, id, name, price, true);

            weapon.Action = new AttackWithWeapon(weapon, minimuDamage, maximumDamage);

            _standardGameItems.Add(weapon);
        }

        private static void BuildScroll(int id, string name, int price, int damage=0)
        {
            GameItem scroll = new GameItem(GameItem.ItemCategory.AttackScroll, id, name, price);

            scroll.Action = new AttackWithScroll(scroll, damage);

            _standardGameItems.Add(scroll);
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

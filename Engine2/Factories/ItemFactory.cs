﻿using Engine.Actions;
using Engine.Models;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Factories
{
    public static class ItemFactory
    {
        private static readonly List<GameItem> _standardGameItems = new List<GameItem>();

        static ItemFactory()
        {
            BuildWeapon(1001, "Pointy Stick", 1, 1, 2);
            BuildWeapon(1002, "Rusty Sword", 5, 1, 3);

            BuildWeapon(1501, "Snake Fangs", 0, 0, 2);
            BuildWeapon(1502, "Rat Claws", 0, 0, 2);
            BuildWeapon(1503, "Spider Fangs", 0, 0, 4);

            BuildHealingItem(2001, "Granola bar", 5, 2);

            BuildMiscellaneousItem(3001, "Oats", 1);
            BuildMiscellaneousItem(3002, "Honey", 2);
            BuildMiscellaneousItem(3003, "Raisins", 2);

            BuildScroll(4001, "Fireball", 10, 10);
            BuildScroll(4002, "Ice spike", 10, 10);
            BuildScroll(4003, "Blizzard", 50, 100);
            BuildScroll(4004, "Mateor", 50, 100);

            BuildMiscellaneousItem(9001, "Snake Fang", 1);
            BuildMiscellaneousItem(9002, "Snakeskin", 2);
            BuildMiscellaneousItem(9003, "Rat tail", 1);
            BuildMiscellaneousItem(9004, "Rat fur", 2);
            BuildMiscellaneousItem(9005, "Spider fang", 1);
            BuildMiscellaneousItem(9006, "Spider silk", 2);
            BuildMiscellaneousItem(9006, "Spider silk", 2);
            BuildMiscellaneousItem(9006, "Spider silk", 2);
            BuildMiscellaneousItem(9006, "Spider silk", 2);
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

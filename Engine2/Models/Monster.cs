﻿using System.Collections.Generic;
using Engine.Factories;

namespace Engine.Models
{
    public class Monster : LivingEntity
    {
        private readonly List<ItemPercentage> _lootTable =
            new List<ItemPercentage>();

        public Monster(int id, string name, string imageName,
            int maximumHitPoints,
            GameItem currentWeapon,
            int rewardExperiencePoints, int gold) :
            base(name, maximumHitPoints, maximumHitPoints, gold)
        {
            ID = id;
            ImageName = imageName;
            CurrentWeapon = currentWeapon;
            RewardExperiencePoints = rewardExperiencePoints;
        }

        public int ID { get; }
        public string ImageName { get; }
        public int RewardExperiencePoints { get; }

        public void AddItemToLootTable(int id, int percentage)
        {
            _lootTable.RemoveAll(itemPercentage => itemPercentage.ID == id);

            _lootTable.Add(new ItemPercentage(id, percentage));
        }

        public Monster GetNewInstance()
        {
            var newMonster = new Monster(ID, Name, ImageName, MaximumHitPoints, CurrentWeapon, RewardExperiencePoints,
                Gold);

            foreach (var itemPercentage in _lootTable)
            {
                newMonster.AddItemToLootTable(itemPercentage.ID, itemPercentage.Percentage);

                if (RandomNumberGenerator.NumberBetween(1, 100) <= itemPercentage.Percentage)
                {
                    newMonster.AddItemToInventory(ItemFactory.CreateGameItem(itemPercentage.ID));
                }
            }

            return newMonster;
        }
    }
}
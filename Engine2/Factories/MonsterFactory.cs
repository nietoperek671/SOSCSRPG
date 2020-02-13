using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Factories
{
    public static class MonsterFactory
    {
        public static Monster GetMonster(int monsterID)
        {
            switch (monsterID)
            {
                case 1:
                    Monster snake = new Monster("Snake", "Snake.png", 
                        maximumHitPoints: 4, hitPoints: 4, 
                        minimumDamage: 1, maximumDamage: 2, 
                        rewardExperiencePoints: 5, rewardGold: 1);
                    
                    AddLootItem(snake, itemID: 9001, percentage: 25);
                    AddLootItem(snake, itemID: 9002, percentage: 75);

                    return snake;

                case 2:
                    Monster rat = new Monster("Rat", "Rat.png", 
                        maximumHitPoints: 4, hitPoints: 4, 
                        minimumDamage: 1, maximumDamage: 1, 
                        rewardExperiencePoints: 5, rewardGold: 1);
                    
                    AddLootItem(rat, itemID: 9003, percentage: 25);
                    AddLootItem(rat, itemID: 9004, percentage: 75);

                    return rat;

                case 3:
                    Monster giantSpider = new Monster("Giant Spider", "GiantSpider.png", 
                        maximumHitPoints: 4, hitPoints: 4, 
                        minimumDamage: 1, maximumDamage: 3, 
                        rewardExperiencePoints: 5, rewardGold: 1);
                    
                    AddLootItem(giantSpider, itemID: 9005, percentage: 25);
                    AddLootItem(giantSpider, itemID: 9006, percentage: 75);

                    return giantSpider;

                default:
                    throw new ArgumentException($"No such monster id: {monsterID}");
            }
        }

        private static void AddLootItem(Monster monster, int itemID, int percentage)
        {
            if (RandomNumberGenerator.NumberBetween(1, 100) <= percentage)
                monster.Inventory.Add(new ItemQuantity(itemID, 1));
        }
    }
}

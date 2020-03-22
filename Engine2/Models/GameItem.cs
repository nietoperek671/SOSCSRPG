using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GameItem
    {
        public enum ItemCategory
        {
            Miscellaneous,
            Weapon
        }
        public ItemCategory Category { get; }
        public int ItemTypeID { get; }
        public string Name { get; }
        public int Price { get; }
        public bool IsUnique { get; }
        public int MinimumDamage { get; }
        public int MaximumDamage { get; }

        public GameItem(ItemCategory itemCategory, int itemTypeID, string name, int price, bool isUnique = false, int minimumDamage = 0, int maximumGamage = 0)
        {
            Category = itemCategory;
            ItemTypeID = itemTypeID;
            Name = name;
            Price = price;
            IsUnique = isUnique;
            MaximumDamage = maximumGamage;
            MinimumDamage = minimumDamage;
        }

        public GameItem Clone()
        {
            return new GameItem(Category, ItemTypeID, Name, Price, IsUnique, MinimumDamage, MaximumDamage);
        }
    }
}

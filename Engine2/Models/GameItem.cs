using System.Text.Json.Serialization;
using Engine.Actions;

namespace Engine.Models
{
    public class GameItem
    {
        public enum ItemCategory
        {
            Miscellaneous,
            Weapon,
            Consumable,
            AttackScroll
        }

        public GameItem(ItemCategory itemCategory, int itemTypeID, string name, int price, bool isUnique = false,
            IAction action = null)
        {
            Category = itemCategory;
            ItemTypeID = itemTypeID;
            Name = name;
            Price = price;
            IsUnique = isUnique;
            Action = action;
        }

        [JsonIgnore] public ItemCategory Category { get; }
        public int ItemTypeID { get; }
        [JsonIgnore] public string Name { get; }
        [JsonIgnore] public int Price { get; }
        [JsonIgnore] public bool IsUnique { get; }
        [JsonIgnore] public IAction Action { get; set; }

        public GameItem Clone() => new GameItem(Category, ItemTypeID, Name, Price, IsUnique, Action);

        public void PerformAction(LivingEntity actor, LivingEntity target)
        {
            Action?.Execute(actor, target);
        }
    }
}
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

        public ItemCategory Category { get; }
        public int ItemTypeID { get; }
        public string Name { get; }
        public int Price { get; }
        public bool IsUnique { get; }
        public IAction Action { get; set; }

        public GameItem Clone() => new GameItem(Category, ItemTypeID, Name, Price, IsUnique, Action);

        public void PerformAction(LivingEntity actor, LivingEntity target)
        {
            Action?.Execute(actor, target);
        }
    }
}
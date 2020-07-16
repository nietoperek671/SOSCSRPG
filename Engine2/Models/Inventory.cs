using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Engine.Services;

namespace Engine.Models
{
    public class Inventory
    {
        private readonly List<GroupedInventoryItem> _backingGroupedInventoryItems = new List<GroupedInventoryItem>();
        private readonly List<GameItem> _backingInventory = new List<GameItem>();

        public Inventory(IEnumerable<GameItem> items = null)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                _backingInventory.Add(item);

                AddItemToGroupedInventory(item);
            }
        }

        public IReadOnlyList<GameItem> Items =>
            _backingInventory.AsReadOnly();

        [JsonIgnore]
        public IReadOnlyList<GroupedInventoryItem> GroupedInventory =>
            _backingGroupedInventoryItems.AsReadOnly();

        [JsonIgnore]
        public IReadOnlyList<GameItem> Weapons =>
            _backingInventory.ItemsThatAre(GameItem.ItemCategory.Weapon).AsReadOnly();

        [JsonIgnore]
        public IReadOnlyList<GameItem> Consumables =>
            _backingInventory.ItemsThatAre(GameItem.ItemCategory.Consumable).AsReadOnly();

        [JsonIgnore]
        public IReadOnlyList<GameItem> Scrolls =>
            _backingInventory.ItemsThatAre(GameItem.ItemCategory.AttackScroll).AsReadOnly();

        [JsonIgnore] public bool HasConsumable => Consumables.Any();

        public bool HasAllTheseItems(IEnumerable<ItemQuantity> items)
        {
            return items.All(item => Items.Count(i => i.ItemTypeID == item.ItemID) >= item.Quantity);
        }

        private void AddItemToGroupedInventory(GameItem item)
        {
            if (item.IsUnique)
            {
                _backingGroupedInventoryItems.Add(new GroupedInventoryItem(item, 1));
            }
            else
            {
                if (_backingGroupedInventoryItems.All(gi => gi.Item.ItemTypeID != item.ItemTypeID))
                {
                    _backingGroupedInventoryItems.Add(new GroupedInventoryItem(item, 1));
                }
                else
                {
                    _backingGroupedInventoryItems.First(gi => gi.Item.ItemTypeID == item.ItemTypeID).Quantity++;
                }
            }
        }
    }
}
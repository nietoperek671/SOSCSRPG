﻿using System.Collections.Generic;
using System.Linq;
using Engine.Factories;
using Engine.Models;

namespace Engine.Services
{
    public static class InventoryService
    {
        public static Inventory AddItem(this Inventory inventory, GameItem item) =>
            inventory.AddItems(new List<GameItem> {item});

        public static Inventory AddItemFromFactory(this Inventory inventory, int itemTypeID) =>
            inventory.AddItems(new List<GameItem> {ItemFactory.CreateGameItem(itemTypeID)});

        public static Inventory AddItems(this Inventory inventory, IEnumerable<GameItem> items) =>
            new Inventory(inventory.Items.Concat(items));

        public static Inventory AddItems(this Inventory inventory, IEnumerable<ItemQuantity> itemQuantities)
        {
            var itemsToAdd = new List<GameItem>();

            foreach (var itemQuantity in itemQuantities)
            {
                for (var i = 0; i < itemQuantity.Quantity; i++)
                {
                    itemsToAdd.Add(ItemFactory.CreateGameItem(itemQuantity.ItemID));
                }
            }

            return inventory.AddItems(itemsToAdd);
        }

        public static Inventory RemoveItem(this Inventory inventory, GameItem item) =>
            inventory.RemoveItems(new List<GameItem> {item});

        public static Inventory RemoveItems(this Inventory inventory, IEnumerable<GameItem> items)
        {
            var workingInventory = inventory.Items.ToList();
            IEnumerable<GameItem> itemsToRemove = items.ToList();

            foreach (var item in itemsToRemove)
            {
                workingInventory.Remove(item);
            }

            return new Inventory(workingInventory);
        }

        public static Inventory RemoveItems(this Inventory inventory, IEnumerable<ItemQuantity> itemQuantities)
        {
            var workingInventory = inventory;

            foreach (var itemQuantity in itemQuantities)
            {
                for (var i = 0; i < itemQuantity.Quantity; i++)
                {
                    workingInventory =
                        workingInventory
                            .RemoveItem(workingInventory
                                .Items
                                .First(item => item.ItemTypeID == itemQuantity.ItemID));
                }
            }

            return workingInventory;
        }

        public static List<GameItem> ItemsThatAre(this IEnumerable<GameItem> inventory, GameItem.ItemCategory category)
        {
            return inventory.Where(i => i.Category == category).ToList();
        }
    }
}
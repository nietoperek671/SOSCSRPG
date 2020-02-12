﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Trader : BaseNotificationClass
    {
        public string Name { get; set; }
        public ObservableCollection<GameItem> Inventory { get; set; } = new ObservableCollection<GameItem>();
        public Trader(string name)
        {
            Name = name;
        }

        public void AddItemToInventory(GameItem item) => Inventory.Add(item);
        public void RemoveItem(GameItem item) => Inventory.Remove(item);
    }
}

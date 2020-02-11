﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Engine.Models
{
    public class Player : BaseNotificationClass
    {
        #region Properites
        private int _experiencePoints;
        private string _name;
        private string _characterClass;
        private int _hitPoints;
        private int _level;
        private int _gold;

        public string Name { 
            get => _name; 
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string CharacterClass { 
            get => _characterClass;
            set
            {
                _characterClass = value;
                OnPropertyChanged(nameof(CharacterClass));
            }
        }
        public int HitPoints { 
            get => _hitPoints; 
            set
            {
                _hitPoints = value;
                OnPropertyChanged(nameof(HitPoints));
            }
        }
        public int ExperiencePoints
        {
            get => _experiencePoints;
            set
            {
                _experiencePoints = value;
                OnPropertyChanged(nameof(ExperiencePoints));
            }
        }
        public int Level { 
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }
        public int Gold { 
            get => _gold;
            set
            {
                _gold = value;
                OnPropertyChanged(nameof(Gold));
            }
        }
        #endregion

        public ObservableCollection<GameItem> Inventory { get; set; }
        public List<GameItem> Weapons => Inventory.Where(o => o is Weapon).ToList();
        public ObservableCollection<QuestStatus> Quests { get; set; }

        public Player()
        {
            Inventory = new ObservableCollection<GameItem>();
            Quests = new ObservableCollection<QuestStatus>();
        }

        public void  AddItemToInventory(GameItem item)
        {
            Inventory.Add(item);

            OnPropertyChanged(nameof(Weapons));
        }
    }
}

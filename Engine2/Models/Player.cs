using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.Models
{
    public class Player : LivingEntity
    {
        #region Properites
        private int _experiencePoints;
        private string _characterClass;
        

        public string CharacterClass { 
            get => _characterClass;
            set
            {
                _characterClass = value;
                OnPropertyChanged(nameof(CharacterClass));
            }
        }
        public int ExperiencePoints
        {
            get => _experiencePoints;
            private set
            {
                _experiencePoints = value;

                OnPropertyChanged(nameof(ExperiencePoints));

                SetLevelAndMaximumHitPoints();
            }
        }
        
        #endregion

        public ObservableCollection<QuestStatus> Quests { get; set; }

        public event EventHandler OnLeveledUp;

        public Player(string name, string characterClass, int expPoints,
                      int maximumHitPoints, int currentHitPoints, int gold) 
            : base(name,maximumHitPoints,currentHitPoints,gold)
        {
            CharacterClass = characterClass;
            ExperiencePoints = expPoints;

            Quests = new ObservableCollection<QuestStatus>();
        }

        public bool HasAllTheseItems(List<ItemQuantity> items) => items.Any(item => Inventory.Count(i => i.ItemTypeID == item.ItemID) < item.Quantity) == false;

        public void AddExperience(int expPoints)
        {
            ExperiencePoints += expPoints;
        }

        private void SetLevelAndMaximumHitPoints()
        {
            int originalLevel = Level;

            Level = ExperiencePoints / 100 + 1;

            if (Level!= originalLevel)
            {
                MaximumHitPoints = Level * 10;

                OnLeveledUp?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }
}

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
        private int _level;

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
        #endregion

        public ObservableCollection<QuestStatus> Quests { get; set; }

        public Player()
        {
            Quests = new ObservableCollection<QuestStatus>();
        }

        public bool HasAllTheseItems(List<ItemQuantity>items) => items.Any(item => Inventory.Count(i => i.ItemTypeID == item.ItemID) < item.Quantity) == false  ;
    }
}

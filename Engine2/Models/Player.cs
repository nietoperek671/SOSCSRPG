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


        public string CharacterClass
        {
            get => _characterClass;
            set
            {
                _characterClass = value;
                OnPropertyChanged();
            }
        }
        public int ExperiencePoints
        {
            get => _experiencePoints;
            private set
            {
                _experiencePoints = value;

                OnPropertyChanged();

                SetLevelAndMaximumHitPoints();
            }
        }

        #endregion

        public ObservableCollection<QuestStatus> Quests { get; }
        public ObservableCollection<Recipe> Recipes { get; }
        public IList<GameItem> Scrolls => Inventory.Where(item => item.Category == GameItem.ItemCategory.AttackScroll).ToList();

        public event EventHandler OnLeveledUp;

        public Player(string name, string characterClass, int expPoints,
                      int maximumHitPoints, int currentHitPoints, int gold)
            : base(name, maximumHitPoints, currentHitPoints, gold)
        {
            CharacterClass = characterClass;
            ExperiencePoints = expPoints;

            Quests = new ObservableCollection<QuestStatus>();
            Recipes = new ObservableCollection<Recipe>();
        }

        public void AddExperience(int expPoints)
        {
            ExperiencePoints += expPoints;
        }

        public void LearnRecipe(Recipe recipe)
        {
            if (!Recipes.Any(r => r.ID == recipe.ID))
            {
                Recipes.Add(recipe);

            }
        }

        private void SetLevelAndMaximumHitPoints()
        {
            int originalLevel = Level;

            Level = ExperiencePoints / 100 + 1;

            if (Level != originalLevel)
            {
                MaximumHitPoints = Level * 10;

                OnLeveledUp?.Invoke(this, System.EventArgs.Empty);
            }
        }

        public override void AddItemToInventory(GameItem item)
        {
            base.AddItemToInventory(item);

            OnPropertyChanged(nameof(Scrolls));
        }
        public override void RemoveItemFromInventory(GameItem item)
        {
            base.RemoveItemFromInventory(item);
        
            OnPropertyChanged(nameof(Scrolls));
        }
    }
}

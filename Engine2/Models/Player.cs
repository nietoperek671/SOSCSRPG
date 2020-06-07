using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Engine.Models
{
    public class Player : LivingEntity
    {
        public Player(
            string name,
            string characterClass,
            int expPoints,
            int dexterity,
            int maximumHitPoints,
            int currentHitPoints,
            int gold)
            : base(name, maximumHitPoints, currentHitPoints, gold, dexterity)
        {
            CharacterClass = characterClass;
            ExperiencePoints = expPoints;

            Quests = new ObservableCollection<QuestStatus>();
            Recipes = new ObservableCollection<Recipe>();
        }

        public ObservableCollection<QuestStatus> Quests { get; }
        public ObservableCollection<Recipe> Recipes { get; }
        public event EventHandler OnLeveledUp;

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
    }
}
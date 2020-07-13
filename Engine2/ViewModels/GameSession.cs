using System;
using System.Linq;
using Engine.EventArgs;
using Engine.Factories;
using Engine.Models;
using Engine.Services;

namespace Engine.ViewModels
{
    public class
        GameSession : BaseNotificationClass
    {
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();
        private Battle _currentBattle;
        private Location _currentLocation;
        private Monster _currentMonster;
        private Player _currentPlayer;
        private Trader _currentTrader;

        public GameSession()
        {
            #region Create player

            var dex = RandomNumberGenerator.NumberBetween(3, 18);
            CurrentPlayer = new Player("Scott", "Fighter", 0, dex, 10, 10, 1000);

            #endregion

            if (CurrentPlayer.Inventory.Weapons.Any() == false)
            {
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            }

            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(2001));
            CurrentPlayer.LearnRecipe(RecipeFactory.RecipeByID(1));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(3001));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(3002));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(3003));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(4001));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(4002));

            CurrentWorld = WorldFactory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, -1);
        }

        public GameSession(Player player, int x, int y)
        {
            CurrentWorld = WorldFactory.CreateWorld();
            CurrentPlayer = player;
            CurrentLocation = CurrentWorld.LocationAt(x, y);
        }

        public string Version { get; } = "0.1.000";

        public World CurrentWorld { get; }

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnKilled -= OnPlayerKilled;
                    _currentPlayer.OnLeveledUp -= OnCurrentPlayerLeveledUp;
                }

                _currentPlayer = value;

                if (_currentPlayer != null)
                {
                    _currentPlayer.OnKilled += OnPlayerKilled;
                    _currentPlayer.OnLeveledUp += OnCurrentPlayerLeveledUp;
                }
            }
        }

        public Location CurrentLocation
        {
            get => _currentLocation;
            set
            {
                _currentLocation = value;

                OnPropertyChanged();

                OnPropertyChanged(nameof(HasLocationToEast));
                OnPropertyChanged(nameof(HasLocationToNorth));
                OnPropertyChanged(nameof(HasLocationToSouth));
                OnPropertyChanged(nameof(HasLocationToWest));

                CompleteQuestAtLocation();
                GivePlayerQuestsAtLocation();
                CurrentMonster = CurrentLocation.GetMonster();

                CurrentTrader = CurrentLocation.TraderHere;
            }
        }

        public Monster CurrentMonster
        {
            get => _currentMonster;
            set
            {
                if (_currentBattle != null)
                {
                    _currentBattle.OnCombatVictory -= OnCurrentMonsterKilled;
                    _currentBattle.Dispose();
                }

                _currentMonster = value;

                if (_currentMonster != null)
                {
                    _currentBattle = new Battle(CurrentPlayer, CurrentMonster);

                    _currentBattle.OnCombatVictory += OnCurrentMonsterKilled;
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasMonster));
            }
        }

        public Trader CurrentTrader
        {
            get => _currentTrader;
            set
            {
                _currentTrader = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasTrader));
            }
        }

        public bool HasTrader => CurrentTrader != null;
        public event EventHandler<GameMessageEventArgs> OnMessageRaised;

        //TODO move this to battle class or to living entity class
        public void UseScroll(GameItem scroll)
        {
            if (scroll.Category != GameItem.ItemCategory.AttackScroll)
            {
                return;
            }

            if (CurrentMonster == null)
            {
                return;
            }

            //TODO which scroll to use
            scroll.Action.OnActionPerformed += (s, e) => _messageBroker.RaiseMessage(e);
            scroll.PerformAction(CurrentPlayer, CurrentMonster);
            CurrentPlayer.RemoveItemFromInventory(scroll);
        }

        public void AttackCurrentMonster()
        {
            _currentBattle.AttackOpponent();
        }

        private void CompleteQuestAtLocation()
        {
            foreach (var quest in CurrentLocation.QuestAvailableHere)
            {
                var questToComplete =
                    CurrentPlayer.Quests.FirstOrDefault(q => q.PlayerQuest.ID == quest.ID && !q.IsCompleted);

                if (questToComplete != null)
                {
                    if (CurrentPlayer.Inventory.HasAllTheseItems(quest.ItemsToComplete))
                    {
                        CurrentPlayer.RemoveItemsFromInventory(quest.ItemsToComplete);

                        _messageBroker.RaiseMessage("");
                        _messageBroker.RaiseMessage($"You completed the '{quest.Name}'");

                        _messageBroker.RaiseMessage($"You received {quest.RewardExperiencePoints} experience points");
                        CurrentPlayer.AddExperience(quest.RewardExperiencePoints);

                        _messageBroker.RaiseMessage($"You received {quest.RewardGold} gold");
                        CurrentPlayer.ReceiveGold(quest.RewardGold);

                        foreach (var itemQuantity in quest.RewardItems)
                        {
                            var rewardItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);

                            CurrentPlayer.AddItemToInventory(rewardItem);
                            _messageBroker.RaiseMessage($"You received a {rewardItem.Name}");
                        }

                        questToComplete.IsCompleted = true;
                    }
                }
            }
        }

        private void GivePlayerQuestsAtLocation()
        {
            foreach (var quest in CurrentLocation.QuestAvailableHere)
            {
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
                {
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));

                    _messageBroker.RaiseMessage("");
                    _messageBroker.RaiseMessage($"You received the '{quest.Name}'");
                    _messageBroker.RaiseMessage(quest.Description);

                    _messageBroker.RaiseMessage("return with:");
                    quest.ItemsToComplete.ForEach(o =>
                        _messageBroker.RaiseMessage($"   {o.Quantity} {ItemFactory.CreateGameItem(o.ItemID).Name}"));

                    _messageBroker.RaiseMessage("And you will receive: ");
                    _messageBroker.RaiseMessage($" {quest.RewardGold} gold");
                    _messageBroker.RaiseMessage($" {quest.RewardExperiencePoints} experience points");
                    quest.RewardItems.ForEach(o =>
                        _messageBroker.RaiseMessage($"   {o.Quantity} {ItemFactory.CreateGameItem(o.ItemID).Name}"));
                }
            }
        }

        public void UseCurrentConsumable()
        {
            if (CurrentPlayer.CurrentConsumable != null)
            {
                CurrentPlayer.UseCurrentConsumable();
            }
        }

        public void CraftItemUsing(Recipe recipe)
        {
            if (CurrentPlayer.Inventory.HasAllTheseItems(recipe.Ingredients))
            {
                CurrentPlayer.RemoveItemsFromInventory(recipe.Ingredients);

                foreach (var itemQuantity in recipe.OutputItems)
                {
                    for (var i = 0; i < itemQuantity.Quantity; i++)
                    {
                        var outputItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                        CurrentPlayer.AddItemToInventory(outputItem);

                        _messageBroker.RaiseMessage($"You craft 1 {outputItem.Name}");
                    }
                }
            }
            else
            {
                _messageBroker.RaiseMessage("You don't have the required ingredients: ");
                foreach (var itemQuantity in recipe.Ingredients)
                {
                    _messageBroker.RaiseMessage(
                        $"    {itemQuantity.Quantity} {ItemFactory.ItemName(itemQuantity.ItemID)}");
                }
            }
        }

        private void OnCurrentPlayerLeveledUp(object sender, System.EventArgs eventArg)
        {
            _messageBroker.RaiseMessage($"You are now level {CurrentPlayer.Level}");
        }

        private void OnPlayerKilled(object sender, System.EventArgs e)
        {
            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage($"The {CurrentMonster?.Name} killed you");

            CurrentLocation = CurrentWorld.LocationAt(0, -1);
            CurrentPlayer.CompletelyHeal();
        }

        private void OnCurrentMonsterKilled(object sender, System.EventArgs e)
        {
            // Get another monster to fight
            CurrentMonster = CurrentLocation.GetMonster();
        }

        #region Location movement checks

        public bool HasLocationToNorth =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1) != null;

        public bool HasLocationToSouth =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null;

        public bool HasLocationToWest =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null;

        public bool HasLocationToEast =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null;

        public bool HasMonster => CurrentMonster != null;

        #endregion

        #region Movement

        public void MoveNorth()
        {
            if (HasLocationToNorth)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
            }
        }

        public void MoveWest()
        {
            if (HasLocationToWest)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
            }
        }

        public void MoveEast()
        {
            if (HasLocationToEast)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);
            }
        }

        public void MoveSouth()
        {
            if (HasLocationToSouth)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
            }
        }

        #endregion
    }
}
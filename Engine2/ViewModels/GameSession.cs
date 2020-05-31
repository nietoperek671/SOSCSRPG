﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.EventArgs;
using Engine.Factories;
using Engine.Models;

namespace Engine.ViewModels
{
    public class
        GameSession : BaseNotificationClass
    {
        private Location _currentLocation;
        private Monster _currentMonster;
        private Player _currentPlayer;
        private Trader _currentTrader;

        public GameSession()
        {
            #region Create player

            CurrentPlayer = new Player("Scott", "Fighter", 0, 10, 10, 1000);

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

        public World CurrentWorld { get; }

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPerformed -= OnCurrentPlayerPerformedAction;
                    _currentPlayer.OnKilled -= OnCurrentPlayerKilled;
                    _currentPlayer.OnLeveledUp -= OnCurrentPlayerLeveledUp;
                }

                _currentPlayer = value;

                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPerformed += OnCurrentPlayerPerformedAction;
                    _currentPlayer.OnKilled += OnCurrentPlayerKilled;
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
                GetMonsterAtLocation();

                CurrentTrader = CurrentLocation.TraderHere;
            }
        }

        public Monster CurrentMonster
        {
            get => _currentMonster;
            set
            {
                if (_currentMonster != null)
                {
                    _currentMonster.OnKilled -= OnCurrentMonsterKilled;
                    _currentMonster.OnActionPerformed -= OnCurrentMonsterPerformedAction;
                }

                _currentMonster = value;

                if (_currentMonster != null)
                {
                    _currentMonster.OnKilled += OnCurrentMonsterKilled;
                    _currentMonster.OnActionPerformed += OnCurrentMonsterPerformedAction;

                    RaiseMessage("");
                    RaiseMessage($"You are attacked by {CurrentMonster.Name}!");
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

        private void GetMonsterAtLocation()
        {
            CurrentMonster = CurrentLocation.GetMonster();
        }

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
            scroll.Action.OnActionPerformed += OnCurrentPlayerPerformedAction;
            scroll.PerformAction(CurrentPlayer, CurrentMonster);
            scroll.Action.OnActionPerformed -= OnCurrentPlayerPerformedAction;
            CurrentPlayer.RemoveItemFromInventory(scroll);
        }

        public void AttackCurrentMonster()
        {
            if (CurrentMonster == null)
            {
                return;
            }

            if (CurrentPlayer.CurrentWeapon == null)
            {
                RaiseMessage("You must have a weapon to fight a monster!");
                return;
            }

            CurrentPlayer.UseCurrentWeaponOn(CurrentMonster);

            if (CurrentMonster.IsDead)
            {
                GetMonsterAtLocation();
            }
            else
            {
                CurrentMonster.UseCurrentWeaponOn(CurrentPlayer);
            }
        }

        private void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
        }

        private void CompleteQuestAtLocation()
        {
            foreach (Quest quest in CurrentLocation.QuestAvailableHere)
            {
                QuestStatus questToComplete = CurrentPlayer.Quests.FirstOrDefault(q => q.PlayerQuest.ID == quest.ID && !q.IsCompleted);

                if (questToComplete != null)
                {
                    if (CurrentPlayer.Inventory.HasAllTheseItems(quest.ItemsToComplete))
                    {
                        CurrentPlayer.RemoveItemsFromInventory(quest.ItemsToComplete);

                        RaiseMessage("");
                        RaiseMessage($"You completed the '{quest.Name}'");

                        RaiseMessage($"You received {quest.RewardExperiencePoints} experience points");
                        CurrentPlayer.AddExperience(quest.RewardExperiencePoints);

                        RaiseMessage($"You received {quest.RewardGold} gold");
                        CurrentPlayer.ReceiveGold(quest.RewardGold);

                        foreach (ItemQuantity itemQuantity in quest.RewardItems)
                        {
                            GameItem rewardItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);

                            CurrentPlayer.AddItemToInventory(rewardItem);
                            RaiseMessage($"You received a {rewardItem.Name}");
                        }

                        questToComplete.IsCompleted = true;
                    }
                }
            }
        }

        private void GivePlayerQuestsAtLocation()
        {
            foreach (Quest quest in CurrentLocation.QuestAvailableHere)
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.ID == quest.ID))
                {
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));

                    RaiseMessage("");
                    RaiseMessage($"You received the '{quest.Name}'");
                    RaiseMessage(quest.Description);

                    RaiseMessage("return with:");
                    quest.ItemsToComplete.ForEach(o => RaiseMessage($"   {o.Quantity} {ItemFactory.CreateGameItem(o.ItemID).Name}"));

                    RaiseMessage("And you will receive: ");
                    RaiseMessage($" {quest.RewardGold} gold");
                    RaiseMessage($" {quest.RewardExperiencePoints} experience points");
                    quest.RewardItems.ForEach(o => RaiseMessage($"   {o.Quantity} {ItemFactory.CreateGameItem(o.ItemID).Name}"));
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

                foreach (ItemQuantity itemQuantity in recipe.OutputItems)
                    for (var i = 0; i < itemQuantity.Quantity; i++)
                    {
                        GameItem outputItem = ItemFactory.CreateGameItem(itemQuantity.ItemID);
                        CurrentPlayer.AddItemToInventory(outputItem);

                        RaiseMessage($"You craft 1 {outputItem.Name}");
                    }
            }
            else
            {
                RaiseMessage("You don't have the required ingredients: ");
                foreach (ItemQuantity itemQuantity in recipe.Ingredients)
                    RaiseMessage($"    {itemQuantity.Quantity} {ItemFactory.ItemName(itemQuantity.ItemID)}");
            }
        }

        private void OnCurrentPlayerLeveledUp(object sender, System.EventArgs eventArg)
        {
            RaiseMessage($"You are now level {CurrentPlayer.Level}");
        }

        private void OnCurrentPlayerPerformedAction(object sender, string result)
        {
            RaiseMessage(result);
        }

        private void OnCurrentMonsterPerformedAction(object sender, string result)
        {
            RaiseMessage(result);
        }

        private void OnCurrentPlayerKilled(object sender, System.EventArgs e)
        {
            RaiseMessage("");
            RaiseMessage($"The {CurrentMonster?.Name} killed you");

            CurrentLocation = CurrentWorld.LocationAt(0, -1);
            CurrentPlayer.CompletelyHeal();
        }

        private void OnCurrentMonsterKilled(object sender, System.EventArgs e)
        {
            RaiseMessage("");
            RaiseMessage($"You defeated the {CurrentMonster.Name}");

            CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);
            RaiseMessage($"You received {CurrentMonster.RewardExperiencePoints} experience points");

            CurrentPlayer.ReceiveGold(CurrentMonster.Gold);
            RaiseMessage($"You received {CurrentMonster.Gold} gold");

            foreach (GameItem gameItem in CurrentMonster.Inventory.Items)
            {
                CurrentPlayer.AddItemToInventory(gameItem);
                RaiseMessage($"You received one {gameItem.Name}.");
            }
        }

        #region Location movement checks

        public bool HasLocationToNorth => CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1) != null;
        public bool HasLocationToSouth => CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null;
        public bool HasLocationToWest => CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null;
        public bool HasLocationToEast => CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null;
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
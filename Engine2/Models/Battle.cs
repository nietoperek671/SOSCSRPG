using System;
using System.Collections.Generic;
using System.Linq;
using Engine.EventArgs;
using Engine.Services;

namespace Engine.Models
{
    public class Battle : IDisposable
    {
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();
        private readonly Monster _opponent;
        private readonly Player _player;

        public Battle(Player player, Monster opponent)
        {
            _player = player;
            _opponent = opponent;

            _player.OnActionPerformed += OnCombatantActionPerformed;
            _opponent.OnActionPerformed += OnCombatantActionPerformed;
            _opponent.OnKilled += OnOpponentKilled;

            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage($"You see a {_opponent.Name} here!");
            if (FirstAttacker == Combatant.Opponent)
            {
                AttackPlayer();
            }
        }

        private static Combatant FirstAttacker =>
            RandomNumberGenerator.NumberBetween(1, 2) == 1 ? Combatant.Player : Combatant.Opponent;

        public void Dispose()
        {
            _player.OnActionPerformed -= OnCombatantActionPerformed;
            _opponent.OnActionPerformed -= OnCombatantActionPerformed;
            _opponent.OnKilled -= OnOpponentKilled;
        }

        public void AttackOpponent()
        {
            if (_player.CurrentWeapon == null)
            {
                _messageBroker.RaiseMessage("You must select a weapon, to attack!");
                return;
            }

            _player.UseCurrentWeaponOn(_opponent);

            if (_opponent.IsAlive)
            {
                AttackPlayer();
            }
        }

        private void AttackPlayer()
        {
            _opponent.UseCurrentWeaponOn(_player);
        }

        private void OnCombatantActionPerformed(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }

        private void OnOpponentKilled(object sender, System.EventArgs e)
        {
            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage($"You defeated the {_opponent.Name}!");

            _messageBroker.RaiseMessage($"You receive {_opponent.RewardExperiencePoints} experience points.");
            _player.AddExperience(_opponent.RewardExperiencePoints);

            _messageBroker.RaiseMessage($"You receive {_opponent.Gold} gold.");
            _player.ReceiveGold(_opponent.Gold);

            foreach (GameItem gameItem in _opponent.Inventory.Items)
            {
                _messageBroker.RaiseMessage($"You received one {gameItem.Name}.");
                _player.AddItemToInventory(gameItem);
            }

            OnCombatVictory?.Invoke(this, new CombatVictoryEventArgs());
        }

        public event EventHandler<CombatVictoryEventArgs> OnCombatVictory;

        private enum Combatant
        {
            Player,
            Opponent
        }
    }
}
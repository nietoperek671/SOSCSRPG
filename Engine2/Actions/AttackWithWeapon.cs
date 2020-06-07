using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Models;
using Engine.Services;

namespace Engine.Actions
{
    public class AttackWithWeapon : BaseAction, IAction
    {
        private readonly int _maximumDamage;
        private readonly int _minimumDamage;

        public AttackWithWeapon(GameItem itemInUse, int minimumDamage, int maximumDamage)
            : base(itemInUse)
        {
            if (itemInUse.Category != GameItem.ItemCategory.Weapon)
            {
                throw new ArgumentException($"{itemInUse.Name} is not a weapon");
            }

            if (minimumDamage < 0)
            {
                throw new ArgumentException("minimum damage < 0");
            }

            if (minimumDamage > maximumDamage)
            {
                throw new ArgumentException("minimum damage > maximum damage");
            }

            _minimumDamage = minimumDamage;
            _maximumDamage = maximumDamage;
        }

        public void Execute(LivingEntity actor, LivingEntity target)
        {
            string actorName = actor is Player ? "You" : $"The {actor.Name.ToLower()}";
            string targetName = target is Player ? "you" : $"the {target.Name.ToLower()}";

            if (CombatService.AttackSucceeded(actor, target))
            {
                int damage = RandomNumberGenerator.NumberBetween(_minimumDamage, _maximumDamage);

                ReportResult($"{actorName} hit {targetName} for {damage} point{(damage > 1 ? "s" : "")}");
                target.TakeDamage(damage);
            }
            else
            {
                ReportResult($"{actorName} missed {targetName}");
            }
        }
    }
}
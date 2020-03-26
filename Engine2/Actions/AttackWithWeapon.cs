using Engine.Models;
using System;

namespace Engine.Actions
{
    public class AttackWithWeapon : BaseAction, IAction
    {
        private readonly int _minimumDamage;
        private readonly int _maximumDamage;

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
            int damage = RandomNumberGenerator.NumberBetween(_minimumDamage, _maximumDamage);

            string actorName = (actor is Player) ? "You" : $"The {actor.Name.ToLower()}";
            string targetName = (target is Player) ? "you" : $"the {target.Name.ToLower()}";

            if (damage == 0)
            {
                ReportResult($"{actorName} missed {targetName}");
            }
            else
            {
                ReportResult($"{actorName} hit {targetName} for {damage} point{(damage > 1 ? "s" : "")}");
                target.TakeDamage(damage);
            }
        }
    }
}

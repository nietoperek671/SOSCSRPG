using Engine.Models;
using System;

namespace Engine.Actions
{
    public class AttackWithWeapon
    {
        private readonly GameItem _weapon;
        private readonly int _minimumDamage;
        private readonly int _maximumDamage;

        public event EventHandler<string> OnActionPerformed;

        public AttackWithWeapon(GameItem weapon, int minimumDamage, int maximumDamage)
        {
            if (weapon.Category != GameItem.ItemCategory.Weapon)
            {
                throw new ArgumentException($"{weapon.Name} is not a weapon");
            }

            if (minimumDamage < 0)
            {
                throw new ArgumentException("minimum damage < 0");
            }

            if (minimumDamage > maximumDamage)
            {
                throw new ArgumentException("minimum damage > maximum damage");
            }

            _weapon = weapon;
            _minimumDamage = minimumDamage;
            _maximumDamage = maximumDamage;
        }

        public void Execute(LivingEntity actor, LivingEntity target)
        {
            int damage = RandomNumberGenerator.NumberBetween(_minimumDamage, _maximumDamage);

            if (damage == 0)
            {
                ReportResult($"You missed!");
            }
            else
            {
                ReportResult($"Yout hit the {target.Name} for {damage} points.");
                target.TakeDamage(damage);
            }
        }

        private void ReportResult(string result)
        {
            OnActionPerformed?.Invoke(this, result);
        }
    }
}

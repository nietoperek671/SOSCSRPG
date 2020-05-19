using Engine.Models;
using System;

namespace Engine.Actions
{
    public class AttackWithScroll : BaseAction, IAction
    {
        private readonly int _damage;

        public AttackWithScroll(GameItem gameItem, int damage)
            : base(gameItem)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Scroll damage must be > 0");
            }

            _damage = damage;
        }

        public void Execute(LivingEntity actor, LivingEntity target)
        {
            string actorName = (actor is Player) ? "You" : $"The {actor.Name.ToLower()}";
            string targetName = (target is Player) ? "you" : $"the {target.Name.ToLower()}";

            if (_damage == 0)
            {
                ReportResult($"{actorName} missed {targetName}");
            }
            else
            {
                ReportResult($"{actorName} cast a {_itemInUse.Name} and hit {targetName} for {_damage} point{(_damage > 1 ? "s" : "")}");
                target.TakeDamage(_damage);
            }
        }
    }
}

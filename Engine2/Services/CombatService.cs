using Engine.Models;

namespace Engine.Services
{
    public static class CombatService
    {
        public enum Combatant
        {
            Player,
            Opponent
        }

        public static Combatant FirstAttacker(Player player, Monster opponent)
        {
            // Formula is: ((Dex(player)^2 - Dex(monster)^2)/10) + Random(-10/10)
            // For dexterity values from 3 to 18, this should produce an offset of +/- 41.5
            var playerDexterity = player.Dexterity * player.Dexterity;
            var opponentDexterity = opponent.Dexterity * opponent.Dexterity;
            var dexterityOffset = (playerDexterity - opponentDexterity) / 10m;
            var randomOffset = RandomNumberGenerator.NumberBetween(-10, 10);
            var totalOffset = dexterityOffset + randomOffset;

            return RandomNumberGenerator.NumberBetween(0, 100) <= 50 + totalOffset
                ? Combatant.Player
                : Combatant.Opponent;
        }

        public static bool AttackSucceeded(LivingEntity attacker, LivingEntity target)
        {
            // Currently using the same formula as FirstAttacker initiative.
            // This will change as we include attack/defense skills,
            // armor, weapon bonuses, enchantments/curses, etc.
            var playerDexterity = attacker.Dexterity * attacker.Dexterity;
            var opponentDexterity = target.Dexterity * target.Dexterity;
            var dexterityOffset = (playerDexterity - opponentDexterity) / 10m;
            var randomOffset = RandomNumberGenerator.NumberBetween(-10, 10);
            var totalOffset = dexterityOffset + randomOffset;

            return RandomNumberGenerator.NumberBetween(0, 100) <= 50 + totalOffset;
        }
    }
}
namespace Engine.Models
{
    public class Monster : LivingEntity
    {
        public string ImageName { get; set; }
        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        public int RewardExperiencePoints { get; private set; }

        public Monster(string name, string imageName, 
                       int maximumHitPoints, int currentHitPoints, 
                       int minimumDamage, int maximumDamage, 
                       int rewardExperiencePoints, int gold)
            :base(name,maximumHitPoints,currentHitPoints,gold)
        {
            ImageName = $"/Engine;component/Images/Monsters/{imageName}";
            MaximumDamage = maximumDamage;
            MinimumDamage = minimumDamage;
            RewardExperiencePoints = rewardExperiencePoints;
        }
    }
}

namespace Engine.Models
{
    public class Trader : LivingEntity
    {
        public Trader(int id, string name) : base(name, 9999, 9999, 9999, 18) => ID = id;

        public int ID { get; }
    }
}
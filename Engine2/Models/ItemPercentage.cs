namespace Engine.Models
{
    public class ItemPercentage
    {
        public ItemPercentage(int id, int percentage)
        {
            ID = id;
            Percentage = percentage;
        }

        public int ID { get; }
        public int Percentage { get; }
    }
}
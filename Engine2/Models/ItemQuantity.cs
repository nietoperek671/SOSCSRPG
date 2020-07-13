using Engine.Factories;

namespace Engine.Models
{
    public class ItemQuantity
    {
        public ItemQuantity(int itemID, int quantity)
        {
            ItemID = itemID;
            Quantity = quantity;
        }

        public int ItemID { get; }
        public int Quantity { get; }

        public string QuantityItemDescription => $"{Quantity} {ItemFactory.ItemName(ItemID)}";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Engine.Models
{
    public class Recipe
    {
        public Recipe(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        [JsonIgnore]
        public List<ItemQuantity> Ingredients { get; set; } = new List<ItemQuantity>();
        [JsonIgnore]
        public List<ItemQuantity> OutputItems { get; set; } = new List<ItemQuantity>();

        [JsonIgnore]
        public string ToolTipContents
        {
            get
            {
                var nl = Environment.NewLine;
                return "Ingredients" + nl +
                       "============" + nl +
                       string.Join(nl, Ingredients.Select(i => i.QuantityItemDescription)) +
                       nl + nl +
                       "Creates" + nl +
                       "============" + nl +
                       string.Join(nl, OutputItems.Select(i => i.QuantityItemDescription));
            }
        }

        public void AddIngredient(int itemID, int quantity)
        {
            if (!Ingredients.Any(x => x.ItemID == itemID))
            {
                Ingredients.Add(new ItemQuantity(itemID, quantity));
            }
        }

        public void AddOutputItem(int itemID, int quantity)
        {
            if (!OutputItems.Any(x => x.ItemID == itemID))
            {
                OutputItems.Add(new ItemQuantity(itemID, quantity));
            }
        }
    }
}
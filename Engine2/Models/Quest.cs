using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Engine.Models
{
    public class Quest
    {
        public Quest(int iD, string name, string description, List<ItemQuantity> itemsToComplete,
            int rewardExperiencePoints, int rewardGold, List<ItemQuantity> rewardItems)
        {
            ID = iD;
            Name = name;
            Description = description;
            ItemsToComplete = itemsToComplete;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            RewardItems = rewardItems;
        }

        public int ID { get; }
        [JsonIgnore]
        public string Name { get; }
        [JsonIgnore]
        public string Description { get; }

        [JsonIgnore]
        public List<ItemQuantity> ItemsToComplete { get; }

        [JsonIgnore]
        public int RewardExperiencePoints { get; }
        [JsonIgnore]
        public int RewardGold { get; }
        [JsonIgnore]
        public List<ItemQuantity> RewardItems { get; }

        [JsonIgnore]
        public string ToolTipContents
        {
            get
            {
                var nl = Environment.NewLine;
                return Description + nl + nl +
                       "Items to complete the quest" + nl +
                       "===========================" + nl +
                       string.Join(nl, ItemsToComplete.Select(i => i.QuantityItemDescription)) +
                       nl + nl +
                       "Rewards" + nl +
                       "===========================" + nl +
                       $"{RewardExperiencePoints} experience points" + nl +
                       $"{RewardGold} gold pieces" + nl +
                       string.Join(nl, RewardItems.Select(i => i.QuantityItemDescription));
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Engine.Factories;

namespace Engine.Models
{
    public class Location
    {
        public Location(int xCoordinate, int yCoordinate, string name, string description, string imageName)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            Name = name;
            Description = description;
            ImageName = imageName;
        }

        public int XCoordinate { get; }
        public int YCoordinate { get; }
        [JsonIgnore]
        public string Name { get; }
        [JsonIgnore]
        public string Description { get; }
        [JsonIgnore]
        public string ImageName { get; }
        [JsonIgnore]
        public List<Quest> QuestAvailableHere { get; } = new List<Quest>();
        [JsonIgnore]
        public List<MonsterEncounter> MonstersHere { get; } = new List<MonsterEncounter>();
        [JsonIgnore]
        public Trader TraderHere { get; set; }

        public void AddMonster(int monsterID, int chanceOfEncountering)
        {
            if (MonstersHere.Exists(m => m.MonsterID == monsterID))
            {
                MonstersHere.First(m => m.MonsterID == monsterID).ChanceOfEncountering = chanceOfEncountering;
            }
            else
            {
                MonstersHere.Add(new MonsterEncounter(monsterID, chanceOfEncountering));
            }
        }

        public Monster GetMonster()
        {
            if (!MonstersHere.Any())
            {
                return null;
            }

            var totalChances = MonstersHere.Sum(m => m.ChanceOfEncountering);

            var randomNumber = RandomNumberGenerator.NumberBetween(1, totalChances);

            var runningTotal = 0;

            foreach (var monsterEncounter in MonstersHere)
            {
                runningTotal += monsterEncounter.ChanceOfEncountering;

                if (randomNumber <= runningTotal)
                {
                    return MonsterFactory.GetMonster(monsterEncounter.MonsterID);
                }
            }

            return MonsterFactory.GetMonster(MonstersHere.Last().MonsterID);
        }
    }
}
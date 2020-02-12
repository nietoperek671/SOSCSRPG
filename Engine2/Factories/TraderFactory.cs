using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Factories
{
    public class TraderFactory
    {
        private static readonly List<Trader> _traders = new List<Trader>();

        static TraderFactory()
        {
            Trader susan = new Trader("Susan");
            susan.AddItemToInventory(ItemFactory.CreateGameItem(1001));

            Trader farmerTrader = new Trader("Farmer Ted");
            farmerTrader.AddItemToInventory(ItemFactory.CreateGameItem(1001));

            Trader peteTheHerbalist = new Trader("Pete the Herbalist");
            peteTheHerbalist.AddItemToInventory(ItemFactory.CreateGameItem(1001));

            AddTraderToList(susan);
            AddTraderToList(farmerTrader);
            AddTraderToList(peteTheHerbalist);
        }

        public static Trader GetTraderByName(string name) => _traders.First(trader => trader.Name == name);

        private static void AddTraderToList(Trader trader)
        {
            if (_traders.Any(t => t.Name == trader.Name))
                throw new ArgumentException($"There is already a trader named '{trader}'");

            _traders.Add(trader);
        }
    }
}

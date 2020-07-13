using System.Collections.Generic;
using System.Linq;

namespace Engine.Models
{
    public class World
    {
        private readonly List<Location> _locations = new List<Location>();

        internal void AddLocation(Location location)
        {
            _locations.Add(location);
        }

        public Location LocationAt(int xCoordinate, int yCoordinate) =>
            _locations.FirstOrDefault(o => o.XCoordinate == xCoordinate && o.YCoordinate == yCoordinate);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class World
    {
        private List<Location> _locations = new List<Location>();

        internal void AddLocation(int xCoordinate, int yCoordinate, string name, string description, string imageName)
        {
            Location loc = new Location();
            loc.XCoordinate = xCoordinate;
            loc.YCoordinate = yCoordinate;
            loc.Name = name;
            loc.Description = description;
            loc.ImageName = $"/Engine;component/Images/Locations/{imageName}";

            _locations.Add(loc);
        }

        public Location LocationAt (int xCoordinate, int yCoordinate) => _locations.FirstOrDefault(o => o.XCoordinate == xCoordinate && o.YCoordinate == yCoordinate);
    }
}

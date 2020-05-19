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
            _locations.Add(new Location(xCoordinate, yCoordinate, name, description,
                                        $"/Engine;component/Images/Locations/{imageName}"));
        }

        public Location LocationAt (int xCoordinate, int yCoordinate) => _locations.FirstOrDefault(o => o.XCoordinate == xCoordinate && o.YCoordinate == yCoordinate);
    }
}

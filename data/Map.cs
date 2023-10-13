using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace rt.data
{
    internal class Map
    {
        public List<Wall> walls;

        public Map() {
            walls = new List<Wall>
            {
                new Wall(0,0,0, 10, 1, new Vector3(255, 0, 0)),
                new Wall(0, 10, 10, 10, 1, new Vector3(0,255, 0)),
                new Wall(10, 10, 10, 0, 1, new Vector3(0,0,255)),
                new Wall(10, 0, 0, 0, 1, new Vector3(255, 255, 0))

            };


        }
    }
}

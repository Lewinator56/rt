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
                new Wall(100,100,100, 110, 1, new Vector3(255, 0, 0)),
                new Wall(100, 110, 110, 110, 1, new Vector3(0,255, 0)),
                new Wall(110, 110, 150, 120, 1, new Vector3(0,0,255)),
                new Wall(110, 100, 100, 120, 1, new Vector3(255, 255, 0)),
                new Wall(120, 120, 130, 120, 1, new Vector3(255,255,255)),
                new Wall(140, 140, 130, 120, 1, new Vector3(255, 0 ,255))

            };


        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace rt.data
{
    internal class Map
    {
        public List<Wall> walls;

        public Map() {
            walls = new List<Wall>();
            

            


        }
        public void load()
        {
            string[] m = File.ReadAllLines("map.txt");
            foreach (string line in m)
            {
                string[] wdata = line.Split(" ");
                walls.Add(new Wall(
                    Convert.ToSingle(wdata[0]),
                    Convert.ToSingle(wdata[1]),
                    Convert.ToSingle(wdata[2]),
                    Convert.ToSingle(wdata[3]),
                    1,
                    new Vector3(
                        Convert.ToSingle(wdata[4]),
                        Convert.ToSingle(wdata[5]),
                        Convert.ToSingle(wdata[6])
                     )
                 ));
            }
        }
    }
}

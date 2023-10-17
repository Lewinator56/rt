using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace rt.data
{
    internal class Map
    {
        public List<Wall> walls;
        public byte[] floortex = new byte[128*128*3];
        public byte[] walltex = new byte[128*128*3];

        public Map() {
            walls = new List<Wall>();
            

            


        }
        public void load()
        {
            string[] m = File.ReadAllLines("map.txt");
            try
            {
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
                         ),
                        Convert.ToInt16(wdata[7]),
                        File.ReadAllBytes("textures/" + wdata[8] + ".bmp")[^49152..],
                        File.ReadAllBytes("textures/alpha_" + wdata[9] + ".bmp")[^49152..]
                     ));
                }
            } catch { }
            
            

            //floortex = File.ReadAllBytes("textures/tile.bmp")[^49152..];

        }
    }
}

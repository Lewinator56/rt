using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace rt.data
{
    internal class Wall : GameObject
    {
        public float x1;
        public float y1;
        public float x2;
        public float y2;
        public float height;
        public Vector3 color;
        public int wtype;
        public byte[] tex;
        public byte[] alpha;

        public Wall(float x1, float y1, float x2, float y2, float height, Vector3 color, int wtype, byte[] tex, byte[] alpha)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            this.height = height;
            this.color = color;
            this.wtype = wtype;
            this.tex = tex;
            this.alpha = alpha;
            
        }



    }
}

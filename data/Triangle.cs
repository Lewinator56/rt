using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace rt.data
{
    internal class Triangle
    {
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;
        Vector3 color;

        public Triangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 color)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.color = color;
        }
    }
}

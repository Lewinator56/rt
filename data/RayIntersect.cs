using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.data
{
    internal class RayIntersect
    {
        public  GameObject? gameObject;
        public float distance;
        public float distToStart;

        public RayIntersect(GameObject? gameObject, float distance)
        {
            this.gameObject = gameObject;
            this.distance = distance;
            
        }
    }
}

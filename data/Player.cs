using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace rt.data
{
    class Player : GameObject
    {

    
        public float stepSize;
        public GameObject rayCaster;
        public float fov = 70;
        

        public Player(string name, Vector3 location, Vector3 rotation, Vector3 scale, float stepSize)
        {
            this.stepSize = stepSize;
            this.name = name;
            this.transform.location = location;
            this.transform.rotation = rotation;
            this.transform.scale = scale;
            

        }

        public void MoveForward(float t)
        {
            transform.location += transform.Forward() * t;
        }
        
    }
}

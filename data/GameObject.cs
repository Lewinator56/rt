using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace rt.data
{
    class GameObject
    {
        public int id;
        public string name;
        public Transform transform = new Transform(Vector3.Zero,Vector3.Zero,Vector3.One);

        public GameObject() { }
        public GameObject(string name, Vector3 location, Vector3 rotation, Vector3 scale) { 
            this.name = name;
            transform = new Transform(location, rotation, scale);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace rt.data
{
    internal class Transform
    {
        public Vector3 location;
        public Vector3 rotation;
        public Vector3 scale;
        
        public Transform(Vector3 location, Vector3 rotation, Vector3 scale)
        {
            this.location = location;
            this.rotation = rotation;
            this.scale = scale;
        }

        public void Rotate(Vector3 axis, float angle)
        {
            rotation.X = rotation.X + (axis.X * angle);
            rotation.Y = rotation.Y + (axis.Y * angle);
            rotation.Z = rotation.Z + (axis.Z * angle);
            //Debug.WriteLine(Angles());
        }
        public Vector3 PreviewRotation(Vector3 axis, float angle)
        {
            return new Vector3 (
                rotation.X + (axis.X * angle),
                rotation.Y + (axis.Y * angle),
                rotation.Z + (axis.Z * angle)
            );
        }
        public Vector3 Angles()
        {
            return new Vector3(180f/MathF.PI * rotation.X, 180f/MathF.PI * rotation.Y, 180f/MathF.PI * rotation.Z);
        }

        public void Scale(Vector3 scale)
        {
            this.scale *= scale;
        }

        public void Translate(Vector3 translation)
        {
            location.X += translation.X;
            location.Y += translation.Y;
            location.Z += translation.Z;
        }

        public Vector3 Forward() { 
            Vector3 vector = new Vector3();
            vector.X = MathF.Cos(rotation.X) * MathF.Sin(rotation.Z);
            vector.Y = MathF.Cos(rotation.X) * MathF.Cos(rotation.Z);
            vector.Z = -MathF.Sin(rotation.X);
            return vector;
        }
        
    }
}

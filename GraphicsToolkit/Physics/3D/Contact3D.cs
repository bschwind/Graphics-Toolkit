using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Bodies;

namespace GraphicsToolkit.Physics._3D
{
    public struct Contact3D
    {
        public Vector3 Normal;
        public Vector3 pointA, pointB;
        public float Dist;
        public float Impulse;
        public RigidBody3D A, B;

        public static Random rand = new Random();

        public Contact3D(Vector3 normal, float d, RigidBody3D a, RigidBody3D b, Vector3 pointA, Vector3 pointB)
        {
            Normal = normal;
            Dist = d;
            A = a;
            B = b;
            this.pointA = pointA;
            this.pointB = pointB;

            Normal = new Vector3((float)(Normal.X + rand.NextDouble() / 100f), (float)(Normal.Y + rand.NextDouble() / 100f), (float)(Normal.Z + rand.NextDouble() / 100f));

            //I = (1+e)*N*(Vr • N) / (1/Ma + 1/Mb)
            //Impulse = (Vector3.Dot(b.Vel - a.Vel, normal) / (a.InvMass + b.InvMass));
            Impulse = 0;
        }

        public void ApplyImpulses(Vector3 imp)
        {
            A.Vel = A.Vel + (imp * A.InvMass);
            B.Vel = B.Vel - (imp * B.InvMass);
        }
    }
}

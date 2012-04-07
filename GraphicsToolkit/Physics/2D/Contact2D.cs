using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Bodies;

namespace GraphicsToolkit.Physics._2D
{
    public struct Contact2D
    {
        public static bool NoisyNormals = false;

        public Vector2 Normal;
        public Vector2 pointA, pointB;
        public float Dist;
        public float Impulse;
        public float InvDenom;
        public RigidBody2D A, B;

        public static Random rand = new Random();

        public Contact2D(Vector2 normal, float d, RigidBody2D a, RigidBody2D b, Vector2 pointA, Vector2 pointB)
        {
            Normal = normal;
            Dist = d;
            A = a;
            B = b;
            this.pointA = pointA;
            this.pointB = pointB;

            Normal = normal;
            if (NoisyNormals)
            {
                Normal = new Vector2((float)(Normal.X + (rand.NextDouble() - 0.5) / 100f), (float)(Normal.Y + (rand.NextDouble() - 0.5) / 100f));
            }


            // compute denominator in impulse equation
            float x = A.InvMass;
            float y = B.InvMass;
            float ran = Vector2.Dot(pointA - A.Pos, Normal);
            float rbn = Vector2.Dot(pointB - B.Pos, Normal);
            float z = ran * ran * A.InvInertia;
            float w = rbn * rbn * B.InvInertia;

            InvDenom = 1f / (x + y + z + w);

            //I = (1+e)*N*(Vr • N) / (1/Ma + 1/Mb)
            //Impulse = (Vector2.Dot(b.Vel - a.Vel, normal) / (a.InvMass + b.InvMass));
            Impulse = 0;
        }

        //Applies an impulse at local point p on the body
        public void ApplyImpulses(Vector2 imp)
        {
            Vector2 fPerp = GameMath.Perp2D(imp);
            Vector2 localAPos = pointA - A.Pos;
            float impRotValA = Vector2.Dot(fPerp, localAPos);
            Vector2 localBPos = pointB - B.Pos;
            float impRotValB = Vector2.Dot(fPerp, localBPos);

            if (A.InvMass != 0.0f)
            {
                A.Vel = A.Vel + (imp * A.InvMass);
                A.RotVel = A.RotVel + (impRotValA * A.InvInertia);
            }
            if (B.InvMass != 0.0f)
            {
                B.Vel = B.Vel - (imp * B.InvMass);
                B.RotVel = B.RotVel - (impRotValB * B.InvInertia);
            }
        }
    }
}

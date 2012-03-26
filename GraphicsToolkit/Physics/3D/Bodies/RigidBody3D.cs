using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Geometry;

namespace GraphicsToolkit.Physics._3D.Bodies
{
    public abstract class RigidBody3D
    {
        //Linear properties
        private Vector3 pos;
        private Vector3 vel;
        private float invMass;
        private Vector3 force;

        //Rotational properties
        private float rot;
        private float rotVel;
        private float invInertia;
        private float torque;

        //Intersection properties
        protected AABB3D motionBounds;

        public Vector3 Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        public float Rot
        {
            get
            {
                return rot;
            }
        }

        public Vector3 Vel
        {
            get
            {
                return vel;
            }

            set
            {
                vel = value;
            }
        }

        public float RotVel
        {
            get
            {
                return rotVel;
            }
            set
            {
                rotVel = value;
            }
        }

        public float InvMass
        {
            get
            {
                return invMass;
            }
        }

        public float InvInertia
        {
            get
            {
                return invInertia;
            }
        }

        public Vector3 Force
        {
            get
            {
                return force;
            }
        }

        public float Torque
        {
            get
            {
                return torque;
            }
        }

        public AABB3D MotionBounds
        {
            get
            {
                return motionBounds;
            }
        }

        public RigidBody3D(Vector3 pos, Vector3 vel, float rotVel, float mass, float inertia)
        {
            if (mass < 0)
            {
                throw new Exception("Mass can not be less than 0");
            }

            //This checks to see if the body has "infinite" mass
            if (mass == 0)
            {
                invMass = 0;
            }
            else
            {
                invMass = 1f / mass;
            }

            if (inertia == 0)
            {
                invInertia = 0;
            }
            else
            {
                invInertia = 1f / inertia;
            }

            this.pos = pos;
            this.vel = vel;
            this.rotVel = rotVel;

            GenerateMotionAABB(0f);
        }

        public void AddForce(Vector3 f)
        {
            force += f;
        }

        public void AddForce(Vector3 f, Vector3 p)
        {
            force += f;
            //TODO: Calculate proper torque
            //Vector3 fPerp = new Vector3(f.Y, -f.X);
            //torque += Vector3.Dot(fPerp, p);
        }

        public void ClearForces()
        {
            force = Vector3.Zero;
            torque = 0f;
        }

        public abstract Contact3D GenerateContact(RigidBody3D rb, float dt);
        public abstract void GenerateMotionAABB(float dt);

        public void Integrate(float dt)
        {
            pos += vel * dt;
            rot += rotVel * dt;
        }

        public Vector3 GetVelocityOfPoint(Vector3 p)
        {
            //TODO: Calculate proper velocity in 3D
            return this.vel; // +(rotVel * GameMath.Perp2D(p - this.pos));
        }
    }
}

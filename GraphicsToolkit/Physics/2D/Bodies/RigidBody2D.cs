using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Geometry;

namespace GraphicsToolkit.Physics._2D.Bodies
{
    public abstract class RigidBody2D
    {
        //Linear properties
        private Vector2 pos;
        private Vector2 vel;
        private float invMass;
        private Vector2 force;

        //Rotational properties
        private float rot;
        private float rotVel;
        private float invInertia;
        private float torque;

        //Intersection properties
        protected AABB2D motionBounds;

        public Vector2 Pos
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
            set
            {
                rot = value;
            }
        }

        public Vector2 Vel
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
            set
            {
                invMass = value;
            }
        }

        public float InvInertia
        {
            get
            {
                return invInertia;
            }
            set
            {
                invInertia = value;
            }
        }

        public Vector2 Force
        {
            get
            {
                return force;
            }
            set
            {
                force = value;
            }
        }

        public float Torque
        {
            get
            {
                return torque;
            }
            set
            {
                torque = value;
            }
        }

        public AABB2D MotionBounds
        {
            get
            {
                return motionBounds;
            }
        }

        public RigidBody2D(Vector2 pos, Vector2 vel, float rotVel, float mass, float inertia)
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

        public void AddForce(Vector2 f)
        {
            force += f;
        }

        //Applies a force at point p in the local coordinates of the body
        public void AddForce(Vector2 f, Vector2 p)
        {
            force += f;
            Vector2 fPerp = GameMath.Perp2D(f);
            torque += Vector2.Dot(fPerp, p);
        }

        public void ClearForces()
        {
            force = Vector2.Zero;
            torque = 0f;
        }

        public abstract void AddContacts(RigidBody2D rb, float dt, ref List<Contact2D> contacts);
        public abstract void GenerateMotionAABB(float dt);
        public abstract void PostIntegrationUpdate(Vector2 dx, float dRot);

        public void Integrate(float dt)
        {
            pos += vel * dt;
            rot += rotVel * dt;
            PostIntegrationUpdate(vel * dt, rotVel * dt);
        }

        //Gets the velocity of a point (defined in world coordinates) on the body
        public Vector2 GetVelocityOfWorldPoint(Vector2 p)
        {
            return this.vel + (-rotVel * GameMath.Perp2D(p - this.pos));
        }

        //Gets the velocity of a point (defined in local coordinates) on the body
        public Vector2 GetVelocityOfLocalPoint(Vector2 p)
        {
            return this.vel + (-rotVel * GameMath.Perp2D(p));
        }
    }
}

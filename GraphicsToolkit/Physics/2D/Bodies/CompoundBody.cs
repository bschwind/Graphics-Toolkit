using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Geometry;

namespace GraphicsToolkit.Physics._2D.Bodies
{
    public class CompoundBody : RigidBody2D
    {
        //The circle positions are relative to this body's position
        private List<CircleBody> circles;

        public CompoundBody(Vector2 pos, Vector2 vel, float rotVel) : base(pos, vel, rotVel, 0f, 0f)
        {
            circles = new List<CircleBody>();
        }

        public List<CircleBody> Bodies
        {
            get
            {
                return circles;
            }
        }

        public void AddBody(CircleBody c)
        {
            if (circles.Count <= 0)
            {
                this.Pos = c.Pos;
                this.InvMass = c.InvMass;
                this.InvInertia = c.InvInertia;

                c.Pos = Vector2.Zero;

                circles.Add(c);
                return;
            }

            circles.Add(c);

            float currentMass = (InvMass > 0.0f) ? 1f/InvMass : 0.0f;
            float circleMass = (c.InvMass > 0.0f) ? 1f / c.InvMass : 0f;

            //Calculate new center of mass
            Vector2 newCom = ((currentMass * this.Pos) + (circleMass * c.Pos)) / (currentMass + circleMass);
            //Calculate new inverse mass
            currentMass += circleMass;
            this.InvMass = 1f / currentMass;

            c.Pos = c.Pos - this.Pos;

            float inertia = 0f;

            foreach (CircleBody circle in circles)
            {
                circle.Pos += this.Pos;
                circle.Pos = circle.Pos - newCom;


                float tempMass = 1f / circle.InvMass;
                //Parallel axis theorem
                inertia += ((tempMass*circle.Radius*circle.Radius)) + tempMass * circle.Pos.LengthSquared();
            }

            this.InvInertia = 1f / inertia;
            this.Pos = newCom;
        }

        public void RemoveBody(CircleBody c)
        {
            circles.Remove(c);
        }

        public override void GenerateMotionAABB(float dt)
        {
            if (circles == null)
            {
                return;
            }

            Vector2 predictedPos = Pos + (Vel * dt);

            Vector2 oldMin = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 oldMax = new Vector2(float.MinValue, float.MinValue);
            Vector2 newMin = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 newMax = new Vector2(float.MinValue, float.MinValue);

            float rotDiff = RotVel * dt;
            Vector2 r1 = new Vector2((float)Math.Cos(rotDiff), (float)-Math.Sin(rotDiff));
            Vector2 r2 = new Vector2((float)Math.Sin(rotDiff), (float)Math.Cos(rotDiff));

            foreach (CircleBody c in circles)
            {
                Vector2 oldPos = this.Pos + c.Pos;

                oldMin = new Vector2(Math.Min(oldMin.X, oldPos.X - c.Radius), Math.Min(oldMin.Y, oldPos.Y - c.Radius));
                oldMax = new Vector2(Math.Max(oldMax.X, oldPos.X + c.Radius), Math.Max(oldMax.Y, oldPos.Y + c.Radius));

                Vector2 newPos = predictedPos + new Vector2(Vector2.Dot(r1,c.Pos), Vector2.Dot(r2, c.Pos));

                newMin = new Vector2(Math.Min(newMin.X, newPos.X - c.Radius), Math.Min(newMin.Y, newPos.Y - c.Radius));
                newMax = new Vector2(Math.Max(newMax.X, newPos.X + c.Radius), Math.Max(newMax.Y, newPos.Y + c.Radius));
            }

            Vector2 finalMin = Vector2.Min(oldMin, newMin);
            Vector2 finalMax = Vector2.Max(oldMax, newMax);

            Vector2 center = new Vector2(finalMin.X + (finalMax.X - finalMin.X) / 2f, finalMin.Y + (finalMax.Y - finalMin.Y) / 2f);

            motionBounds = new AABB2D(center, new Vector2((finalMax.X-finalMin.X) / 2f, (finalMax.Y-finalMin.Y)/2f));
        }

        public override void PostIntegrationUpdate(Vector2 dx, float dRot)
        {
            Vector2 r1 = new Vector2((float)Math.Cos(dRot), (float)-Math.Sin(dRot));
            Vector2 r2 = new Vector2((float)Math.Sin(dRot), (float)Math.Cos(dRot));

            foreach (CircleBody c in circles)
            {
                c.Pos = new Vector2(Vector2.Dot(r1, c.Pos), Vector2.Dot(r2, c.Pos));
                c.Rot = this.Rot;
            }
        }

        public override void AddContacts(RigidBody2D rb, float dt, ref List<Contact2D> contacts)
        {
            //Profiler says this method takes up a lot of processing time (AKA, it's run quite often)
            if (rb as CircleBody != null)
            {
                CircleBody c = rb as CircleBody;

                foreach (CircleBody circle in circles)
                {
                    Vector2 tempPos = circle.Pos;
                    circle.Pos += this.Pos;

                    circle.AddContacts(c, dt, ref contacts);
                    Contact2D justAdded = contacts[contacts.Count - 1];
                    contacts[contacts.Count - 1] = new Contact2D(justAdded.Normal, justAdded.Dist, this, justAdded.B, justAdded.pointA, justAdded.pointB);

                    circle.Pos = tempPos;
                }
            }
            else if (rb as LineBody != null)
            {
                foreach (CircleBody circle in circles)
                {
                    Vector2 tempPos = circle.Pos;
                    circle.Pos += this.Pos;

                    circle.AddContacts(rb, dt, ref contacts);
                    Contact2D justAdded = contacts[contacts.Count - 1];
                    contacts[contacts.Count - 1] = new Contact2D(justAdded.Normal, justAdded.Dist, justAdded.A, this, justAdded.pointA, justAdded.pointB);

                    circle.Pos = tempPos;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

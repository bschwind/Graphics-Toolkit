//Special thanks to Paul Firth:
//http://www.wildbunny.co.uk/blog/2011/04/06/physics-engines-for-dummies/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Bodies;
using GraphicsToolkit.Physics._2D.Geometry;
using GraphicsToolkit.Physics._2D.Partitions;
using GraphicsToolkit.Physics._2D.Constraints;

namespace GraphicsToolkit.Physics._2D
{
    public class PhysicsEngine2D
    {
        private Vector2 gravity = new Vector2(0, -0.1f);
        private List<RigidBody2D> bodies;
        private List<RigidBody2D> lines;
        private List<Constraint2D> constraints;
        private List<Contact2D> contacts = new List<Contact2D>();
        private Partition2D partition;

        public PhysicsEngine2D() : this(new GridPartition2D(Vector2.Zero, new Vector2(20, 10), 40, 40))
        {

        }

        public PhysicsEngine2D(Partition2D p)
        {
            bodies = new List<RigidBody2D>();
            lines = new List<RigidBody2D>();
            constraints = new List<Constraint2D>();

            partition = p;
        }

        public void AddRigidBody(RigidBody2D rb)
        {
            if (rb as LineBody != null)
            {
                lines.Add(rb);
            }
            else
            {
                bodies.Add(rb);
            }
        }

        public void AddConstraint(Constraint2D c)
        {
            constraints.Add(c);
        }

        public List<RigidBody2D> GetBodies()
        {
            return bodies;
        }

        public void SetGravity(Vector2 g)
        {
            gravity = g;
        }

        public void Update(GameTime g)
        {
            float dt = Math.Max((float)g.ElapsedGameTime.TotalSeconds, 1f / 60);
            //merged = bodies[0].MotionBounds;
            //Apply gravity to each body
            //Also apply external forces here, such as player input
            foreach (RigidBody2D rb in bodies)
            {
                //Only apply to moving objects
                if (rb.InvMass > 0)
                {
                    //Add in gravity, as well as any forces applied to our object
                    rb.Vel = rb.Vel + gravity + (rb.Force * rb.InvMass);
                }
                if (rb.InvInertia > 0)
                {
                    rb.RotVel = rb.RotVel + (rb.Torque * rb.InvInertia);
                }

                rb.ClearForces();
                rb.GenerateMotionAABB(dt);
            }

            //Detect and resolve contacts
            contacts.Clear();

            partition.GenerateContacts(ref bodies, ref contacts, dt);

            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = 0; j < lines.Count; j++)
                {
                    contacts.Add(bodies[i].GenerateContact(lines[j], dt));
                }
            }

            Solver2D.Solve(contacts, 1, dt);

            foreach (Constraint2D c in constraints)
            {
                c.Solve(dt);
            }

            //Integrate
            foreach (RigidBody2D rb in bodies)
            {
                rb.Integrate(dt);
            }
        }
    }
}

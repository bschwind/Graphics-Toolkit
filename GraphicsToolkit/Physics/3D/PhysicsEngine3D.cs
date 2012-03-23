using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Bodies;
using GraphicsToolkit.Physics._3D.Geometry;
using GraphicsToolkit.Physics._3D.Partitions;
using GraphicsToolkit.Physics._3D.Constraints;

namespace GraphicsToolkit.Physics._3D
{
    public class PhysicsEngine3D
    {
        private Vector3 gravity = new Vector3(0, -0.1f, 0);
        private List<RigidBody3D> bodies;
        private List<RigidBody3D> lines;
        private List<Constraint3D> constraints;
        private List<Contact3D> contacts = new List<Contact3D>();
        private Partition3D partition;

        public PhysicsEngine3D() : this(new GridPartition3D(Vector3.Zero, new Vector3(20, 10, 10), 40, 40))
        {

        }

        public PhysicsEngine3D(Partition3D p)
        {
            bodies = new List<RigidBody3D>();
            lines = new List<RigidBody3D>();
            constraints = new List<Constraint3D>();

            //By default, use a grid partition
            partition = p;
        }

        public void AddRigidBody(RigidBody3D rb)
        {
            if (rb as PlaneBody != null)
            {
                lines.Add(rb);
            }
            else
            {
                bodies.Add(rb);
            }
        }

        public void AddConstraint(Constraint3D c)
        {
            constraints.Add(c);
        }

        public List<RigidBody3D> GetBodies()
        {
            return bodies;
        }

        public void SetGravity(Vector3 g)
        {
            gravity = g;
        }

        public void Update(GameTime g)
        {
            float dt = Math.Max((float)g.ElapsedGameTime.TotalSeconds, 1f / 60);
            //merged = bodies[0].MotionBounds;
            //Apply gravity to each body
            //Also apply external forces here, such as player input
            foreach (RigidBody3D rb in bodies)
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

            Solver3D.Solve(contacts, 1, dt);

            foreach (Constraint3D c in constraints)
            {
                c.Solve(dt);
            }

            //Integrate
            foreach (RigidBody3D rb in bodies)
            {
                rb.Integrate(dt);
            }
        }
    }
}

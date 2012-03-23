using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsToolkit.Physics._3D.Partitions
{
    public class BruteForcePartition3D : Partition3D
    {
        public override void GenerateContacts(ref List<Bodies.RigidBody3D> bodies, ref List<Contact3D> contacts, float dt)
        {
            for (int i = 0; i < bodies.Count - 1; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    contacts.Add(bodies[i].GenerateContact(bodies[j], dt));
                }
            }
        }
    }
}

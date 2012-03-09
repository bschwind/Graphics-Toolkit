using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D;
using GraphicsToolkit.Physics._3D.Bodies;

namespace GraphicsToolkit.Physics._3D.Partitions
{
    //An abstract class for an object which takes a list of bodies and generates a list of contacts
    //For example, a grid, quadtree, octree, sweep-and-prune, bounding volume hierarchy, etc.
    public abstract class Partition3D
    {
        public abstract void GenerateContacts(ref List<RigidBody3D> bodies, ref List<Contact3D> contacts, float dt);
    }
}

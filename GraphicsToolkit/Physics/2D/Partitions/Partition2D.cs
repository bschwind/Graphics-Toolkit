using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D;
using GraphicsToolkit.Physics._2D.Bodies;

namespace GraphicsToolkit.Physics._2D.Partitions
{
    //An abstract class for an object which takes a list of bodies and generates a list of contacts
    //For example, a grid, quadtree, octree, sweep-and-prune, bounding volume hierarchy, etc.
    public abstract class Partition2D
    {
        public abstract void GenerateContacts(ref List<RigidBody2D> bodies, ref List<Contact2D> contacts, float dt);
    }
}

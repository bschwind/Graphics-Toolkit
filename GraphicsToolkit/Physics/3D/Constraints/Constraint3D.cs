using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Bodies;

namespace GraphicsToolkit.Physics._3D.Constraints
{
    public abstract class Constraint3D
    {
        protected RigidBody3D BodyA;
        protected RigidBody3D BodyB;

        public Constraint3D(RigidBody3D bodyA, RigidBody3D bodyB)
        {
            BodyA = bodyA;
            BodyB = bodyB;
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            BodyA.Vel += impulse * BodyA.InvMass;
            BodyB.Vel -= impulse * BodyB.InvMass;
        }

        public abstract void Solve(float dt);
    }
}

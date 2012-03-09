using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Bodies;

namespace GraphicsToolkit.Physics._2D.Constraints
{
    public abstract class Constraint2D
    {
        protected RigidBody2D BodyA;
        protected RigidBody2D BodyB;

        public Constraint2D(RigidBody2D bodyA, RigidBody2D bodyB)
        {
            BodyA = bodyA;
            BodyB = bodyB;
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            BodyA.Vel += impulse * BodyA.InvMass;
            BodyB.Vel -= impulse * BodyB.InvMass;
        }

        public abstract void Solve(float dt);
    }
}

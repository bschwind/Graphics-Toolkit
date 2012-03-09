using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Bodies;

namespace GraphicsToolkit.Physics._2D.Constraints
{
    public class DistanceConstraint2D : Constraint2D
    {
        private float distance;

        public DistanceConstraint2D(RigidBody2D bodyA, RigidBody2D bodyB, float dist)
            : base(bodyA, bodyB)
        {
            distance = dist;
        }

        public override void Solve(float dt)
        {
            Vector2 axis = BodyB.Pos - BodyA.Pos;
            float currentDist = axis.Length();
            axis *= (1f / currentDist);
            float relVel = Vector2.Dot(BodyB.Vel - BodyA.Vel, axis);
            float relDist = currentDist - distance;

            float remove = relVel + (relDist / dt);
            float impulse = remove / (BodyA.InvMass + BodyB.InvMass);

            Vector2 impVec = axis * impulse;

            ApplyImpulse(impVec);
        }
    }
}

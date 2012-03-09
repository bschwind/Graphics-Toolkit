using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Bodies;

namespace GraphicsToolkit.Physics._3D.Constraints
{
    public class DistanceConstraint3D : Constraint3D
    {
        private float distance;

        public DistanceConstraint3D(RigidBody3D bodyA, RigidBody3D bodyB, float dist)
            : base(bodyA, bodyB)
        {
            distance = dist;
        }

        public override void Solve(float dt)
        {
            Vector3 axis = BodyB.Pos - BodyA.Pos;
            float currentDist = axis.Length();
            axis *= (1f / currentDist);
            float relVel = Vector3.Dot(BodyB.Vel - BodyA.Vel, axis);
            float relDist = currentDist - distance;

            float remove = relVel + (relDist / dt);
            float impulse = remove / (BodyA.InvMass + BodyB.InvMass);

            Vector3 impVec = axis * impulse;

            ApplyImpulse(impVec);
        }
    }
}

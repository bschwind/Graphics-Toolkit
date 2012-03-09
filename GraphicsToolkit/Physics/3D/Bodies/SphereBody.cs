using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Geometry;

namespace GraphicsToolkit.Physics._3D.Bodies
{
    public class SphereBody : RigidBody3D
    {
        private float radius;

        public float Radius
        {
            get
            {
                return radius;
            }
        }

        public SphereBody(Vector3 pos, Vector3 vel, float mass, float radius)
            : base(pos, vel, 0f, mass, 1f)
        {
            this.radius = radius;
        }

        public override void GenerateMotionAABB(float dt)
        {
            Vector3 predictedPos = Pos + (Vel * dt);

            Vector3 center = (Pos + predictedPos) / 2f;
            Vector3 halfExtents = new Vector3((Math.Abs(predictedPos.X - Pos.X) * 0.5f) + radius, (Math.Abs(predictedPos.Y - Pos.Y) * 0.5f) + radius, (Math.Abs(predictedPos.Z - Pos.Z) * 0.5f) + radius);

            motionBounds = new AABB3D(center, halfExtents);
        }

        public override Contact3D GenerateContact(RigidBody3D rb, float dt)
        {
            //Profiler says this method takes up a lot of processing time (AKA, it's run quite often)
            if (rb as SphereBody != null)
            {
                SphereBody c = rb as SphereBody;
                Vector3 normal = rb.Pos - this.Pos;
                float normLen = normal.Length();
                float dist = normLen - (this.radius + c.radius);
                normal /= normLen;
                Vector3 pa = this.Pos + normal * this.radius;
                Vector3 pb = rb.Pos - normal * c.radius;

                return new Contact3D(normal, dist, this, rb, pa, pb);
            }
            else if (rb as PlaneBody != null)
            {
                PlaneBody pb = rb as PlaneBody;
                return pb.GenerateContact(this, dt);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

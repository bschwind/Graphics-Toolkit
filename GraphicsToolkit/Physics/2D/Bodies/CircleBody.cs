using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Geometry;

namespace GraphicsToolkit.Physics._2D.Bodies
{
    public class CircleBody : RigidBody2D
    {
        private float radius;

        public float Radius
        {
            get
            {
                return radius;
            }
        }

        public CircleBody(Vector2 pos, Vector2 vel, float mass, float radius)
            : base(pos, vel, 0f, mass, 1f)
        {
            this.radius = radius;
        }

        public override void GenerateMotionAABB(float dt)
        {
            Vector2 predictedPos = Pos + (Vel * dt);

            Vector2 center = (Pos + predictedPos) / 2f;
            Vector2 halfExtents = new Vector2((Math.Abs(predictedPos.X-Pos.X) * 0.5f) + radius, (Math.Abs(predictedPos.Y-Pos.Y) * 0.5f) + radius);

            motionBounds = new AABB2D(center, halfExtents);
        }

        public override Contact2D GenerateContact(RigidBody2D rb, float dt)
        {
            //Profiler says this method takes up a lot of processing time (AKA, it's run quite often)
            if (rb as CircleBody != null)
            {
                CircleBody c = rb as CircleBody;
                Vector2 normal = rb.Pos - this.Pos;
                float normLen = normal.Length();
                float dist = normLen - (this.radius + c.radius);
                normal /= normLen;
                Vector2 pa = this.Pos + normal * this.radius;
                Vector2 pb = rb.Pos - normal * c.radius;

                return new Contact2D(normal, dist, this, rb, pa, pb);
            }
            else if (rb as LineBody != null)
            {
                LineBody pb = rb as LineBody;
                return pb.GenerateContact(this, dt);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._2D.Geometry;

namespace GraphicsToolkit.Physics._2D.Bodies
{
    public class LineBody : RigidBody2D
    {
        private Vector2 normal;
        private Vector2 p;

        public Vector2 Normal
        {
            get
            {
                return normal;
            }
        }

        public Vector2 P
        {
            get
            {
                return p;
            }
        }

        public LineBody(Vector2 normal, Vector2 p)
            : base(p, Vector2.Zero, 0f, 0, 0f)
        {
            this.normal = Vector2.Normalize(normal);
            this.p = p;

            //A line extends infinitely, so make our AABB as big as possible
            motionBounds = new AABB2D(Vector2.Zero, new Vector2(float.MaxValue, float.MaxValue));
        }

        public float DistanceToPoint(Vector2 q)
        {
            Vector2 d = q - p;
            float dist = Vector2.Dot(d, normal);
            return dist;
        }

        public Vector2 ProjectPointOntoPlane(Vector2 q)
        {
            Vector2 d = q - p;
            float dist = Vector2.Dot(d, normal);
            return q - Vector2.Normalize(d) * dist;
        }

        public override void GenerateMotionAABB(float dt)
        {
            //Do nothing. These lines will never move!
        }

        public override Contact2D GenerateContact(RigidBody2D rb, float dt)
        {
            if (rb as CircleBody != null)
            {
                CircleBody c = rb as CircleBody;
                float dist = DistanceToPoint(c.Pos) - c.Radius;

                Vector2 pointOnPlane = c.Pos - (normal * (dist+c.Radius));
                Vector2 pointOnBall = c.Pos - (normal * c.Radius);

                return new Contact2D(normal, dist, this, rb, pointOnPlane, pointOnBall);
            }
            else if (rb as LineBody != null)
            {
                return new Contact2D();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GraphicsToolkit.Physics._3D.Geometry;

namespace GraphicsToolkit.Physics._3D.Bodies
{
    public class PlaneBody : RigidBody3D
    {
        private Vector3 normal;
        private Vector3 p;

        public Vector3 PlaneNormal
        {
            get
            {
                return normal;
            }
        }

        public Vector3 P
        {
            get
            {
                return p;
            }
        }

        public PlaneBody(Vector3 normal, Vector3 p)
            : base(p, Vector3.Zero, 0f, 0, 0f)
        {
            this.normal = Vector3.Normalize(normal);
            this.p = p;

            //A line extends infinitely, so make our AABB as big as possible
            motionBounds = new AABB3D(Vector3.Zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
        }

        public float DistanceToPoint(Vector3 q)
        {
            Vector3 d = q - p;
            float dist = Vector3.Dot(d, normal);
            return dist;
        }

        public Vector3 ProjectPointOntoPlane(Vector3 q)
        {
            Vector3 d = q - p;
            float dist = Vector3.Dot(d, normal);
            return q - Vector3.Normalize(d) * dist;
        }

        public override void GenerateMotionAABB(float dt)
        {
            //Do nothing. These lines will never move!
        }

        public override Contact3D GenerateContact(RigidBody3D rb, float dt)
        {
            if (rb as SphereBody != null)
            {
                SphereBody c = rb as SphereBody;
                float dist = DistanceToPoint(c.Pos) - c.Radius;

                Vector3 pointOnPlane = c.Pos - (normal * (dist+c.Radius));
                Vector3 pointOnBall = c.Pos - (normal * c.Radius);

                return new Contact3D(normal, dist, this, rb, pointOnPlane, pointOnBall);
            }
            else if (rb as PlaneBody != null)
            {
                return new Contact3D();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

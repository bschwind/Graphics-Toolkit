using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Physics._3D.Geometry
{
    public struct Sphere
    {
        public Vector3 Pos;
        public float Radius;

        public Sphere(Vector3 pos, float radius)
        {
            Pos = pos;
            Radius = radius;
        }

        public static Sphere CreateFromPoints(Vector3[] points)
        {
            Sphere sphere = new Sphere();
            AABB3D box = AABB3D.CreateFromPoints(points);
            sphere.Pos = box.Pos;
            float maxDist = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                if ((points[i] - sphere.Pos).LengthSquared() > maxDist)
                {
                    maxDist = (points[i] - sphere.Pos).LengthSquared();
                }
            }

            maxDist = (float)Math.Sqrt(maxDist);

            sphere.Radius = maxDist;

            return sphere;
        }
    }
}

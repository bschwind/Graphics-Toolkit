using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Physics._2D.Geometry
{
    public struct Circle2D
    {
        public Vector2 Pos;
        public float Radius;

        public Circle2D(Vector2 pos, float radius)
        {
            Pos = pos;
            Radius = radius;
        }

        public static Circle2D CreateFromPoints(Vector2[] points)
        {
            Circle2D circle = new Circle2D();
            AABB2D box = AABB2D.CreateFromPoints(points);
            circle.Pos = box.Pos;
            float maxDist = float.MinValue;

            for (int i = 0; i < points.Length; i++)
            {
                if ((points[i] - circle.Pos).LengthSquared() > maxDist)
                {
                    maxDist = (points[i] - circle.Pos).LengthSquared();
                }
            }

            maxDist = (float)Math.Sqrt(maxDist);

            circle.Radius = maxDist;

            return circle;
        }
    }
}

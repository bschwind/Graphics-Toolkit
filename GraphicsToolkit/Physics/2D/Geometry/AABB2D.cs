using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Physics._2D.Geometry
{
    public struct AABB2D
    {
        public Vector2 Pos;
        public Vector2 HalfExtents;

        public AABB2D(Vector2 pos, Vector2 halfExtents)
        {
            Pos = pos;
            HalfExtents = halfExtents;
        }

        public Vector2 GetMin()
        {
            return Pos - HalfExtents;
        }

        public Vector2 GetMax()
        {
            return Pos + HalfExtents;
        }

        public void Inflate(float amount)
        {
            HalfExtents.X += amount;
            HalfExtents.Y += amount;
        }

        public static AABB2D CreateFromMinMax(Vector2 min, Vector2 max)
        {
            min = Vector2.Min(min, max);
            max = Vector2.Max(min, max);

            Vector2 pos = new Vector2((min.X + max.X) / 2f, (min.Y + max.Y) / 2f);
            Vector2 halfExtents = new Vector2((max.X - min.X) / 2f, (max.Y - min.Y) / 2f);

            return new AABB2D(pos, halfExtents);
        }

        public static AABB2D CreateFromPoints(Vector2[] points)
        {
            Vector2 min = points[0];
            Vector2 max = min;

            for (int i = 0; i < points.Length; i++)
            {
                min = Vector2.Min(min, points[i]);
                max = Vector2.Max(max, points[i]);
            }

            return CreateFromMinMax(min, max);
        }

        public static AABB2D CreateMerged(AABB2D a, AABB2D b)
        {
            Vector2 min = Vector2.Min(a.Pos - a.HalfExtents, b.Pos - b.HalfExtents);
            Vector2 max = Vector2.Max(a.Pos + a.HalfExtents, b.Pos + b.HalfExtents);

            return CreateFromMinMax(min, max);
        }

        public bool Intersects(AABB2D b)
        {
            if (Math.Abs(this.Pos.X - b.Pos.X) > (this.HalfExtents.X + b.HalfExtents.X)) return false;
            if (Math.Abs(this.Pos.Y - b.Pos.Y) > (this.HalfExtents.Y + b.HalfExtents.Y)) return false;

            return true;
        }
    }
}

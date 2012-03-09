using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Physics._3D.Geometry
{
    public struct AABB3D
    {
        public Vector3 Pos;
        public Vector3 HalfExtents;

        public AABB3D(Vector3 pos, Vector3 halfExtents)
        {
            Pos = pos;
            HalfExtents = halfExtents;
        }

        public Vector3 GetMin()
        {
            return Pos - HalfExtents;
        }

        public Vector3 GetMax()
        {
            return Pos + HalfExtents;
        }

        public void Inflate(float amount)
        {
            HalfExtents.X += amount;
            HalfExtents.Y += amount;
        }

        public static AABB3D CreateFromMinMax(Vector3 min, Vector3 max)
        {
            min = Vector3.Min(min, max);
            max = Vector3.Max(min, max);

            Vector3 pos = new Vector3((min.X + max.X) / 2f, (min.Y + max.Y) / 2f, (min.Z + max.Z) / 2f);
            Vector3 halfExtents = new Vector3((max.X - min.X) / 2f, (max.Y - min.Y) / 2f, (max.Z - min.Z) / 2f);

            return new AABB3D(pos, halfExtents);
        }

        public static AABB3D CreateFromPoints(Vector3[] points)
        {
            Vector3 min = points[0];
            Vector3 max = min;

            for (int i = 0; i < points.Length; i++)
            {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            return CreateFromMinMax(min, max);
        }

        public static AABB3D CreateMerged(AABB3D a, AABB3D b)
        {
            Vector3 min = Vector3.Min(a.Pos - a.HalfExtents, b.Pos - b.HalfExtents);
            Vector3 max = Vector3.Max(a.Pos + a.HalfExtents, b.Pos + b.HalfExtents);

            return CreateFromMinMax(min, max);
        }

        public bool Intersects(AABB3D b)
        {
            if (Math.Abs(this.Pos.X - b.Pos.X) > (this.HalfExtents.X + b.HalfExtents.X)) return false;
            if (Math.Abs(this.Pos.Y - b.Pos.Y) > (this.HalfExtents.Y + b.HalfExtents.Y)) return false;

            return true;
        }
    }
}

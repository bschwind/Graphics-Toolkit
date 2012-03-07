using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsToolkit.Graphics
{
    public class Mesh
    {
        public VertexBuffer Vertices;
        public IndexBuffer Indices;
        public Texture2D Texture;

        public BoundingSphere CalculateBounds()
        {
            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[Vertices.VertexCount];
            Vertices.GetData<VertexPositionNormalTexture>(verts);
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < verts.Length; i++)
            {
                points.Add(verts[i].Position);
            }

            BoundingSphere s = BoundingSphere.CreateFromPoints(points);
            s.Radius += 0.08f;
            return s;
        }
    }
}

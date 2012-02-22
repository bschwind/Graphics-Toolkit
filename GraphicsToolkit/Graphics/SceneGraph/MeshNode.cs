using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsToolkit.Graphics.SceneGraph
{
    public class MeshNode : SceneNode
    {
        private Mesh mesh;
        private BoundingSphere bounds;

        public MeshNode(Mesh m)
        {
            mesh = m;
            bounds = mesh.CalculateBounds();
        }

        public BoundingSphere GetBounds()
        {
            return bounds;
        }

        protected override void InternalDraw(GameTime g, Matrix absoluteTransform, PrimitiveBatch batch, Camera cam)
        {
            base.InternalDraw(g, absoluteTransform, batch, cam);
            bounds.Center = absoluteTransform.Translation;
            batch.DrawMesh(mesh, absoluteTransform, cam);
        }
    }
}

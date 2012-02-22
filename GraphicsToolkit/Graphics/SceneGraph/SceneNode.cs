using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsToolkit.Graphics.SceneGraph
{
    public class SceneNode
    {
        private List<SceneNode> children;
        private Matrix localTransform, pivotTransform, absoluteTransform;
        private Vector3 pos, scl;
        private Vector3 pivot;
        private Quaternion rot;

        public SceneNode()
        {
            children = new List<SceneNode>();
            pos = Vector3.Zero;
            scl = Vector3.One;
            rot = Quaternion.Identity;

            SetPivot(Vector3.Zero);
        }

        public void AddChild(SceneNode n)
        {
            children.Add(n);
        }

        public void RemoveChild(SceneNode n)
        {
            children.Remove(n);
        }

        public void SetPos(Vector3 p)
        {
            pos = p;
        }

        public Vector3 GetPos()
        {
            return pos;
        }

        public Matrix GetAbsoluteTransform()
        {
            return absoluteTransform;
        }

        public void SetScl(Vector3 s)
        {
            scl = s;
        }

        public void SetRotation(Quaternion q)
        {
            rot = q;
        }

        public void SetPivot(Vector3 p)
        {
            pivot = p;
            pivotTransform = Matrix.CreateTranslation(-pivot);
        }

        public void Draw(GameTime g, Matrix parentTransform, PrimitiveBatch batch, Camera cam)
        {
            localTransform = Matrix.CreateScale(scl) * Matrix.CreateFromQuaternion(rot) * Matrix.CreateTranslation(pos);
            absoluteTransform = pivotTransform * localTransform * parentTransform;
            InternalDraw(g, absoluteTransform, batch, cam);

            foreach (SceneNode n in children)
            {
                n.Draw(g,  Matrix.Invert(pivotTransform) * absoluteTransform, batch, cam);
            }
        }

        protected virtual void InternalDraw(GameTime g, Matrix absoluteTransform, PrimitiveBatch batch, Camera cam)
        {

        }
    }
}

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
        private Matrix localTransform;
        private Vector3 pos, scl;
        private Quaternion rot;

        public SceneNode()
        {
            children = new List<SceneNode>();
            pos = Vector3.Zero;
            scl = Vector3.One;
            rot = Quaternion.Identity;
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

        public void SetScl(Vector3 s)
        {
            scl = s;
        }

        public void SetRotation(Quaternion q)
        {
            rot = q;
        }

        public void Draw(GameTime g, Matrix parentTransform, PrimitiveBatch batch, Camera cam)
        {
            localTransform = Matrix.CreateScale(scl) * Matrix.CreateFromQuaternion(rot) * Matrix.CreateTranslation(pos);
            Matrix finalTransform = localTransform * parentTransform;
            InternalDraw(g, finalTransform, batch, cam);

            foreach (SceneNode n in children)
            {
                n.Draw(g, finalTransform, batch, cam);
            }
        }

        protected virtual void InternalDraw(GameTime g, Matrix absoluteTransform, PrimitiveBatch batch, Camera cam)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraphicsToolkit.Graphics
{
    //Used for keeping track of the view information
    public class Camera
    {
        protected Matrix view, projection;
        protected float nearPlane = 0.1f;
        protected float farPlane = 1000f;

        public Camera()
        {

        }

        public Matrix View
        {
            get
            {
                return view;
            }
        }

        public Matrix Projection
        {
            get
            {
                return projection;
            }
        }

        public virtual void Resize()
        {

        }

        public virtual void Update(GameTime g)
        {

        }
    }
}
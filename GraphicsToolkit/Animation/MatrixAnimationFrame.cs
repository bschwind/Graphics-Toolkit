using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Animation
{
    public struct MatrixAnimationFrame
    {
        public Matrix[] Transforms;

        public MatrixAnimationFrame(Matrix[] matrices)
        {
            Transforms = matrices;
        }
    }
}

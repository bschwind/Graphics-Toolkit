using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Animation
{
    public struct MatrixAnimationFrame
    {
        public float roll, pitch, yaw;

        public MatrixAnimationFrame(float roll, float pitch, float yaw)
        {
            this.roll = roll;
            this.pitch = pitch;
            this.yaw = yaw;
        }
    }
}

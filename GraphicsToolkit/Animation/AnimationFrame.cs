using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Animation
{
    public struct AnimationFrame
    {
        public int FrameNumber;
        public float Time;
        public Vector3[] Positions;

        public AnimationFrame(int frameNumber, float time, Vector3[] positions)
        {
            FrameNumber = frameNumber;
            Time = time;
            Positions = positions;
        }
    }
}

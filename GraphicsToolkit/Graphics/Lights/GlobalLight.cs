using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Graphics.Lights
{
    public struct GlobalLight
    {
        public Vector3 Direction;
        public Color Color;
        public float Intensity;

        public GlobalLight(Vector3 dir, Color color, float intensity)
        {
            Direction = dir;
            Direction.Normalize();
            Color = color;
            Intensity = intensity;
        }
    }
}

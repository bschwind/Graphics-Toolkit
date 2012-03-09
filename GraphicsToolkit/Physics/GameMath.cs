using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GraphicsToolkit.Physics
{
    public class GameMath
    {
        public static Vector2 Project2D(Vector2 a, Vector2 b)
        {
            return (Vector2.Dot(a, b) / Vector2.Dot(b, b)) * b;
        }

        public static Vector2 Perp2D(Vector2 a)
        {
            return new Vector2(a.Y, -a.X);
        }
    }
}

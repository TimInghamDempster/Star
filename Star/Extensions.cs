using Microsoft.Xna.Framework;
using System;

namespace Star
{
    internal static class Extensions
    {
        internal static Vector3 Sqrt(this Vector3 v) => 
            new Vector3(
                (float)Math.Sqrt(v.X),
                (float)Math.Sqrt(v.Y), 
                (float)Math.Sqrt(v.Z));

        internal static Vector3 Div(this Vector3 v, float f) => v/f;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class LineUtilities2f
    {
        /// <summary>
        /// Reflects the point p about the line v0 -> v1
        /// http://en.wikipedia.org/wiki/Reflection_(mathematics)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector2f Reflect( Vector2f p, Vector2f v0, Vector2f v1 )
        {
            // find the orthogonal vector
            var a = ( v1 - v0 ).OrthogonalVector();

            // moves p in the direction of -a
            // by twice the projection of p onto a
            return p - 2 * a * Vector2f.Dot( p - v0, a ) / Vector2f.Dot( a, a );
        }
    }
}

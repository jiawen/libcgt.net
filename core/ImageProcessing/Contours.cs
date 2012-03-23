using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    public static class Contours
    {
        public static Vector2f SnapToEdge( Image1f gradientNorm, Vector2f p, int radius )
        {
            Vector2i q = p.RoundToInt();
            float max = float.NegativeInfinity;
            Vector2f maxQ = Vector2f.Zero;

            for( int y = q.y - radius; y <= q.y + radius; ++y )
            {
                for( int x = q.x - radius; x <= q.x + radius; ++x )
                {
                    float gn = gradientNorm[ x, y ];
                    if( gn > max )
                    {
                        max = gn;
                        maxQ = new Vector2f( x, y );
                    }
                }
            }

            return maxQ;
        }
    }
}

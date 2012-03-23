using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class Color4ub
    {
        public static Vector4ub TransparentBlack = new Vector4ub( 0, 0, 0, 0 );

        public static Vector4ub Blue = new Vector4ub( 0, 0, 255, 255 );
        public static Vector4ub Green = new Vector4ub( 0, 255, 0, 255 );
        public static Vector4ub Red = new Vector4ub( 255, 0, 0, 255 );

        public static Vector4ub AliceBlue = new Vector4ub( 240, 248, 255, 255 );
        public static Vector4ub AntiqueWhite = new Vector4ub( 250, 235, 215, 255 );
        public static Vector4ub Black = new Vector4ub( 0, 0, 0, 255 );
        public static Vector4ub BlanchedAlmond = new Vector4ub( 255, 235, 205, 255 );
        public static Vector4ub Chartreuse = new Vector4ub( 127, 255, 0, 255 );
        public static Vector4ub CornflowerBlue = new Vector4ub( 100, 149, 237, 255 );
        public static Vector4ub Cyan = new Vector4ub( 0, 255, 255, 255 );
        public static Vector4ub Grey = new Vector4ub( 128, 128, 128, 255 );
        public static Vector4ub Indigo = new Vector4ub( 75, 0, 130, 255 );
        public static Vector4ub Magenta = new Vector4ub( 255, 0, 255, 255 );
        public static Vector4ub Orange = new Vector4ub( 255, 165, 0, 255 );
        public static Vector4ub OrangeRed = new Vector4ub( 255, 69, 0, 255 );
        public static Vector4ub Purple = new Vector4ub( 128, 0, 128, 255 );
        public static Vector4ub Violet = new Vector4ub( 238, 130, 238, 255 );
        public static Vector4ub Wheat = new Vector4ub( 245, 222, 179, 255 );
        public static Vector4ub White = new Vector4ub( 255, 255, 255, 255 );
        public static Vector4ub Yellow = new Vector4ub( 255, 255, 0, 255 );

        public static Vector4ub[] GenerateRandomColorMap( Random random, int length )
        {
            var colorMap = new Vector4ub[ length ];

            for( int i = 0; i < length; ++i )
            {
                colorMap[ i ] = random.NextVector4ub();
            }

            return colorMap;
        }

        public static Vector4ub[] GenerateRandomOpaqueColorMap( Random random, int length )
        {
            return GenerateRandomColorMapWithAlpha( random, length, 255 );
        }

        public static Vector4ub[] GenerateRandomColorMapWithAlpha( Random random, int length, byte alpha )
        {
            var colorMap = GenerateRandomColorMap( random, length );
            for( int i = 0; i < colorMap.Length; ++i )
            {
                colorMap[ i ].w = alpha;
            }
            return colorMap;
        }
    }
}

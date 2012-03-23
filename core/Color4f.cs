using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class Color4f
    {
        public static Vector4f TransparentBlack = new Vector4f( 0, 0, 0, 0 );

        public static Vector4f Blue = new Vector4f( 0, 0, 1, 1 );
        public static Vector4f Green = new Vector4f( 0, 1, 0, 1 );
        public static Vector4f Red = new Vector4f( 1, 0, 0, 1 );

        public static Vector4f AliceBlue = new Vector4f( 0.941f, 0.973f, 1, 1 );
        public static Vector4f AntiqueWhite = new Vector4f( 0.980f, 0.922f, 0.843f, 1 );
        public static Vector4f Black = new Vector4f( 0, 0, 0, 1 );
        public static Vector4f BlanchedAlmond = new Vector4f( 1, 0.922f, 0.804f, 1 );
        public static Vector4f Chartreuse = new Vector4f( 0.498f, 1, 0, 1 );
        public static Vector4f CornflowerBlue = new Vector4f( 0.392f, 0.584f, 0.929f, 1 );
        public static Vector4f Cyan = new Vector4f( 0, 1, 1, 1 );
        public static Vector4f Grey = new Vector4f( 0.5f, 0.5f, 0.5f, 1 );
        public static Vector4f Indigo = new Vector4f( 0.294f, 0, 0.510f, 1 );
        public static Vector4f Magenta = new Vector4f( 1, 0, 1, 1 );
        public static Vector4f Orange = new Vector4f( 1, 0.647f, 0, 1 );
        public static Vector4f OrangeRed = new Vector4f( 1, 0.271f, 0, 1 );
        public static Vector4f Purple = new Vector4f( 0.5f, 0, 0.5f, 1 );
        public static Vector4f Violet = new Vector4f( 0.933f, 0.510f, 0.933f, 1 );
        public static Vector4f Wheat = new Vector4f( 0.961f, 0.871f, 0.702f, 1 );
        public static Vector4f White = new Vector4f( 1, 1, 1, 1 );
        public static Vector4f Yellow = new Vector4f( 1, 1, 0, 1 );

        public static Vector4f[] GenerateRandomColorMap( Random random, int length )
        {
            var colorMap = new Vector4f[ length ];

            for( int i = 0; i < length; ++i )
            {
                colorMap[ i ] = random.NextVector4f();
            }

            return colorMap;
        }

        public static Vector4f[] GenerateRandomOpaqueColorMap( Random random, int length )
        {
            return GenerateRandomColorMapWithAlpha( random, length, 1.0f );
        }

        public static Vector4f[] GenerateRandomColorMapWithAlpha( Random random, int length, float alpha )
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

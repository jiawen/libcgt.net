using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    /// <summary>
    /// Convenient extensions to the Random class for floats, vectors, and ranges
    /// </summary>
    public static class RandomExtensions
    {
        public static bool NextBool( this Random random )
        {
            return ( random.Next() % 2 == 0 );
        }

        public static int NextBit( this Random random )
        {
            return ( random.Next() % 2 );
        }

        public static float NextFloat( this Random random )
        {
            return ( float )( random.NextDouble() );
        }

        public static float NextFloat( this Random random, float min, float max )
        {
            var u = random.NextFloat();
            return min + ( max - min ) * u;
        }

        public static Vector2f NextVector2f( this Random random )
        {
            return new Vector2f( random.NextFloat(), random.NextFloat() );
        }

        public static Vector3f NextVector3f( this Random random )
        {
            return new Vector3f( random.NextFloat(), random.NextFloat(), random.NextFloat() );
        }

        public static Vector4f NextVector4f( this Random random )
        {
            return new Vector4f( random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat() );
        }

        public static Vector4ub NextVector4ub( this Random random )
        {
            byte r = ( byte ) ( random.Next( 256 ) );
            byte g = ( byte ) ( random.Next( 256 ) );
            byte b = ( byte ) ( random.Next( 256 ) );
            byte a = ( byte ) ( random.Next( 256 ) );

            return new Vector4ub( r, g, b, a );
        }
    }
}

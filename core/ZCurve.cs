using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class ZCurve
    {
        /// <summary>
        /// Given a (not necessarily power-of-two sized) rectangle of width by height
        /// Returns list of indices mapping the linear coordinate (row-major) into
        /// z order.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static List< Vector2i > GenerateLinearToZ( int width, int height )
        {
            int length = Math.Max( width, height );
            int l = Arithmetic.RoundUpToNearestPowerOfTwo( length );

            var output = new List< Vector2i >( l );

            int n = l * l;
            for( int i = 0; i < n; ++i )
            {
                var xy = LinearToZ( i );
                if( xy.x < width && xy.y < height )
                {
                    output.Add( xy );
                }
            }
            return output;
        }
        
        public static Vector2i LinearToZ( int idx )
        {
            throw new NotImplementedException( "finish the rest of the bits.  do the reverse mapping, do 64-bit" );

            int i = ( ( idx & 0x01 ) >> 0 ) | ( ( idx & 0x04 ) >> 1 ) | ( ( idx & 0x10 ) >> 2 ) |
                    ( ( idx & 0x40 ) >> 3 ) | ( ( idx & 0x100 ) >> 4 ) | ( ( idx & 0x400 ) >> 5 );
            int j = ( ( idx & 0x02 ) >> 1 ) | ( ( idx & 0x08 ) >> 2 ) | ( ( idx & 0x20 ) >> 3 ) |
                    ( ( idx & 0x80 ) >> 4 ) | ( ( idx & 0x200 ) >> 5 ) | ( ( idx & 0x800 ) >> 6 );

            return new Vector2i( i, j );
        }        
    }
}

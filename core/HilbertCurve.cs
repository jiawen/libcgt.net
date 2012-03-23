using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class HilbertCurve
    {
        /// <summary>
        /// Given an origin, the x-axis, y-axis, and the order of the curve
        /// Returns a list of points in 2D along the hilbert curve.
        /// The length of the returned list is 2^order.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List< Vector2f > GenerateFloat2( Vector2f origin,
            Vector2f xAxis, Vector2f yAxis, int order )
        {
            int sideLength = ( 1 << order );
            int capacity = sideLength * sideLength;
            var output = new List< Vector2f >( capacity );
            HilbertFloat( origin.x, origin.y, xAxis.x, xAxis.y, yAxis.x, yAxis.y, order, output );
            return output;
        }

        public static List< Vector2i > GenerateInt2( int order )
        {
            throw new NotImplementedException( "off-by-one bugs!" );
            /*
            int sideLength = ( 1 << order );
            int capacity = sideLength * sideLength;
            var output = new List< Vector2i >( capacity );

            HilbertInt( 0, 0, sideLength, 0, 0, sideLength, order, output );
            return output;
            */
        }

        private static void HilbertFloat( float x0, float y0,
            float xi, float xj,
            float yi, float yj,
            int n,
            List< Vector2f > output )
        {
            if( n <= 0 )
            {
                float x = x0 + ( xi + yi ) / 2;
                float y = y0 + ( xj + yj ) / 2;
                
                output.Add( new Vector2f( x, y ) );
            }
            else
            {
                HilbertFloat( x0, y0, yi / 2, yj / 2, xi / 2, xj / 2, n - 1, output );
                HilbertFloat( x0 + xi / 2, y0 + xj / 2, xi / 2, xj / 2, yi / 2, yj / 2, n - 1, output );
                HilbertFloat( x0 + xi / 2 + yi / 2, y0 + xj / 2 + yj / 2, xi / 2, xj / 2, yi / 2, yj / 2, n - 1, output );
                HilbertFloat( x0 + xi / 2 + yi, y0 + xj / 2 + yj, -yi / 2, -yj / 2, -xi / 2, -xj / 2, n - 1, output );
            }
        }

        private static void HilbertInt( int x0, int y0,
            int xi, int xj,
            int yi, int yj,
            int n,
            List< Vector2i > output )
        {
            if( n <= 0 )
            {
                int x = x0 + xi / 2 + yi / 2;
                int y = y0 + xj / 2 + yj / 2;
                output.Add( new Vector2i( x, y ) );
            }
            else
            {
                HilbertInt( x0, y0,
                    yi / 2, yj / 2,
                    xi / 2, xj / 2,
                    n - 1, output );

                HilbertInt( x0 + xi / 2, y0 + xj / 2,
                    xi / 2, xj / 2,
                    yi / 2, yj / 2,
                    n - 1, output );

                HilbertInt( x0 + xi / 2 + yi / 2, y0 + xj / 2 + yj / 2,
                    xi / 2, xj / 2,
                    yi / 2, yj / 2,
                    n - 1, output );

                HilbertInt( x0 + xi / 2 + yi - 1, y0 + xj / 2 + yj - 1,
                    -yi / 2, -yj / 2,
                    -xi / 2, -xj / 2,
                    n - 1, output );
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class MathUtils
    {
        public static readonly Random GlobalRandomSeededTime = new Random();
        public static readonly Random GlobalRandomSeededZero = new Random( 0 );

        public const float QUARTER_PI = ( float )( 0.25 * Math.PI );
        public const float HALF_PI = ( float )( 0.5 * Math.PI );
        public const float THREE_QUARTERS_PI = ( float )( 0.75 * Math.PI );
        public const float PI = ( float )( Math.PI );
        public const float TWO_PI = ( float )( 2.0 * Math.PI );

        public static readonly float ROOT_TWO = Sqrt( 2.0f );
        public static readonly float HALF_ROOT_TWO = 0.5f * ROOT_TWO;

        public static float ClampToRange( float t, float min, float max )
        {
            if( t < min )
            {
                t = min;
            }

            if( t > max )
            {
                t = max;
            }

            return t;
        }

        /// <summary>
        /// Convenience method for float sqrt
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float Sqrt( float f )
        {
            return ( float )( Math.Sqrt( f ) );
        }

        /// <summary>
        /// Convenience method for integer square root (rounds down).
        /// TODO: replace with a numerically robust one:
        /// http://www.azillionmonkeys.com/qed/sqroot.html
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int IntegerSqrt( float f )
        {
            return ( int )( Math.Sqrt( f ) );
        }

        public static float Log( float x )
        {
            return ( float ) ( Math.Log( x ) );
        }

        public static float Log( float x, float newBase )
        {
            return ( float ) ( Math.Log( x, newBase ) );
        }

        public static float Cos( float x )
        {
            return ( float ) Math.Cos( x );
        }

        public static float Sin( float x )
        {
            return ( float ) Math.Sin( x );
        }

        public static float Tan( float x )
        {
            return ( float ) Math.Tan( x );
        }

        public static float Acos( float x )
        {
            return ( float ) ( Math.Atan( x ) );
        }

        public static float Asin( float x )
        {
            return ( float ) ( Math.Atan( x ) );
        }

        public static float Atan( float x )
        {
            return ( float ) ( Math.Atan( x ) );
        }

        public static float Atan2( float y, float x )
        {
            return ( float ) ( Math.Atan2( y, x ) );
        }

        /// <summary>
        /// Cotangent
        /// </summary>
        /// <returns></returns>
        public static float Cot( float x )
        {
            return ( float ) Cot( ( double )x );
        }

        public static double Cot( double x )
        {
            return Math.Cos( x ) / Math.Sin( x );
        }

        public static float Exp( float x )
        {
            return ( float ) ( Math.Exp( x ) );
        }

        public static float Pow( float x, float p )
        {
            return ( float ) ( Math.Pow( x, p ) );
        }

        public static float Round( float f )
        {
            return ( float )( Math.Round( f ) );
        }

        /// <summary>
        /// Generates 100 linearly spaced points between a and b.
        /// a and b are included in the range, a does not have to be less than b.
        /// </summary>
        /// <returns></returns>
        public static float[] LinSpace( float a, float b )
        {
            return LinSpace( a, b, 100 );
        }

        /// <summary>
        /// Generates n linearly spaced points between a and b.
        /// a and b are included in the range, a does not have to be less than b.
        /// </summary>
        /// <returns></returns>
        public static float[] LinSpace( float a, float b, int n )
        {
            float[] output = new float[ n ];
            float delta = ( b - a ) / ( n - 1 );

            for( int i = 0; i < n - 1; ++i )
            {
                output[ i ] = a + i * delta;
            }
            output[ n - 1 ] = b;

            return output;
        }

        /// <summary>
        /// Given a range [a,b], generates n evenly spaced samples
        /// between a and b such that each sample lies in the *center* of the bin        
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static float[] GenerateSamplesAtBinCenter( float a, float b, int n )
        {
            float[] output = new float[ n ];
            float delta = ( b - a ) / n;

            float a0 = a + 0.5f * delta;
            for( int i = 0; i < n; ++i )
            {
                output[ i ] = a0 + i * delta;
            }

            return output;
        }

        /// <summary>
        /// Given a range a and b, the bottom left and top right of a rectangle,
        /// generates nx by ny evenly spaced samples within the rectangle
        /// such that each sample lies in the *center* of the bin        
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Array2D< Vector2f > GenerateSamplesAtBinCenter( Vector2f a, Vector2f b, int nx, int ny )
        {
            var output = new Array2D< Vector2f >( nx, ny );
            float dx = ( b.x - a.x ) / nx;
            float dy = ( b.y - a.y ) / ny;

            float a0x = a.x + 0.5f * dx;
            float a0y = a.y + 0.5f * dy;

            for( int y = 0; y < ny; ++y )
            {
                for( int x = 0; x < nx; ++x )
                {
                    output[ x, y ] = new Vector2f( a0x + x * dx, a0y + y * dy );
                }
            }

            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class Arithmetic
    {
        /// <summary>
        /// Generates an array of count integers starting from 0
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] IntegerSlice( int count )
        {
            return IntegerSlice( 0, count );
        }
       
        /// <summary>
        /// Generates an array of count integers starting from i0
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] IntegerSlice( int i0, int count )
        {
            int[] slice = new int[ count ];
            for( int i = 0; i < count; ++i )
            {
                slice[ i ] = i0 + i;
            }
            return slice;
        }

        /// <summary>
        /// Generates an List of count integers starting from i0
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List< int > IntegerSliceList( int i0, int count )
        {
            var slice = new List< int >( count );
            for( int i = 0; i < count; ++i )
            {
                slice.Add( i0 + i );
            }
            return slice;
        }

        public static int Clamp( this int x, int loInclusive, int hiExclusive )
        {
            if( x < loInclusive )
            {
                x = loInclusive;
            }
            else if( x >= hiExclusive )
            {
                x = hiExclusive - 1;
            }

            return x;
        }

        public static float Clamp( this float x, float lo, float hi )
        {
            if( x < lo )
            {
                x = lo;
            }
            if( x > hi )
            {
                x = hi;
            }

            return x;
        }        

        public static Vector4f Clamp( this Vector4f v, float lo, float hi )
        {
            return new Vector4f( v.x.Clamp( lo, hi ), v.y.Clamp( lo, hi ), v.z.Clamp( lo, hi ), v.w.Clamp( lo, hi ) );
        }

        public static Vector2f ClampNorm( this Vector2f v, float lo, float hi )
        {
            var norm = v.Norm();

            if( norm < lo )
            {
                return v.Normalized( lo );
            }
            if( norm > hi )
            {
                return v.Normalized( hi );
            }

            return v;
        }

        public static Vector3f ClampNorm( this Vector3f v, float lo, float hi )
        {
            var norm = v.Norm();

            if( norm < lo )
            {
                return v.Normalized( lo );
            }
            if( norm > hi )
            {
                return v.Normalized( hi );
            }

            return v;
        }

        public static float Saturate( this int x )
        {
            return x.Clamp( 0, 256 );
        }

        public static float Saturate( this float x )
        {
            return x.Clamp( 0.0f, 1.0f );
        }

        public static Vector4f Saturate( this Vector4f v )
        {
            return v.Clamp( 0.0f, 1.0f );
        }

        public static bool IsPowerOfTwo( this int x )
        {
            if( x < 1 )
            {
                return false;
            }
            else
            {
                return( ( x & ( x - 1 ) ) == 0 );

                // for unsigned int, the following takes care of 0 without branching
                // !(v & (v - 1)) && v;
            }
        }

        public static bool IsEven( this int x )
        {
            return( x % 2 == 0 );
        }

        public static bool IsOdd( this int x )
        {
            return( x % 2 == 1 );
        }

        /// <summary>
        /// Rounds x up to the nearest power of two.
        /// </summary>
        /// <param name="x">Input integer</param>
        /// <returns>
        /// Given an integer x, returns the next highest power of two with the following rules:
        /// If x < 1, then returns 1
        /// If x is a power of 2, returns x
        /// Otherwise, returns the next power of 2 greater than x.
        /// </returns>
        public static int RoundUpToNearestPowerOfTwo( this int x )
        {
            if( x < 1 )
            {
                return 1;
            }

            double log2x = Math.Log( x, 2 );
            double nextLog2 = Math.Ceiling( log2x );
            return ( int )( Math.Pow( 2, nextLog2 ) );
        }

        /// <summary>
        /// Given an (not necessarily power-of-two) integer x
        /// Returns the smallest integer y such that 2^y > x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int Log2( int x )
        {
            float yFloat = MathUtils.Log( x, 2 );
            return yFloat.CeilToInt();
        }

        // TODO: simpler, but if is power of 2, still gives the next one
        /*
        public static uint nlpo2( uint x )
        {
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                return(x+1);
        }
        */

        // Rounding

        /// <summary>
        /// Returns the fractional part of x
        /// 0 <= Fraction( x ) < 1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float FractionalPart( this float x )
        {
            return x - x.FloorToInt();
        }        

        /// <summary>
        /// Returns the fractional part of x
        /// 0 <= Fraction( x ) < 1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Vector2f FractionalPart( this Vector2f x )
        {
            return x - x.FloorToInt();
        }

        /// <summary>
        /// Returns the fractional part of x
        /// 0 <= Fraction( x ) < 1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Vector3f FractionalPart( this Vector3f x )
        {
            return x - x.FloorToInt();
        }

        /// <summary>
        /// Returns the fractional part of x
        /// 0 <= Fraction( x ) < 1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Vector4f FractionalPart( this Vector4f x )
        {
            return x - x.FloorToInt();
        }

        public static int FloorToInt( this float x )
        {
            return( ( int )x );
        }

        public static Vector2i FloorToInt( this Vector2f v )
        {
            return new Vector2i( v.x.FloorToInt(), v.y.FloorToInt() );
        }

        public static Vector3i FloorToInt( this Vector3f v )
        {
            return new Vector3i( v.x.FloorToInt(), v.y.FloorToInt(), v.z.FloorToInt() );
        }

        public static Vector4i FloorToInt( this Vector4f v )
        {
            return new Vector4i( v.x.FloorToInt(), v.y.FloorToInt(), v.z.FloorToInt(), v.w.FloorToInt() );
        }

        public static int RoundToInt( this float x )
        {
            return( ( int )( x + 0.5f ) );
        }

        public static int RoundToInt( this double x )
        {
            return( ( int )( x + 0.5f ) );
        }

        public static Vector2i RoundToInt( this Vector2f v )
        {
            return new Vector2i( v.x.RoundToInt(), v.y.RoundToInt() );
        }

        public static Vector3i RoundToInt( this Vector3f v )
        {
            return new Vector3i( v.x.RoundToInt(), v.y.RoundToInt(), v.z.RoundToInt() );
        }

        public static Vector4i RoundToInt( this Vector4f v )
        {
            return new Vector4i( v.x.RoundToInt(), v.y.RoundToInt(), v.z.RoundToInt(), v.w.RoundToInt() );
        }

        public static int CeilToInt( this float x )
        {
            return ( int )( Math.Ceiling( x ) );
        }

        public static Vector2i CeilToInt( this Vector2f v )
        {
            return new Vector2i( v.x.CeilToInt(), v.y.CeilToInt() );
        }

        /// <summary>
        /// Returns the modulus a mod n, *not* the remainder.
        /// if a is negative, a mod n is still positive.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Mod( int a, int n )
        {
            int r = a % n;
            if( r < 0 )
            {
                r += n;
            }
            return r;
        }

        /// <summary>
        /// Rounded integer lerp, x and y are *inclusive*
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int Lerp( int x, int y, float t )
        {
            float fx = x;
            float fy = y;
            float fz = Lerp( fx, fy, t );
            int rz = fz.RoundToInt();

            int min = Math.Min( x, y + 1 );
            int max = Math.Max( x, y + 1 );

            return rz.Clamp( min, max );
        }

        /// <summary>
        /// Catmull-Rom Cubic Interpolation
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float CubicInterpolate( float p0, float p1, float p2, float p3, float t )
        {
            // geometric construction:
	        //            t
	        //   (t+1)/2     t/2
	        // t+1        t	        t-1

	        // bottom level
	        var p0p1 = Arithmetic.Lerp( p0, p1, t + 1 );
	        var p1p2 = Arithmetic.Lerp( p1, p2, t );
	        var p2p3 = Arithmetic.Lerp( p2, p3, t - 1 );

	        // middle level
	        var p0p1_p1p2 = Arithmetic.Lerp( p0p1, p1p2, 0.5f * ( t + 1 ) );
	        var p1p2_p2p3 = Arithmetic.Lerp( p1p2, p2p3, 0.5f * t );

	        // top level
	        return Arithmetic.Lerp( p0p1_p1p2, p1p2_p2p3, t );
        }

        /// <summary>
        /// Standard floating point linear interpolation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Lerp( float x, float y, float t )
        {
            return( x + t * ( y - x ) );
        }
        
        /// <summary>
        /// Returns the fraction of the way i is between 0 and count
        /// </summary>
        /// <param name="i"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static float Fraction( int i, int count )
        {           
            return ( ( float )i ) / ( count - 1 );
        }

        /// <summary>
        /// Returns the fraction of the way i is between i0 and i0 + count
        /// for instance, if i0 = 0 and count = 10
        /// i = 0 --> 0.0f
        /// i = 9 --> 1.0f
        /// </summary>
        /// <param name="i"></param>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static float Fraction( int i, int i0, int count )
        {
            float diff = i - i0;
            return diff / ( count - 1 );
        }

        /// <summary>
        /// Given a < x < b, returns the fraction of the way x is between a and b
        /// Equivalent to ( x - a ) / ( b - a )
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Fraction( float x, float a, float b )
        {
            return ( x - a ) / ( b - a );
        }

        // nonlinearly interpolate between x and y using a cosine curve
        public static float CosineInterpolate( float x, float y, float t )
        {
            // cos( pi * t ) goes between 1 and 0
            // rescale to [0,1]
            // mirror the function left and right to make it the function x at t = 0 and y at t = 1

            float cosValue = ( float )( 0.5 * ( 1 + Math.Cos( Math.PI * t ) ) );
            float t2 = 1 - cosValue;
            return Lerp( x, y, t2 );
        }

        /// <summary>
        /// Nonlinearly interpolate between x and y using an t^2 curve
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="t"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public static float SquareInterpolate( float x, float y, float t )
        {
            return Lerp( x, y, t * t );
        }

        /// <summary>
        /// nonlinearly interpolate between x and y using a power law curve t^gamma
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="t"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public static float PowerLawInterpolate( float x, float y, float t, float gamma )
        {
            float t2 = ( float ) ( Math.Pow( t, gamma ) );
            return Lerp( x, y, t2 );
        }

        /// <summary>
        /// Returns 0 if x < min, 1 if x > max
        /// And a number between 0 and 1 if x \in [min,max]
        /// TODO: this behavior mimicks HLSL
        /// It doesn't interpolate between min and max, it's more like the inverse
        /// Write a SmoothStepInterpolate function?
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float SmoothStep( float min, float max, float x )
        {
            x = ( ( x - min ) / ( max - min ) ).Saturate();
            return x * x * ( 3 - 2 * x );
        }

        /// <summary>
        /// Evaluates the simple logistic curve 1 / ( 1 + e^-t )
        /// This function is extremely close to 0 when t < -6
        /// and extremely close to 1 when t > 6
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float LogisticSigmoid( float t )
        {
            return 1.0f / ( 1 + MathUtils.Exp( -t ) );
        }

        public static float DivideIntsToFloat( int x, int y )
        {
            return ( ( float )x ) / y;
        }

        public static double DivideLongsToFloat( long x, long y )
        {
            return ( ( float )x ) / y;
        }

        public static double DivideLongsToDouble( long x, long y )
        {
            return ( ( double )x ) / y;
        }

        public static float DegreesToRadians( float degrees )
        {
            return( ( float )( degrees * Math.PI / 180 ) );
        }

        public static float RadiansToDegrees( float radians )
        {
            return( ( float )( 180 * radians / Math.PI ) );
        }

        /// <summary>
        /// Decrements i by 1 and correctly mods it by n to wrap around
        /// The range of this function is [0,n-1]
        /// 
        /// DecrementAndMod( 4, 5 ) = 3
        /// DecrementAndMod( 0, 5 ) = 4
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int DecrementAndMod( int i, int n )
        {
            return ( i - 1 + n ) % n;
        }

        /// <summary>
        /// Increments i by 1 and mods it by n to wrap around
        /// The range of this function is [0,n-1]
        /// </summary>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int IncrementAndMod( int i, int n )
        {
            return ( i + 1 ) % n;
        }

        public static int BinSize( int totalSize, int nBins )
        {
            int binSize = totalSize / nBins;
            if( binSize * nBins < totalSize )
            {
                ++binSize;
            }
            return binSize;
        }

        public static int BinSizeWithOverlap( int totalSize, int nBins, int overlap )
        {
            int padding = ( nBins - 1 ) * overlap;
            return BinSize( totalSize + padding, nBins );
        }

        public static int NumBins( int totalSize, int binSize )
        {
            return totalSize / binSize;
        }

        public static float Square( float x )
        {
            return x * x;
        }

        public static double Square( double x )
        {
            return x * x;
        }
    }
}

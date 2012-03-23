using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Vector2f : IEquatable< Vector2f >
    {
        // HACK:
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        // HACK
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public float x;
        public float y;

        public static Vector2f Zero
        {
            get
            {
                return new Vector2f( 0, 0 );
            }
        }

        public static Vector2f Up
        {
            get
            {
                return new Vector2f( 0, 1 );
            }
        }

        public static Vector2f Right
        {
            get
            {
                return new Vector2f( 1, 0 );
            }
        }

        public static Vector2f UpRight
        {
            get
            {
                return new Vector2f( 1, 1 );
            }
        }

        public Vector2f( float[] v )
        {
            x = v[ 0 ];
            y = v[ 1 ];
        }

        public Vector2f( float x, float y )
        {
            this.x = x;
            this.y = y;
        }

        public Vector2f( Vector2f v )
            : this( v.x, v.y )
        {
            
        }

        public Vector2f( Vector2i v )
            : this( v.x, v.y )
        {
            
        }

        /// <summary>
        /// Component-wise absolute value
        /// </summary>
        /// <returns></returns>
        public Vector2f Abs()
        {
            return new Vector2f( Math.Abs( x ), Math.Abs( y ) );
        }

        public float Norm()
        {
            return ( float )( Math.Sqrt( x * x + y * y ) );

        }

        public float NormSquared()
        {
            return ( x * x + y * y );
        }

        public void Normalize()
        {
            float length = Norm();

            x = x / length;
            y = y / length;
        }

        /// <summary>
        /// Normalize this vector to point in the same direction but have length length
        /// </summary>
        /// <param name="length"></param>
        public void Normalize( float length )
        {
            float s = length / Norm();
            
            x = s * x;
            y = s * y;
        }

        public Vector2f Normalized()
        {
            float length = Norm();

            return new Vector2f
            (
                x / length,
                y / length
            );
        }

        public Vector2f Normalized( float length )
        {
            float s = length / Norm();

            return new Vector2f
            (
                s * x,
                s * y
            );
        }

        /// <summary>
        /// Returns a Vector orthogonal to this.
        /// Its norm depends on the vector, but is zero only for the zero vector.
        /// Note that the function this function is not continuous.
        /// </summary>
        /// <returns></returns>
        public Vector2f OrthogonalVector()
        {
            return new Vector2f( -y, x );
        }

        /// <summary>
        /// Returns x / y
        /// </summary>
        public float AspectRatio
        {
            get
            {
                return x / y;
            }
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
        
        public override bool Equals( object o )
        {
            if( o is Vector2f )
            {                
                var rhs = ( Vector2f )o;
                return( Equals( rhs ) );
            }
            return false;
        }

        public bool Equals( Vector2f other )
        {
            return ( x == other.x && y == other.y );
        }

        public override String ToString()
        {
            return string.Format( "< {0}, {1} >", x, y );
        }        

        // explicit conversion to Vector2i
        public static explicit operator Vector2i( Vector2f v )
        {
            return new Vector2i( ( int )( v.x ), ( int )( v.y ) );
        }

        public static Vector2f operator + ( Vector2f lhs, Vector2f rhs )
        {
            return new Vector2f( lhs.x + rhs.x, lhs.y + rhs.y );
        }

        public static Vector2f operator + ( Vector2f lhs, Vector2i rhs )
        {
            return new Vector2f( lhs.x + rhs.x, lhs.y + rhs.y );
        }

        public static Vector2f operator + ( Vector2i lhs, Vector2f rhs )
        {
            return new Vector2f( lhs.x + rhs.x, lhs.y + rhs.y );
        }

        public static Vector2f operator - ( Vector2f v )
        {
            return new Vector2f( -v.x, -v.y );
        }

        public static Vector2f operator - ( Vector2f lhs, Vector2f rhs )
        {
            return new Vector2f( lhs.x - rhs.x, lhs.y - rhs.y );
        }

        public static Vector2f operator - ( Vector2f lhs, Vector2i rhs )
        {
            return new Vector2f( lhs.x - rhs.x, lhs.y - rhs.y );
        }

        public static Vector2f operator - ( Vector2i lhs, Vector2f rhs )
        {
            return new Vector2f( lhs.x - rhs.x, lhs.y - rhs.y );
        }

        public static Vector2f operator * ( float a, Vector2f rhs )
        {
            return new Vector2f( a * rhs.x, a * rhs.y );
        }

        public static Vector2f operator * ( Vector2f lhs, float a )
        {
            return( a * lhs );
        }

        /// <summary>
        /// Per-element multiplication
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector2f operator * ( Vector2f lhs, Vector2f rhs )
        {
            return new Vector2f( lhs.x * rhs.x, lhs.y * rhs.y );
        }

        public static Vector2f operator / ( Vector2f v, float f )
        {
            float a = 1 / f;
            return new Vector2f( a * v.x, a * v.y );
        }

        public static Vector2f operator / ( Vector2f lhs, Vector2f rhs )
        {
            return new Vector2f( lhs.x / rhs.x, lhs.y / rhs.y );
        }

        public static Vector2f operator / ( Vector2f lhs, Vector2i rhs )
        {
            return new Vector2f( lhs.x / rhs.x, lhs.y / rhs.y );
        }

        public static Vector2f operator / ( Vector2i lhs, Vector2f rhs )
        {
            return new Vector2f( lhs.x / rhs.x, lhs.y / rhs.y );
        }

        public static bool operator == ( Vector2f lhs, Vector2f rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator != ( Vector2f lhs, Vector2f rhs )
        {
            return !( lhs.Equals( rhs ) );
        }

        /// <summary>
        /// Returns a Vector4 where each component contains
        /// the sign of each corresponding component of v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2f Sign( Vector2f v )
        {
            return new Vector2f( Math.Sign( v.x ), Math.Sign( v.y ) );
        }

        public static float Dot( Vector2f lhs, Vector2f rhs )
        {
            return( lhs.x * rhs.x + lhs.y * rhs.y );
        }

        public static Vector3f Cross( Vector2f lhs, Vector2f rhs )
        {
            return new Vector3f( 0, 0, lhs.x * rhs.y - lhs.y * rhs.x );
        }
        
        public static Vector2f Lerp( Vector2f x, Vector2f y, float t )
        {
            return( x + t * ( y - x ) );
        }

        /// <summary>
        /// Catmull-Rom Cubic Interpolation
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2f CubicInterpolate( Vector2f v0, Vector2f v1, Vector2f v2, Vector2f v3, float t )
        {
            // geometric construction:
	        //            t
	        //   (t+1)/2     t/2
	        // t+1        t	        t-1

	        // bottom level
	        var v0v1 = Vector2f.Lerp( v0, v1, t + 1 );
	        var v1v2 = Vector2f.Lerp( v1, v2, t );
	        var v2v3 = Vector2f.Lerp( v2, v3, t - 1 );

	        // middle level
	        var v0v1_v1v2 = Vector2f.Lerp( v0v1, v1v2, 0.5f * ( t + 1 ) );
	        var v1v2_v2v3 = Vector2f.Lerp( v1v2, v2v3, 0.5f * t );

	        // top level
	        return Vector2f.Lerp( v0v1_v1v2, v1v2_v2v3, t );
        }

        /// <summary>
        /// Returns the projection of u onto v,
        /// Defined as the length of the component of u in the direction of v
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float ProjectionLength( Vector2f u, Vector2f v )
        {
            return Vector2f.Dot( u, v.Normalized() );
        }

        /// <summary>
        /// Returns the projection of u onto v,
        /// Defined as the vector in the direction of v
        /// with length equal to the component of u in the direction of v
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2f Projection( Vector2f u, Vector2f v )
        {
            // let w = v / |v|
            // want ( u dot w ) * w
            // = [ u dot ( v / |v| ) ] ( v / |v| )
            // = v * ( u dot v ) / |v|^2

            var v2 = v.NormSquared();
            var c = Vector2f.Dot( u, v ) / v2;
            return c * v;
        }

        /// <summary>
        /// Given *unit* vectors u and v, returns the angle between them.
        /// The range is between [-pi, pi]
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float AngleBetween( Vector2f u, Vector2f v )
        {
            float cosTheta = Vector2f.Dot( u, v );
            float sinTheta = Vector2f.Cross( u, v ).Norm();

            return MathUtils.Atan2( sinTheta, cosTheta );
        }

        public static float BoxDistance( Vector2f lhs, Vector2f rhs )
        {
            return Math.Max( Math.Abs( lhs.x - rhs.x ), Math.Abs( lhs.y - rhs.y ) );
        }

        public static float L1Distance( Vector2f lhs, Vector2f rhs )
        {
            return Math.Abs( lhs.x - rhs.x ) + Math.Abs( lhs.y - rhs.y );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    // TODO: make it immutable?
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Vector3f : IEquatable< Vector3f >
    {
        public float x;
        public float y;
        public float z;

        public static Vector3f Zero
        {
            get
            {
                return new Vector3f( 0, 0, 0 );
            }
        }

        public static Vector3f Up
        {
            get
            {
                return new Vector3f( 0, 1, 0 );
            }
        }

        public static Vector3f Right
        {
            get
            {
                return new Vector3f( 1, 0, 0 );
            }
        }

        public static Vector3f Forward
        {
            get
            {
                return new Vector3f( 0, 0, -1 );
            }
        }

        public Vector3f( float x, float y, float z )
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3f( float[] v )
            : this( v[ 0 ], v[ 1 ], v[ 2 ] )
        {

        }        

        public Vector3f( Vector2f xy, float z )
            : this( xy.x, xy.y, z )
        {

        }

        public Vector3f( Vector3f v )
            : this( v.x, v.y, v.z )
        {

        }

        public Vector2f XY
        {
            get
            {
                return new Vector2f( x, y );
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        public float this[ int k ]
        {
            get
            {
                switch( k )
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 2 for a 3-vector." );
                }
            }
            set
            {
                switch( k )
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 2 for a 3-vector." );
                }
            }
        }

        public float Norm()
        {
            return ( float )( Math.Sqrt( x * x + y * y + z * z ) );

        }

        public float NormSquared()
        {
            return ( x * x + y * y + z * z );
        }

        public void Normalize()
        {
            float length = Norm();

            x = x / length;
            y = y / length;
            z = z / length;
        }

        public Vector3f Normalized()
        {
            float length = Norm();

            return new Vector3f
            (
                x / length,
                y / length,
                z / length
            );
        }

        public Vector3f Normalized( float length )
        {
            float s = length / Norm();

            return new Vector3f
            (
                s * x,
                s * y,
                s * z
            );
        }

        public Vector2f Homogenized()
        {
            return new Vector2f( x / z, y / z );
        }        

        /// <summary>
        /// TODO: check that this function is robust
        /// Returns a vector orthogonal to this.
        /// Its norm depends on the vector, but is zero only for the zero vector.
        /// Note that the function this function is not continuous.
        /// </summary>
        /// <returns></returns>
        public Vector3f OrthogonalVector()
        {
            // Find smallest component. Keep equal case for null values.
            if( ( Math.Abs( y ) >= 0.9f * Math.Abs( x ) ) && ( Math.Abs( z ) >= 0.9f * Math.Abs( x ) ) )
            {
                return new Vector3f( 0.0f, -z, y ); 
            }
            else if( ( Math.Abs( x ) >= 0.9f * Math.Abs( y ) ) && ( Math.Abs( z ) >= 0.9f * Math.Abs( y ) ) )
            {
                return new Vector3f( -z, 0.0f, x );
            }
            else
            {
                return new Vector3f( -y, x, 0.0f );
            }
        }

        public float Min()
        {
            return Math.Min( x, Math.Min( y, z ) );
        }

        public float Max()
        {
            return Math.Max( x, Math.Max( y, z ) );
        }
        
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            if( obj is Vector3f )
            {
                return ( Equals( ( Vector3f ) obj ) );
            }
            return false;
        }

        public bool Equals( Vector3f other )
        {
            return ( x == other.x && y == other.y && z == other.z );
        }        

        public override string ToString()
        {
            return string.Format( "< {0}, {1}, {2} >", x, y, z );
        }

        public static Vector3f Parse( string s )
        {
            var tokens = s.Split( ' ' );
            
            float x = float.Parse( tokens[ 0 ] );
            float y = float.Parse( tokens[ 1 ] );
            float z = float.Parse( tokens[ 2 ] );
            
            return new Vector3f( x, y, z );
        }

        public static Vector3f operator * ( float a, Vector3f rhs )
        {
            return new Vector3f( a * rhs.x, a * rhs.y, a * rhs.z );
        }

        public static Vector3f operator * ( Vector3f lhs, float a )
        {
            return( a * lhs );
        }

        /// <summary>
        /// Per-element multiplication
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector3f operator * ( Vector3f lhs, Vector3f rhs )
        {
            return new Vector3f( lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z );
        }

        public static Vector3f operator + ( Vector3f lhs, Vector3f rhs )
        {
            return new Vector3f( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z );
        }

        public static Vector3f operator + ( Vector3f lhs, Vector3i rhs )
        {
            return new Vector3f( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z );
        }

        public static Vector3f operator + ( Vector3i lhs, Vector3f rhs )
        {
            return new Vector3f( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z );
        }

        public static Vector3f operator - ( Vector3f lhs, Vector3f rhs )
        {
            return new Vector3f( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z );
        }

        public static Vector3f operator - ( Vector3f lhs, Vector3i rhs )
        {
            return new Vector3f( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z );
        }

        public static Vector3f operator - ( Vector3i lhs, Vector3f rhs )
        {
            return new Vector3f( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z );
        }

        public static Vector3f operator - ( Vector3f v )
        {
            return new Vector3f( -v.x, -v.y, -v.z );
        }

        public static Vector3f operator / ( Vector3f v, float f )
        {
            float a = 1 / f;
            return new Vector3f( a * v.x, a * v.y, a * v.z );
        }

        public static Vector3f operator / ( Vector3f lhs, Vector3f rhs )
        {
            return new Vector3f( lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z );
        }

        public static float Dot( Vector3f lhs, Vector3f rhs )
        {
            return( lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z );
        }

        // TODO: rest of the classes needs == and !=
        public static bool operator == ( Vector3f lhs, Vector3f rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator != ( Vector3f lhs, Vector3f rhs )
        {
            return !( lhs.Equals( rhs ) );
        }

        /// <summary>
        /// Returns a Vector4 where each component contains
        /// the sign of each corresponding component of v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3f Sign( Vector3f v )
        {
            return new Vector3f( Math.Sign( v.x ), Math.Sign( v.y ), Math.Sign( v.z ) );
        }

        public static Vector3f Cross( Vector3f lhs, Vector3f rhs )
        {
            return new Vector3f
            (
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x
            );
        }

        public static Vector3f Lerp( Vector3f x, Vector3f y, float t )
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
        public static Vector3f CubicInterpolate( Vector3f v0, Vector3f v1, Vector3f v2, Vector3f v3, float t )
        {
            // geometric construction:
	        //            t
	        //   (t+1)/2     t/2
	        // t+1        t	        t-1

	        // bottom level
	        var v0v1 = Vector3f.Lerp( v0, v1, t + 1 );
	        var v1v2 = Vector3f.Lerp( v1, v2, t );
	        var v2v3 = Vector3f.Lerp( v2, v3, t - 1 );

	        // middle level
	        var v0v1_v1v2 = Vector3f.Lerp( v0v1, v1v2, 0.5f * ( t + 1 ) );
	        var v1v2_v2v3 = Vector3f.Lerp( v1v2, v2v3, 0.5f * t );

	        // top level
	        return Vector3f.Lerp( v0v1_v1v2, v1v2_v2v3, t );
        }

        /// <summary>
        /// Returns the projection of u onto v,
        /// Defined as the length of the component of u in the direction of v
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float ProjectionLength( Vector3f u, Vector3f v )
        {
            return Vector3f.Dot( u, v.Normalized() );
        }

        /// <summary>
        /// Returns the projection of u onto v,
        /// Defined as the vector in the direction of v
        /// with length equal to the component of u in the direction of v
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3f Projection( Vector3f u, Vector3f v )
        {
            // let w = v / |v|
            // want ( u dot w ) * w
            // = [ u dot ( v / |v| ) ] ( v / |v| )
            // = v * ( u dot v ) / |v|^2

            var v2 = v.NormSquared();
            var c = Vector3f.Dot( u, v ) / v2;
            return c * v;
        }

        /// <summary>
        /// Given *unit* vectors u and v, returns the angle between them.
        /// The range is between [-pi, pi]
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float AngleBetween( Vector3f u, Vector3f v )
        {
            float cosTheta = Vector3f.Dot( u, v );
            float sinTheta = Vector3f.Cross( u, v ).Norm();

            return MathUtils.Atan2( sinTheta, cosTheta );
        }

        public static float BoxDistance( Vector3f lhs, Vector3f rhs )
        {
            return Math.Max( Math.Max( Math.Abs( lhs.x - rhs.x ), Math.Abs( lhs.y - rhs.y ) ), Math.Abs( lhs.z - rhs.z ) );
        }

        public static float L1Distance( Vector3f lhs, Vector3f rhs )
        {
            return Math.Abs( lhs.x - rhs.x ) + Math.Abs( lhs.y - rhs.y ) + Math.Abs( lhs.z - rhs.z );
        }
    }
}

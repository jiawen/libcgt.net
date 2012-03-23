using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using libcgt;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Vector2i : IEquatable< Vector2i >
    {
        public int x;
        public int y;

        public static Vector2i Zero
        {
            get
            {
                return new Vector2i( 0, 0 );
            }
        }

        public static Vector2i Up
        {
            get
            {
                return new Vector2i( 0, 1 );
            }
        }

        public static Vector2i Right
        {
            get
            {
                return new Vector2i( 1, 0 );
            }
        }

        public Vector2i( params int[] v )
        {
            x = v[ 0 ];
            y = v[ 1 ];
        }

        public Vector2i( Vector2i v )
        {
            this.x = v.x;
            this.y = v.y;
        }

        public float Norm()
        {
            return ( float )( Math.Sqrt( NormSquared() ) );
        }

        public int NormSquared()
        {
            return( x * x + y * y );
        }
        
        public Vector2f Normalized()
        {
            float length = ( float )( Norm() );

            return new Vector2f
            (
                x / length,
                y / length
            );
        }

        /// <summary>
        /// Returns x / y as a float
        /// </summary>
        public float AspectRatio
        {
            get
            {
                return Arithmetic.DivideIntsToFloat( x, y );
            }
        }

        /// <summary>
        /// Returns a Vector orthogonal to this.
        /// Its norm depends on the vector, but is zero only for the zero vector.
        /// Note that the function this function is not continuous.
        /// </summary>
        /// <returns></returns>
        public Vector2i OrthogonalVector()
        {
            return new Vector2i( -y, x );
        }        

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
        
        public override bool Equals( object o )
        {
            if( o is Vector2i )
            {                
                Vector2i rhs = ( Vector2i )o;
                return( Equals( rhs ) );
            }
            return false;
        }

        public bool Equals( Vector2i other )
        {
            return ( x == other.x && y == other.y );
        }

        public override string ToString()
        {
            return string.Format( "< {0}, {1} >", x, y );
        }

        public static Vector2i Parse( string s )
        {
            var tokens = s.Split( ' ' );
            
            int x = int.Parse( tokens[ 0 ] );
            int y = int.Parse( tokens[ 1 ] );
            
            return new Vector2i( x, y );
        }

        public static Vector2f operator * ( float a, Vector2i rhs )
        {
            return new Vector2f( a * rhs.x, a * rhs.y );
        }

        public static Vector2f operator * ( Vector2i rhs, float a )
        {
            return rhs * a;
        }

        public static Vector2i operator * ( int a, Vector2i rhs )
        {
            return new Vector2i( a * rhs.x, a * rhs.y );
        }

        public static Vector2i operator * ( Vector2i lhs, int a )
        {
            return( a * lhs );
        }

        /// <summary>
        /// Per-element multiplication
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector2i operator * ( Vector2i lhs, Vector2i rhs )
        {
            return new Vector2i( lhs.x * rhs.x, lhs.y * rhs.y );
        }

        public static Vector2i operator + ( Vector2i lhs, Vector2i rhs )
        {
            return new Vector2i( lhs.x + rhs.x, lhs.y + rhs.y );
        }

        public static Vector2i operator - ( Vector2i lhs, Vector2i rhs )
        {
            return new Vector2i( lhs.x - rhs.x, lhs.y - rhs.y );
        }

        public static Vector2i operator - ( Vector2i v )
        {
            return new Vector2i( -v.x, -v.y );
        }

        public static Vector2f operator / ( Vector2i v, float a )
        {
            return new Vector2f( v.x / a, v.y / a );
        }

        public static Vector2i operator / ( Vector2i v, int a )
        {
            return new Vector2i( v.x / a, v.y / a );
        }

        public static Vector2i operator / ( Vector2i lhs, Vector2i rhs )
        {
            return new Vector2i( lhs.x / rhs.x, lhs.y / rhs.y );
        }

        public static bool operator == ( Vector2i lhs, Vector2i rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator != ( Vector2i lhs, Vector2i rhs )
        {
            return !( lhs.Equals( rhs ) );
        }

        public static int Dot( Vector2i lhs, Vector2i rhs )
        {
            return( lhs.x * rhs.x + lhs.y * rhs.y );
        }

        public static Vector3i Cross( Vector2i lhs, Vector2i rhs )
        {
            return new Vector3i( 0, 0, lhs.x * rhs.y - lhs.y * rhs.x );
        }

        // implicit conversion to float
        public static implicit operator Vector2f( Vector2i v )
        {
            return new Vector2f( v.x, v.y);
        }
    }
}

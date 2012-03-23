using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Vector3i : IEquatable< Vector3i >
    {
        public int x;
        public int y;
        public int z;

        public static Vector3i Zero
        {
            get
            {
                return new Vector3i( 0, 0, 0 );
            }
        }

        public static Vector3i Up
        {
            get
            {
                return new Vector3i( 0, 1, 0 );
            }
        }

        public static Vector3i Right
        {
            get
            {
                return new Vector3i( 1, 0, 0 );
            }
        }

        public Vector3i( params int[] v )
        {
            x = v[ 0 ];
            y = v[ 1 ];
            z = v[ 2 ];
        }

        public Vector3i( Vector2i xy, int z )
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = z;
        }

        public Vector3i( Vector3i v )
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public Vector2i XY
        {
            get
            {
                return new Vector2i( x, y );
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        public int this[ int k ]
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
            return ( float )( Math.Sqrt( NormSquared() ) );
        }

        public int NormSquared()
        {
            return( x * x + y * y + z * z );
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

        // TODO
        /*
        public Vector3i OrthogonalVector()
        {
            // return new Vector3i( -y, x );
        } 
        */

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        
        public override bool Equals( object o )
        {
            if( o is Vector3i )
            {                
                Vector3i rhs = ( Vector3i )o;
                return( Equals( rhs ) );
            }
            return false;
        }

        public bool Equals( Vector3i other )
        {
            return ( x == other.x && y == other.y && z == other.z );
        }

        public override string ToString()
        {
            return string.Format( "< {0}, {1}, {2} >", x, y, z );
        }
        
        public static Vector3i Parse( string s )
        {
            var tokens = s.Split( ' ' );
            
            int x = int.Parse( tokens[ 0 ] );
            int y = int.Parse( tokens[ 1 ] );
            int z = int.Parse( tokens[ 2 ] );
            
            return new Vector3i( x, y, z );
        }

        public static Vector3i operator * ( int a, Vector3i rhs )
        {
            return new Vector3i( a * rhs.x, a * rhs.y );
        }

        public static Vector3i operator * ( Vector3i lhs, int a )
        {
            return( a * lhs );
        }

        public static Vector3i operator + ( Vector3i lhs, Vector3i rhs )
        {
            return new Vector3i( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z );
        }

        public static Vector3i operator - ( Vector3i lhs, Vector3i rhs )
        {
            return new Vector3i( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z );
        }

        public static Vector3i operator - ( Vector3i v )
        {
            return new Vector3i( -v.x, -v.y, -v.z );
        }

        public static Vector3i operator / ( Vector3i v, int a )
        {
            return new Vector3i( v.x / a, v.y / a, v.z / a );
        }

        public static bool operator == ( Vector3i lhs, Vector3i rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator != ( Vector3i lhs, Vector3i rhs )
        {
            return !( lhs.Equals( rhs ) );
        }

        public static int Dot( Vector3i lhs, Vector3i rhs )
        {
            return( lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z );
        }

        public static Vector3i Cross( Vector3i lhs, Vector3i rhs )
        {
            return new Vector3i
            (
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x
            );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Vector4i : IEquatable< Vector4i >
    {
        public int x;
        public int y;
        public int z;
        public int w;

        public static Vector4i Zero
        {
            get
            {
                return new Vector4i( 0, 0, 0, 0 );
            }
        }

        public static Vector4i ZeroH
        {
            get
            {
                return new Vector4i( 0, 0, 0, 1 );
            }
        }

        public static Vector4i UpH
        {
            get
            {
                return new Vector4i( 0, 1, 0, 1 );
            }
        }

        public static Vector4i RightH
        {
            get
            {
                return new Vector4i( 1, 0, 0, 1 );
            }
        }

        public static Vector4i ForwardH
        {
            get
            {
                return new Vector4i( 0, 0, -1, 1 );
            }
        }

        public static Vector4i Ones
        {
            get
            {
                return new Vector4i( 1, 1, 1, 1 );
            }
        }

        public Vector4i( int[] v )
        {
            x = v[ 0 ];
            y = v[ 1 ];
            z = v[ 2 ];
            w = v[ 3 ];
        }

        public Vector4i( int x, int y, int z, int w )
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4i( Vector2i xy, int z, int w )
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = z;
            this.w = w;
        }

        public Vector4i( Vector2i xy, Vector2i zw )
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = zw.x;
            this.w = zw.y;
        }

        public Vector4i( int x, Vector3i yzw )
        {
            this.x = x;
            this.y = yzw.x;
            this.z = yzw.y;
            this.w = yzw.z;
        }

        public Vector4i( Vector3i xyz, int w )
        {
            this.x = xyz.x;
            this.y = xyz.y;
            this.z = xyz.z;
            this.w = w;
        }

        public Vector4i( Vector4i v )
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }

        public int X
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

        public int Y
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

        public int Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
            }
        }

        public int W
        {
            get
            {
                return w;
            }
            set
            {
                w = value;
            }
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

        public Vector2i ZW
        {
            get
            {
                return new Vector2i( z, w );
            }
            set
            {
                z = value.x;
                w = value.y;
            }
        }

        public Vector3i XYZ
        {
            get
            {
                return new Vector3i( x, y, z );
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        public Vector3i YZW
        {
            get
            {
                return new Vector3i( y, z, w );
            }
            set
            {
                y = value.x;
                z = value.y;
                w = value.z;
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
                    case 3:
                        return w;
                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 3 for a 4-vector." );
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
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 3 for a 4-vector." );
                }
            }
        }

        public float Norm()
        {
            return( ( float )( Math.Sqrt( x * x + y * y + z * z + w * w ) ) );
        }

        public int NormSquared()
        {
            return( x * x + y * y + z * z + w * w );
        }        

        public Vector4f Normalized()
        {
            float length = Norm();

            return new Vector4f( x / length, y / length, y / length, w / length );
        }

        public Vector3i Homogenized()
        {
            return new Vector3i( x / w, y / w, z / w );
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override bool Equals( object o )
        {
            if( o is Vector4i )
            {
                Vector4i rhs = ( Vector4i )o;
                return ( Equals( rhs ) );
            }
            return false;
        }

        public bool Equals( Vector4i other )
        {
            return ( x == other.x && y == other.y && z == other.z && w == other.w );
        }

        public override String ToString()
        {
            return string.Format( "< {0}, {1}, {2}, {3} >", x, y, z, w );
        }        

        public static Vector4i operator * ( int a, Vector4i rhs )
        {
            return new Vector4i( a * rhs.x, a * rhs.y, a * rhs.z, a * rhs.w );
        }

        public static Vector4i operator * ( Vector4i lhs, int a )
        {
            return( a * lhs );
        }

        // TODO: homogeneous add?
        public static Vector4i operator + ( Vector4i lhs, Vector4i rhs )
        {
            return new Vector4i( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w );
        }

        // TODO: homogeneous subtract
        public static Vector4i operator - ( Vector4i lhs, Vector4i rhs )
        {
            return new Vector4i( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w );
        }

        // TODO: homogeneous negate
        public static Vector4i operator - ( Vector4i v )
        {
            return new Vector4i( -v.x, -v.y, -v.z, -v.w );
        }

        public static Vector4i operator / ( Vector4i lhs, int f )
        {
            return new Vector4i( lhs.x / f, lhs.y / f, lhs.z / f, lhs.w / f );
        }

        public static Vector4i operator / ( Vector4i lhs, Vector4i rhs )
        {
            return new Vector4i( lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w );
        }

        public static bool operator == ( Vector4i lhs, Vector4i rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator != ( Vector4i lhs, Vector4i rhs )
        {
            return !( lhs.Equals( rhs ) );
        }

        public static Vector4i Negate( Vector4i v )
        {
            return new Vector4i( -v.x, -v.y, -v.z, -v.w );
        }
        
        public static int Dot( Vector4i lhs, Vector4i rhs )
        {
            return( lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w );
        }
    }
}

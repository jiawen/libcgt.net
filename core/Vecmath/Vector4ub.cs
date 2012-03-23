using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using libcgt.core.ImageProcessing;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Vector4ub : IEquatable< Vector4ub >
    {
        public byte x;
        public byte y;
        public byte z;
        public byte w;

        public static Vector4ub Zero
        {
            get
            {
                return new Vector4ub( 0, 0, 0, 0 );
            }
        }

        public Vector4ub( byte[] v )
        {
            x = v[ 0 ];
            y = v[ 1 ];
            z = v[ 2 ];
            w = v[ 3 ];
        }

        public Vector4ub( byte x, byte y, byte z, byte w )
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /*
        public Vector4ub( Vector2f xy, byte z, byte w )
        {
            this.x = xy.X;
            this.y = xy.Y;
            this.z = z;
            this.w = w;
        }

        public Vector4ub( Vector3f xyz, byte w )
        {
            this.x = xyz.X;
            this.y = xyz.Y;
            this.z = xyz.Z;
            this.w = w;
        }
        */

        public Vector4ub( Vector4ub v )
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }

        /*
        public Vector3f XYZ
        {
            get
            {
                return new Vector3f( x, y, z );
            }
            set
            {
                x = value.X;
                y = value.Y;
                z = value.Z;
            }
        }

        public Vector3f YZW
        {
            get
            {
                return new Vector3f( y, z, w );
            }
            set
            {
                y = value.X;
                z = value.Y;
                w = value.Z;
            }
        }
        */

        public byte this[ int k ]
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

        public Vector3f Homogenized()
        {
            return new Vector3f( x / w, y / w, z / w );
        }        

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override bool Equals( object o )
        {
            if( o is Vector4ub )
            {
                var rhs = ( Vector4ub )o;
                return ( Equals( rhs ) );
            }
            return false;
        }

        public bool Equals( Vector4ub other )
        {
            return ( x == other.x && y == other.y && z == other.z && w == other.w );
        }

        public override String ToString()
        {
            return string.Format( "< {0}, {1}, {2}, {3} >", x, y, z, w );
        }        

        /*
        public static Vector4ub operator * ( byte a, Vector4ub rhs )
        {
            return new Vector4ub( a * rhs.x, a * rhs.y, a * rhs.z, a * rhs.w );
        }

        public static Vector4ub operator * ( Vector4ub lhs, byte a )
        {
            return( a * lhs );
        }

        // TODO: homogeneous add?
        public static Vector4ub operator + ( Vector4ub lhs, Vector4ub rhs )
        {
            return new Vector4ub( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w );
        }

        // TODO: homogeneous subtract
        public static Vector4ub operator - ( Vector4ub lhs, Vector4ub rhs )
        {
            return new Vector4ub( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w );
        }

        // TODO: homogeneous negate
        public static Vector4ub operator - ( Vector4ub v )
        {
            return new Vector4ub( -v.x, -v.y, -v.z, -v.w );
        }

        public static Vector4ub operator / ( Vector4ub lhs, Vector4ub rhs )
        {
            return new Vector4ub( lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w );
        }

        public static Vector4ub Negate( Vector4ub v )
        {
            return new Vector4ub( -v.x, -v.y, -v.z, -v.w );
        }
        
        public static Vector4ub Normalize( Vector4ub v )
        {
            byte length = v.Norm();
            return new Vector4ub( v.x / length, v.y / length, v.z / length, v.w / length );
        }

        public static byte Dot( Vector4ub lhs, Vector4ub rhs )
        {
            return( lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w );
        }
        */

        // TODO: including ImageProcessing is weird, is this the right way to go?
        public static Vector4ub Lerp( Vector4ub x, Vector4ub y, float t )
        {
            return Vector4f.Lerp( x.ToFloat(), y.ToFloat(), t ).ToUnsignedByte();
        }
    }
}

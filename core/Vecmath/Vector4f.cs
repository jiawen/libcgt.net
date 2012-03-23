using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Vector4f : IEquatable< Vector4f >
    {
        // TODO: user defined conversions: http://msdn.microsoft.com/en-us/library/aa288476(VS.71).aspx
        // explicit for float to int
        // implicit for int to float

        public float x;
        public float y;
        public float z;
        public float w;

        public static Vector4f Zero
        {
            get
            {
                return new Vector4f( 0, 0, 0, 0 );
            }
        }

        public static Vector4f ZeroH
        {
            get
            {
                return new Vector4f( 0, 0, 0, 1 );
            }
        }

        public static Vector4f UpH
        {
            get
            {
                return new Vector4f( 0, 1, 0, 1 );
            }
        }

        public static Vector4f RightH
        {
            get
            {
                return new Vector4f( 1, 0, 0, 1 );
            }
        }

        public static Vector4f ForwardH
        {
            get
            {
                return new Vector4f( 0, 0, -1, 1 );
            }
        }

        public Vector4f( float[] v )
        {
            x = v[ 0 ];
            y = v[ 1 ];
            z = v[ 2 ];
            w = v[ 3 ];
        }

        public Vector4f( float x, float y, float z, float w )
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4f( Vector2f xy, float z, float w )
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = z;
            this.w = w;
        }        

        public Vector4f( float x, float y, Vector2f zw )
        {
            this.x = x;
            this.y = y;
            this.z = zw.x;
            this.w = zw.y;
        }

        public Vector4f( Vector2f xy, Vector2f zw )
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = zw.x;
            this.w = zw.y;
        }

        public Vector4f( Vector3f xyz, float w )
        {
            this.x = xyz.x;
            this.y = xyz.y;
            this.z = xyz.z;
            this.w = w;
        }

        public Vector4f( Vector4f v )
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
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

        public Vector2f ZW
        {
            get
            {
                return new Vector2f( z, w );
            }
            set
            {
                z = value.x;
                w = value.y;
            }
        }

        public Vector3f XYZ
        {
            get
            {
                return new Vector3f( x, y, z );
            }
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
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
                y = value.x;
                z = value.y;
                w = value.z;
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

        public float NormSquared()
        {
            return( x * x + y * y + z * z + w * w );
        }        

        public void Normalize()
        {
            float length = Norm();
            float rLength = 1.0f / length;

            x = rLength * x;
            y = rLength * y;
            z = rLength * z;
            w = rLength * w;
        }

        public Vector4f Normalized()
        {
            float length = Norm();
            float rLength = 1.0f / length;

            return new Vector4f( rLength * x, rLength * y, rLength * z, rLength * w );
        }

        public Vector3f Homogenized()
        {
            float rw = 1.0f / w;
            return new Vector3f( rw * x, rw * y, rw * z );
        }

#if false
        /// <summary>
        /// TODO: check that this function is robust
        /// Returns a vector orthogonal to this.
        /// </summary>
        public void OrthogonalVector()
        {
            // find the two smallest components
            // say x, and y, then return -w, z, etc
            // nasty set of case statements
        }
#endif

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            if( obj is Vector4f )
            {
                return ( Equals( ( Vector4f) obj ) );
            }
            return false;
        }

        public bool Equals( Vector4f other )
        {
            return ( x == other.x && y == other.y && z == other.z && w == other.w );
        }

        public override string ToString()
        {
            return string.Format( "< {0}, {1}, {2}, {3} >", x, y, z, w );
        }

        // implicit conversion to Vector4f
        public static implicit operator Vector4f( Vector4i v )
        {
            return new Vector4f( v.x, v.y, v.z, v.w );
        }

        // explicit conversion to Vector4f
        public static explicit operator Vector4i( Vector4f v )
        {
            return new Vector4i( ( int )( v.x ), ( int )( v.y ), ( int )( v.z ), ( int )( v.w ) );
        }

        public static Vector4f operator * ( float a, Vector4f rhs )
        {
            return new Vector4f( a * rhs.x, a * rhs.y, a * rhs.z, a * rhs.w );
        }

        public static Vector4f operator * ( Vector4f lhs, float a )
        {
            return( a * lhs );
        }

        /// <summary>
        /// Per-element multiplication
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector4f operator * ( Vector4f lhs, Vector4f rhs )
        {
            return new Vector4f( lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w );
        }

        // TODO: homogeneous add?
        public static Vector4f operator + ( Vector4f lhs, Vector4f rhs )
        {
            return new Vector4f( lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w );
        }

        public static Vector4f operator + ( Vector4f lhs, Vector4i rhs )
        {
            return new Vector4f( lhs.x + rhs.X, lhs.y + rhs.Y, lhs.z + rhs.Z, lhs.w + rhs.W );
        }

        public static Vector4f operator + ( Vector4i lhs, Vector4f rhs )
        {
            return new Vector4f( lhs.X + rhs.x, lhs.Y + rhs.y, lhs.Z + rhs.z, lhs.W + rhs.w );
        }

        // TODO: homogeneous subtract
        public static Vector4f operator - ( Vector4f lhs, Vector4f rhs )
        {
            return new Vector4f( lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w );
        }

        public static Vector4f operator - ( Vector4f lhs, Vector4i rhs )
        {
            return new Vector4f( lhs.x - rhs.X, lhs.y - rhs.Y, lhs.z - rhs.Z, lhs.w - rhs.W );
        }

        public static Vector4f operator - ( Vector4i lhs, Vector4f rhs )
        {
            return new Vector4f( lhs.X - rhs.x, lhs.Y - rhs.y, lhs.Z - rhs.z, lhs.W - rhs.w );
        }

        // TODO: homogeneous negate
        public static Vector4f operator - ( Vector4f v )
        {
            return new Vector4f( -v.x, -v.y, -v.z, -v.w );
        }

        public static Vector4f operator / ( Vector4f lhs, float f )
        {
            return new Vector4f( lhs.x / f, lhs.y / f, lhs.z / f, lhs.w / f );
        }

        public static Vector4f operator / ( Vector4f lhs, Vector4f rhs )
        {
            return new Vector4f( lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w );
        }

        public static bool operator == ( Vector4f lhs, Vector4f rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator != ( Vector4f lhs, Vector4f rhs )
        {
            return !( lhs.Equals( rhs ) );
        }

        /// <summary>
        /// A 4D "cross product" of 3 4-vectors
        /// (Finds a vector normal to the given 3)
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector4f Cross( Vector4f v0, Vector4f v1, Vector4f v2 )
        {
            float x = MatrixUtil.Determinant3x3f( v0.y, v0.z, v0.w, v1.y, v1.z, v1.w, v2.y, v2.z, v2.w );
            float y = -MatrixUtil.Determinant3x3f( v0.x, v0.z, v0.w, v1.x, v1.z, v1.w, v2.x, v2.z, v2.w );
            float z = MatrixUtil.Determinant3x3f( v0.x, v0.y, v0.w, v1.x, v1.y, v1.w, v2.x, v2.y, v2.w );
            float w = -MatrixUtil.Determinant3x3f( v0.x, v0.y, v0.z, v1.x, v1.y, v1.z, v2.x, v2.y, v2.z );

            return new Vector4f( x, y, z, w );
        }

        public static Vector4f Negate( Vector4f v )
        {
            return new Vector4f( -v.x, -v.y, -v.z, -v.w );
        }

        /// <summary>
        /// Returns a Vector4 where each component contains
        /// the sign of each corresponding component of v
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector4f Sign( Vector4f v )
        {
            return new Vector4f( Math.Sign( v.x ), Math.Sign( v.y ), Math.Sign( v.z ), Math.Sign( v.w ) );
        }
        
        public static float Dot( Vector4f lhs, Vector4f rhs )
        {
            return( lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w );
        }

        public static Vector4f Lerp( Vector4f x, Vector4f y, float t )
        {
            return( x + t * ( y - x ) );
        }
    }
}

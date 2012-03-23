using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Quat4f : IEquatable< Quat4f >
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static Quat4f Zero
        {
            get
            {
                return new Quat4f( 0, 0, 0, 0 );
            }
        }

        public static Quat4f Identity
        {
            get
            {
                return new Quat4f( 0, 0, 0, 1 );
            }
        }

        public static Quat4f FromRotationMatrix( Matrix3f m )
        {
            float x;
            float y;
            float z;
            float w;

            // Compute one plus the trace of the matrix
            float onePlusTrace = 1.0f + m[ 0, 0 ] + m[ 1, 1 ] + m[ 2, 2 ];

            if( onePlusTrace > 1e-5 )
            {
                // Direct computation
                float s = MathUtils.Sqrt( onePlusTrace ) * 2.0f;
                x = ( m[ 2, 1 ] - m[ 1, 2 ] ) / s;
                y = ( m[ 0, 2 ] - m[ 2, 0 ] ) / s;
                z = ( m[ 1, 0 ] - m[ 0, 1 ] ) / s;
                w = 0.25f * s;
            }
            else
            {
                // Computation depends on major diagonal term
                if( ( m[ 0, 0 ] > m[ 1, 1 ] ) & ( m[ 0, 0 ] > m[ 2, 2 ] ) )
                {
                    float s = MathUtils.Sqrt( 1.0f + m[ 0, 0 ] - m[ 1, 1 ] - m[ 2, 2 ] ) * 2.0f;
                    x = 0.25f * s;
                    y = ( m[ 0, 1 ] + m[ 1, 0 ] ) / s;
                    z = ( m[ 0, 2 ] + m[ 2, 0 ] ) / s;
                    w = ( m[ 1, 2 ] - m[ 2, 1 ] ) / s;
                }
                else if( m[ 1, 1 ] > m[ 2, 2 ] )
                {
                    float s = MathUtils.Sqrt( 1.0f + m[ 1, 1 ] - m[ 0, 0 ] - m[ 2, 2 ] ) * 2.0f;
                    x = ( m[ 0, 1 ] + m[ 1, 0 ] ) / s;
                    y = 0.25f * s;
                    z = ( m[ 1, 2 ] + m[ 2, 1 ] ) / s;
                    w = ( m[ 0, 2 ] - m[ 2, 0 ] ) / s;
                }
                else
                {
                    float s = MathUtils.Sqrt( 1.0f + m[ 2, 2 ] - m[ 0, 0 ] - m[ 1, 1 ] ) * 2.0f;
                    x = ( m[ 0, 2 ] + m[ 2, 0 ] ) / s;
                    y = ( m[ 1, 2 ] + m[ 2, 1 ] ) / s;
                    z = 0.25f * s;
                    w = ( m[ 0, 1 ] - m[ 1, 0 ] ) / s;
                }
            }

            var q = new Quat4f( x, y, z, w );
            return q.Normalized();
        }

        /// <summary>
        /// Given 3 vectors of a rotated basis, returns a quaternion representing the rotation into the basis
        /// The three vectors do not have to be normalized but must be orthogonal and direct (X^Y=k*Z, with k>0).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static Quat4f FromRotatedBasis( Vector3f x, Vector3f y, Vector3f z )
        {
            return FromRotationMatrix( new Matrix3f( x, y, z ) );
        }

        public Quat4f( float x, float y, float z, float w )
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quat4f( Quat4f q )
        {
            this.x = q.x;
            this.y = q.y;
            this.z = q.z;
            this.w = q.w;
        }

        // public Quat4f( Vector3f axis, Vector

        public Quat4f( Vector3f axis, float angle )
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
            w = 1.0f;

            SetAxisAngle( axis, angle );
        }

        
        /// <summary>
        /// Construct a quaternion out of a vector.
        /// Let:
        /// v = < vx, vy, vz >
        /// Then q = 0 + vx * i + vy * j + vz * k
        /// </summary>
        /// <param name="v"></param>
        public Quat4f( Vector3f v )
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = 0;
        }

        /// <summary>
        /// Note that this rotation is not uniquely defined. The selected axis is usually orthogonal to \p from
        /// and \p to. However, this method is robust and can handle small or almost identical vectors.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Quat4f( Vector3f from, Vector3f to )
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
            w = 1.0f;

            float epsilon = 1e-10f;

            float fromSqNorm = from.NormSquared();
            float toSqNorm = to.Norm();
  
            // Identity quaternion when one vector is null
            if( ( fromSqNorm < epsilon ) || ( toSqNorm < epsilon ) )
            {
                // do nothing, already identity
            }
            else
            {
                Vector3f axis = Vector3f.Cross( from, to );
                float axisSqNorm = axis.NormSquared();

                // Aligned vectors, pick any axis, not aligned with from or to
                if( axisSqNorm < epsilon )
                {
                    axis = from.OrthogonalVector();
                }

                float angle = ( float )( Math.Asin( Math.Sqrt( axisSqNorm / ( fromSqNorm * toSqNorm ) ) ) );
                if( Vector3f.Dot( from, to ) < 0.0f )
                {
                    angle = ( float )( Math.PI - angle );
                }

                SetAxisAngle( axis, angle );
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

        public Vector3f Axis
        {
            get
            {
                Vector3f axis = new Vector3f( x, y, z );
                float norm = axis.Norm();
                if( norm > 1e-8 )
                {
                    axis /= norm;
                }

                if( Math.Acos( w ) <= 0.5 * Math.PI )
                {
                    return axis;
                }
                else
                {
                    return -axis;
                }
            }
        }

        public float Angle
        {
            get
            {
                float angle = ( float )( 2.0 * Math.Acos( w ) );
                if( angle < Math.PI )
                {
                    return angle;
                }
                else
                {
                    return ( ( float )( 2 * Math.PI - angle ) );
                }
            }
        }

        // TODO: use interior_ptr in C++
        public float this[ int i ]
        {
            get
            {
                switch( i )
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
                        throw new IndexOutOfRangeException( "Quaternions only have 4 elements, i = " + i );
                }
            }
            set
            {
                switch( i )
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
                        throw new IndexOutOfRangeException( "Quaternions only have 4 elements, i = " + i );
                }
            }
        }

        public void SetAxisAngle( Vector3f axis, float angle )
        {
            float norm = axis.Norm();
            if( norm < 1e-8 )
            {
                // null rotation
                x = 0;
                y = 0;
                z = 0;
                w = 1;
            }
            else
            {
                float halfAngle = 0.5f * angle;
                float sinHalfAngle = ( float )( Math.Sin( halfAngle ) );
                x = sinHalfAngle * axis.x / norm;
                y = sinHalfAngle * axis.y / norm;
                z = sinHalfAngle * axis.z / norm;
                w = ( float )( Math.Cos( halfAngle ) );
            }
        }

        public void GetAxisAngle( out Vector3f axis, out float angle )
        {
            axis = this.Axis;
            angle = this.Angle;
        }

        /// <summary>
        /// Rotates a vector using this quaternion.
        /// Equivalent to:
        /// Let q be this quaternion, q^-1 be its inverse, and p = ( v, 0 ) (real part 0)
        /// Returns ( q * p * q^-1 ).xyz
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3f Rotate( Vector3f v )
        {
            float q00 = 2.0f * x * x;
            float q11 = 2.0f * y * y;
            float q22 = 2.0f * z * z;

            float q01 = 2.0f * x * y;
            float q02 = 2.0f * x * z;
            float q03 = 2.0f * x * w;

            float q12 = 2.0f * y * z;
            float q13 = 2.0f * y * w;

            float q23 = 2.0f * z * w;

            return new Vector3f
            (
                ( 1.0f - q11 - q22 ) * v.x + ( q01 - q23 ) * v.y + ( q02 + q13 ) * v.z,
                ( q01 + q23 ) * v.x + ( 1.0f - q22 - q00 ) * v.y + ( q12 - q03 ) * v.z,
                ( q02 - q13 ) * v.x + ( q12 + q03 ) * v.y + ( 1.0f - q11 - q00 ) * v.z
            );
        }

        /// <summary>
        /// Rotates a vector using the inverse of this quaternion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3f InverseRotate( Vector3f v )
        {
            return Inverse().Rotate( v );
        }

        public Quat4f Conjugated()
        {
            return new Quat4f( -x, -y, -z, w );
        }

        public void Conjugate()
        {
            x = -x;
            y = -y;
            z = -z;
        }

        public Quat4f Inverse()
        {
            return Conjugated() / NormSquared();
        }

        public void Invert()
        {
            Normalize();
            Conjugate();
        }

        public void Negate()
        {
            Invert();
            w = -w;
        }

        public float Norm()
        {
            return ( float )( Math.Sqrt( x * x + y * y + z * z + w * w ) );
        }

        public float NormSquared()
        {
            return ( x * x + y * y + z * z + w * w );
        }

        public void Normalize()
        {
            float norm = Norm();
            x /= norm;
            y /= norm;
            z /= norm;
            w /= norm;
        }

        public Quat4f Normalized()
        {
            float norm = Norm();
            return new Quat4f( x / norm, y / norm, z / norm, w / norm );
        }

        public Matrix4f GetMatrix()
        {
            Matrix4f m = Matrix4f.Zero;

            float q00 = 2.0f * x * x;
            float q11 = 2.0f * y * y;
            float q22 = 2.0f * z * z;

            float q01 = 2.0f * x * y;
            float q02 = 2.0f * x * z;
            float q03 = 2.0f * x * w;

            float q12 = 2.0f * y * z;
            float q13 = 2.0f * y * w;

            float q23 = 2.0f * z * w;

            m[ 0, 0 ] = 1.0f - q11 - q22;
            m[ 1, 0 ] =        q01 - q23;
            m[ 2, 0 ] =        q02 + q13;

            m[ 0, 1 ] =        q01 + q23;
            m[ 1, 1 ] = 1.0f - q22 - q00;
            m[ 2, 1 ] =        q12 - q03;

            m[ 0, 2 ] =        q02 - q13;
            m[ 1, 2 ] =        q12 + q03;
            m[ 2, 2 ] = 1.0f - q11 - q00;

            m[ 0, 3 ] = 0.0f;
            m[ 1, 3 ] = 0.0f;
            m[ 2, 3 ] = 0.0f;

            m[ 3, 0 ] = 0.0f;
            m[ 3, 1 ] = 0.0f;
            m[ 3, 2 ] = 0.0f;
            m[ 3, 3 ] = 1.0f;

            return m;
        }

        public Matrix4f GetInverseMatrix()
        {
            return GetMatrix().Inverse();
        }

        public Quat4f Log()
        {            
            float norm = MathUtils.Sqrt( x * x + y * y + z * z );
            if( norm < 1e-6 )
            {
                return new Quat4f( x, y, z, 0.0f );
            }
            else
            {
                float coef = MathUtils.Acos( w ) / norm;
                return new Quat4f( x * coef, y * coef, z * coef, 0.0f );
            }
        }

        public Quat4f Exp()
        {
            float theta = ( float )( Math.Sqrt( x * x + y * y + z * z ) );

            if( theta < 1e-6 )
            {
                return new Quat4f( x, y, z, MathUtils.Cos( theta ) );
            }
            else
            {
                float coef = MathUtils.Sin( theta ) / theta;
                return new Quat4f( x * coef, y * coef, z * coef, MathUtils.Cos( theta ) );
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder( "< " );
            sb.AppendFormat( "{0:F6} ", w );

            if( x > 0 )
            {
                sb.AppendFormat( "+ {0:F6}i ", x );
            }
            else
            {
                sb.AppendFormat( "- {0:F6}i ", -x );
            }

            if( y > 0 )
            {
                sb.AppendFormat( "+ {0:F6}j ", y );
            }
            else
            {
                sb.AppendFormat( "- {0:F6}j ", -y );
            }

            if( z > 0 )
            {
                sb.AppendFormat( "+ {0:F6}k >", z );
            }
            else
            {
                sb.AppendFormat( "- {0:F6}k >", -z );
            }

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            if( obj is Quat4f )
            {
                return ( Equals( ( Quat4f ) obj ) );
            }
            return false;
        }

        public bool Equals( Quat4f other )
        {
            return ( x == other.x && y == other.y && z == other.z && w == other.w );
        }

        public static Quat4f operator + ( Quat4f a, Quat4f b )
        {
            return new Quat4f
            (
                a.x + b.x,
                a.y + b.y,
                a.z + b.z,
                a.w + b.w
            );
        }

        public static Quat4f operator - ( Quat4f a, Quat4f b )
        {
            return new Quat4f
            (
                a.x - b.x,
                a.y - b.y,
                a.z - b.z,
                a.w - b.w
            );
        }

        public static Quat4f operator - ( Quat4f q )
        {
            q.Negate();
            return q;
        }

        // For efficiency reasons, the result is not normalized
        // Call Normalize() on the result when composing small rotations
        public static Quat4f operator * ( Quat4f a, Quat4f b )
        {
            return new Quat4f
            (
                a.w * b.x + b.w * a.x + a.y * b.z - a.z * b.y,
                a.w * b.y + b.w * a.y + a.z * b.x - a.x * b.z,
                a.w * b.z + b.w * a.z + a.x * b.y - a.y * b.x,
                a.w * b.w - b.x * a.x - a.y * b.y - a.z * b.z
            );
        }

        public static Vector3f operator * ( Quat4f q, Vector3f v )
        {
            return q.Rotate( v );
        }

        public static Quat4f operator * ( Quat4f q, float f )
        {
            return new Quat4f( f * q.x, f * q.y, f * q.z, f * q.w );
        }

        public static Quat4f operator * ( float f, Quat4f q )
        {
            return new Quat4f( f * q.x, f * q.y, f * q.z, f * q.w );
        }

        public static Quat4f operator / ( Quat4f q, float f )
        {
            return new Quat4f( q.x / f, q.y / f, q.z / f, q.w / f );
        }

        public static bool operator == ( Quat4f lhs, Quat4f rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator != ( Quat4f lhs, Quat4f rhs )
        {
            return !( lhs.Equals( rhs ) );
        }

        public static float Dot( Quat4f a, Quat4f b )
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static Quat4f Slerp( Quat4f a, Quat4f b, float t )
        {
            return Slerp( a, b, t, true );
        }

        /// <summary>
        /// Returns the slerp interpolation of a and b, at time t.
        /// 
        /// t should range in [0,1]. Result is a when t=0 and b when t=1.
        /// 
        /// When allowFlip is true the slerp interpolation will always use the "shortest path"
        /// between the Quaternions' orientations, by "flipping" the source Quaternion if needed (see
        /// negate()).
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <param name="allowFlip"></param>
        /// <returns></returns>
        public static Quat4f Slerp( Quat4f a, Quat4f b, float t, bool allowFlip )
        {
            float cosAngle = Quat4f.Dot( a, b );

            float c1;
            float c2;

            // Linear interpolation for close orientations
            if( ( 1.0f - Math.Abs( cosAngle ) ) < 0.01f )
            {
                c1 = 1.0f - t;
                c2 = t;
            }
            else
            {
                // Spherical interpolation
                float angle = MathUtils.Acos( Math.Abs( cosAngle ) );
                float sinAngle = MathUtils.Sin( angle );
                c1 = MathUtils.Sin( angle * ( 1.0f - t ) ) / sinAngle;
                c2 = MathUtils.Sin( angle * t ) / sinAngle;
            }

            // Use the shortest path
            if( allowFlip && ( cosAngle < 0.0f ) )
            {
                c1 = -c1;
            }

            var q = new Quat4f
            (
                c1 * a.x + c2 * b.x,
                c1 * a.y + c2 * b.y,
                c1 * a.z + c2 * b.z,
                c1 * a.w + c2 * b.w
            );
            return q.Normalized();
        }

        /// <summary>
        /// Catmull-Rom cubic interpolation
        /// </summary>
        /// <param name="q0"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <param name="q3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Quat4f CubicInterpolate( Quat4f q0, Quat4f q1, Quat4f q2, Quat4f q3, float t )
        {
            // geometric construction:
	        //            t
	        //   (t+1)/2     t/2
	        // t+1        t	        t-1

	        // bottom level
	        var q0q1 = Quat4f.Slerp( q0, q1, t + 1 );
	        var q1q2 = Quat4f.Slerp( q1, q2, t );
	        var q2q3 = Quat4f.Slerp( q2, q3, t - 1 );

	        // middle level
	        var q0q1_q1q2 = Quat4f.Slerp( q0q1, q1q2, 0.5f * ( t + 1 ) );
	        var q1q2_q2q3 = Quat4f.Slerp( q1q2, q2q3, 0.5f * t );

	        // top level
	        return Quat4f.Slerp( q0q1_q1q2, q1q2_q2q3, t );
        }

        /// Returns the slerp interpolation of the two quaternions a and b, at time t, using
        /// tangents tgA and tgB.
        ///
        /// The resulting Quaternion is "between" a and b (result is a when t=0 and b for t=1).
        /// Use SquadTangent() to define the quaternion tangents tgA and tgB.
        public static Quat4f Squad( Quat4f a, Quat4f tangentA, Quat4f tangentB, Quat4f b, float t )
        {
            Quat4f ab = Quat4f.Slerp( a, b, t );
            Quat4f tg = Quat4f.Slerp( tangentA, tangentB, t, false );
            return Quat4f.Slerp( ab, tg, 2.0f * t * ( 1.0f - t ), false );
        }

        /// <summary>
        /// Returns the angular difference between a and b using the
        /// quaterion log map.
        /// The returned quaternion is purely imaginary (a 3-vector)
        /// that has a norm in [0,pi/2]
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Quat4f LogDifference( Quat4f a, Quat4f b )
        {
            Quat4f difference = a.Inverse() * b;
            return difference.Normalized().Log();
        }

        /// <summary>        
        /// Returns a tangent Quaternion for center, defined by before and after quaternions.
        /// Useful for smooth spline interpolation of quaternion with squad() and slerp(). */
        /// </summary>
        /// <param name="before"></param>
        /// <param name="center"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static Quat4f SquadTangent( Quat4f before, Quat4f center, Quat4f after )
        {
            Quat4f l1 = Quat4f.LogDifference( center, before );
            Quat4f l2 = Quat4f.LogDifference( center, after );
            Quat4f e;

            e.x = -0.25f * ( l1.x + l2.x );
            e.y = -0.25f * ( l1.y + l2.y );
            e.z = -0.25f * ( l1.z + l2.z );
            e.w = -0.25f * ( l1.w + l2.w );

            return ( center * ( e.Exp() ) );
        }

        /// <summary>
        /// returns a random orientation given 3 uniform randoms numbers u_i
        /// </summary>
        /// <param name="u0"></param>
        /// <param name="u1"></param>
        /// <param name="u2"></param>
        /// <returns></returns>
        public static Quat4f RandomQuaternion( float u0, float u1, float u2 )
        {
            double r1 = Math.Sqrt( 1.0 - u0 );
            double r2 = Math.Sqrt( u0 );
            double t1 = 2.0 * Math.PI * u1;
            double t2 = 2.0 * Math.PI * u2;
            return new Quat4f
            (
                ( float )( Math.Sin( t1 ) * r1 ),
                ( float )( Math.Cos( t1 ) * r1 ),
                ( float )( Math.Sin( t2 ) * r2 ),
                ( float )( Math.Cos( t2 ) * r2 )
            );
        }

        /// <summary>
        /// Returns the "rotational difference" a - b = a^-1 * b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Quat4f RotationalDifference( Quat4f a, Quat4f b )
        {
            return a.Inverse() * b;
        }
    }
}

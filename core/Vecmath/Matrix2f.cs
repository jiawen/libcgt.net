using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Matrix2f
    {        
        public float m00;
        public float m10;

        public float m01;
        public float m11;

        public static Matrix2f Zero
        {
            get
            {
                return new Matrix2f( 0 );
            }
        }

        public static Matrix2f Identity
        {
            get
            {
                var m = new Matrix2f( 0 );
                
                m[ 0, 0 ] = 1;
                m[ 1, 1 ] = 1;

                return m;
            }
        }

        /// <summary>
        /// A shear transformation parallel to the X axis.
        /// "Objects higher on the y axis are pushed to the right"
        /// [ 1 b ]
        /// [ 0 1 ]
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static Matrix2f ShearingX( float b )
        {
            var m = Matrix2f.Identity;
            m.m01 = b;
            return m;
        }

        /// <summary>
        /// A shear transformation parallel to the Y axis.
        /// "Objects to the right are pushed up"
        /// [ 1 0 ]
        /// [ b 1 ]
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static Matrix2f ShearingY( float b )
        {
            var m = Matrix2f.Identity;
            m.m10 = b;
            return m;
        }

        public static Matrix2f Scaling( float sx, float sy )
        {
            var m = Matrix2f.Identity;
            
            if( sx == 0 || sy == 0 )
            {
                var msg = string.Format( "You probably don't want a scale parameter of 0, got: sx = {0}, sy = {1}",
                    sx, sy );
                throw new ArgumentException( msg );
            }

            m.m00 = sx;
            m.m11 = sy;

            return m;
        }

        public static Matrix2f Scaling( Vector2f sxsy )
        {
            return Scaling( sxsy.x, sxsy.y );
        }

        public Matrix2f( float fillValue )
        {
            m00 = fillValue;
            m10 = fillValue;

            m01 = fillValue;
            m11 = fillValue;
        }

        public Matrix2f( float m00, float m01,
                        float m10, float m11 )
        {
            this.m00 = m00;
            this.m10 = m10;

            this.m01 = m01;
            this.m11 = m11;
        }

        public Matrix2f( float[ , ] elems )
        {
            m00 = elems[ 0, 0 ];
            m10 = elems[ 1, 0 ];

            m01 = elems[ 0, 1 ];
            m11 = elems[ 1, 1 ];           
        }

        /// <summary>
        /// Constructs a Matrix2f given 2 column vectors
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        public Matrix2f( Vector2f v0, Vector2f v1 )
        {
            m00 = v0.x;
            m10 = v0.y;
            
            m01 = v1.x;
            m11 = v1.y;
        }

        public Matrix2f( Matrix2f m )
        {
            this.m00 = m.m00;
            this.m10 = m.m10;

            this.m01 = m.m01;
            this.m11 = m.m11;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public float[] ColumnMajorElements
        {
            get
            {
                float[] elements = new float[ 4 ];

                elements[ 0 ] = m00;
                elements[ 1 ] = m10;

                elements[ 2 ] = m01;
                elements[ 3 ] = m11;

                return elements;
            }
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public float[] RowMajorElements
        {
            get
            {
                float[] elements = new float[ 4 ];

                elements[ 0 ] = m00;
                elements[ 1 ] = m01;

                elements[ 2 ] = m10;
                elements[ 3 ] = m11;

                return elements;
            }
        }

        public float this[ int k ]
        {
            get
            {
                // TODO: this is retarded
                switch( k )
                {
                    case 0:
                        return m00;
                    case 1:
                        return m10;
                    case 2:
                        return m01;
                    case 3:
                        return m11;

                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 3 for a 2x2 matrix." );
                }
            }
            set
            {
                switch( k )
                {
                    case 0:
                        m00 = value;
                        break;
                    case 1:
                        m10 = value;
                        break;
                    case 2:
                        m01 = value;
                        break;
                    case 3:
                        m11 = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 3 for a 2x2 matrix." );
                }
            }
        }

        public float this[ int i, int j ]
        {
            get
            {
                return this[ 2 * j + i ];
            }
            set
            {
                this[ 2 * j + i ] = value;
            }
        }     

        public float Determinant()
        {
            return( m00 * m11 - m01 * m10 );
        }        

        public Matrix2f Transposed()
        {
            return new Matrix2f( m00, m10, m01, m11 );
        }

        public override string ToString()
        {
            return string.Format( "[ {0:F6} {1:F6} ]\n[ {2:F6} {3:F6} ]",
                this[ 0, 0 ], this[ 0, 1 ], this[ 1, 0 ], this[ 1, 1 ] );
        }

        public Matrix2f Inverse()
        {
            float determinant = Determinant();
            
            if( determinant == 0.0f ) // exactly
            {
                throw new ArgumentException( "Matrix is singular." );
            }
            else
            {
                float reciprocalDeterminant = 1.0f / determinant;
                return reciprocalDeterminant * ( new Matrix2f( m11, -m01, -m10, m00 ) );
            }
        }

        public static Matrix2f operator + ( Matrix2f lhs, Matrix2f rhs )
        {
            return new Matrix2f
            (
                lhs.m00 + rhs.m00, lhs.m01 + rhs.m01,
                lhs.m10 + rhs.m10, lhs.m11 + rhs.m11
            );
        }

        public static Matrix2f operator - ( Matrix2f lhs, Matrix2f rhs )
        {
            return new Matrix2f
            (
                lhs.m00 - rhs.m00, lhs.m01 - rhs.m01,
                lhs.m10 - rhs.m10, lhs.m11 - rhs.m11
            );
        }

        public static Matrix2f operator - ( Matrix2f m )
        {
            return new Matrix2f
            (
                -m.m00, -m.m01,
                -m.m10, -m.m11
            );
        }

        public static Matrix2f operator * ( float a, Matrix2f rhs )
        {
            return new Matrix2f
            (
                a * rhs.m00, a * rhs.m01,
                a * rhs.m10, a * rhs.m11
            );
        }

        public static Matrix2f operator * ( Matrix2f lhs, float a )
        {
            return a * lhs;
        }

        public static Matrix2f operator * ( Matrix2f lhs, Matrix2f rhs )
        {
            return new Matrix2f
            (
                lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10, lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11,
                lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10, lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11
            );
        }

        public static Vector2f operator * ( Matrix2f lhs, Vector2f rhs )
        {
            return new Vector2f
            (
                lhs.m00 * rhs.x + lhs.m01 * rhs.y,
                lhs.m10 * rhs.x + lhs.m11 * rhs.y
            );
        } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Matrix3f
    {        
        private float m00;
        private float m10;
        private float m20;

        private float m01;
        private float m11;
        private float m21;

        private float m02;
        private float m12;
        private float m22;

        public static Matrix3f Zero
        {
            get
            {
                return new Matrix3f( 0 );
            }
        }

        public static Matrix3f Identity
        {
            get
            {
                var m = new Matrix3f( 0 );
                
                m[ 0, 0 ] = 1;
                m[ 1, 1 ] = 1;
                m[ 2, 2 ] = 1;

                return m;
            }
        }

        public static Matrix3f RotateBetween( Vector3f from, Vector3f to )
        {
            from.Normalize();
            to.Normalize();            

            var crossProduct = Vector3f.Cross( from, to );
            var normalizedAxis = crossProduct.Normalized();
            float sinTheta = crossProduct.Norm();
            float cosTheta = Vector3f.Dot( from, to );
            
            float x = normalizedAxis.x;
            float y = normalizedAxis.y;
            float z = normalizedAxis.z;
            
            var m = new Matrix3f();

            m[ 0, 0 ] = x * x * ( 1.0f - cosTheta ) + cosTheta;
            m[ 0, 1 ] = y * x * ( 1.0f - cosTheta ) - z * sinTheta;
            m[ 0, 2 ] = z * x * ( 1.0f - cosTheta ) + y * sinTheta;

            m[ 1, 0 ] = x * y * ( 1.0f - cosTheta ) + z * sinTheta;
            m[ 1, 1 ] = y * y * ( 1.0f - cosTheta ) + cosTheta;
            m[ 1, 2 ] = z * y * ( 1.0f - cosTheta ) - x * sinTheta;

            m[ 2, 0 ] = x * z * ( 1.0f - cosTheta ) - y * sinTheta;
            m[ 2, 1 ] = y * z * ( 1.0f - cosTheta ) + x * sinTheta;
            m[ 2, 2 ] = z * z * ( 1.0f - cosTheta ) + cosTheta;

            return m;
        }

        public static Matrix3f RotateAxis( Vector3f axis, float radians )
        {
            var normalizedAxis = axis.Normalized();
            float cosTheta = ( float )( Math.Cos( radians ) );
            float sinTheta = ( float )( Math.Sin( radians ) );

            float x = normalizedAxis.x;
            float y = normalizedAxis.y;
            float z = normalizedAxis.z;

            var m = new Matrix3f();

            m[ 0, 0 ] = x * x * ( 1.0f - cosTheta ) + cosTheta;
            m[ 0, 1 ] = y * x * ( 1.0f - cosTheta ) - z * sinTheta;
            m[ 0, 2 ] = z * x * ( 1.0f - cosTheta ) + y * sinTheta;

            m[ 1, 0 ] = x * y * ( 1.0f - cosTheta ) + z * sinTheta;
            m[ 1, 1 ] = y * y * ( 1.0f - cosTheta ) + cosTheta;
            m[ 1, 2 ] = z * y * ( 1.0f - cosTheta ) - x * sinTheta;

            m[ 2, 0 ] = x * z * ( 1.0f - cosTheta ) - y * sinTheta;
            m[ 2, 1 ] = y * z * ( 1.0f - cosTheta ) + x * sinTheta;
            m[ 2, 2 ] = z * z * ( 1.0f - cosTheta ) + cosTheta;

            return m;
        }

        public static Matrix3f RotateX( float radians )
        {            
            float c = ( float ) ( Math.Cos( radians ) );
            float s = ( float ) ( Math.Sin( radians ) );

            return new Matrix3f
            (
                1, 0, 0,
                0, c, -s,
                0, s, c
            );
        }

        public static Matrix3f RotateY( float radians )
        {
            float c = ( float ) ( Math.Cos( radians ) );
            float s = ( float ) ( Math.Sin( radians ) );

            return new Matrix3f
            (
                c, 0, s,
                0, 1, 0,
                -s, 0, c                
            );
        }

        public static Matrix3f RotateZ( float radians )
        {
            float c = ( float ) ( Math.Cos( radians ) );
            float s = ( float ) ( Math.Sin( radians ) );

            return new Matrix3f
            (
                c, -s, 0,
                s, c, 0,
                0, 0, 1
            );
        }

        public Matrix3f( float fillValue )
        {
            m00 = fillValue;
            m10 = fillValue;
            m20 = fillValue;

            m01 = fillValue;
            m11 = fillValue;
            m21 = fillValue;

            m02 = fillValue;
            m12 = fillValue;
            m22 = fillValue;
        }

        public Matrix3f( float m00, float m01, float m02,
                        float m10, float m11, float m12,
                        float m20, float m21, float m22 )
        {
            this.m00 = m00;
            this.m10 = m10;
            this.m20 = m20;

            this.m01 = m01;
            this.m11 = m11;
            this.m21 = m21;

            this.m02 = m02;
            this.m12 = m12;
            this.m22 = m22;
        }

        public Matrix3f( float[ , ] elems )
        {
            m00 = elems[ 0, 0 ];
            m10 = elems[ 1, 0 ];
            m20 = elems[ 2, 0 ];

            m01 = elems[ 0, 1 ];
            m11 = elems[ 1, 1 ];
            m21 = elems[ 2, 1 ];

            m02 = elems[ 0, 2 ];
            m12 = elems[ 1, 2 ];
            m22 = elems[ 2, 2 ];
        }

        /// <summary>
        /// Constructs a Matrix3f given 3 column vectors
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        public Matrix3f( Vector3f v0, Vector3f v1, Vector3f v2 )
        {
            m00 = v0.x;
            m10 = v0.y;
            m20 = v0.z;
            
            m01 = v1.x;
            m11 = v1.y;
            m21 = v1.z;
            
            m02 = v2.x;
            m12 = v2.y;
            m22 = v2.z;
        }

        public Matrix3f( Matrix3f m )
        {
            this.m00 = m.m00;
            this.m10 = m.m10;
            this.m20 = m.m20;

            this.m01 = m.m01;
            this.m11 = m.m11;
            this.m21 = m.m21;

            this.m02 = m.m02;
            this.m12 = m.m12;
            this.m22 = m.m22;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public float[] ColumnMajorElements
        {
            get
            {
                float[] elements = new float[ 9 ];

                elements[ 0 ] = m00;
                elements[ 1 ] = m10;
                elements[ 2 ] = m20;

                elements[ 3 ] = m01;
                elements[ 4 ] = m11;
                elements[ 5 ] = m21;

                elements[ 6 ] = m02;
                elements[ 7 ] = m12;
                elements[ 8 ] = m22;

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
                float[] elements = new float[ 9 ];

                elements[ 0 ] = m00;
                elements[ 1 ] = m01;
                elements[ 2 ] = m02;

                elements[ 3 ] = m10;
                elements[ 4 ] = m11;
                elements[ 5 ] = m12;

                elements[ 6 ] = m20;
                elements[ 7 ] = m21;
                elements[ 8 ] = m22;

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
                        return m20;
                    
                    case 3:
                        return m01;
                    case 4:
                        return m11;
                    case 5:
                        return m21;

                    case 6:
                        return m02;
                    case 7:
                        return m12;
                    case 8:
                        return m22;

                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 8 for a 3x3 matrix." );
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
                        m20 = value;
                        break;
                    
                    case 3:
                        m01 = value;
                        break;
                    case 4:
                        m11 = value;
                        break;
                    case 5:
                        m21 = value;
                        break;

                    case 6:
                        m02 = value;
                        break;
                    case 7:
                        m12 = value;
                        break;
                    case 8:
                        m22 = value;
                        break;
                }
            }
        }

        public float this[ int i, int j ]
        {
            get
            {
                return this[ 3 * j + i ];
            }
            set
            {
                this[ 3 * j + i ] = value;
            }
        }     

        public float Determinant()
        {
            return MatrixUtil.Determinant3x3f( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
        }

        public Vector3f GetColumn( int j )
        {
            return new Vector3f
            (
                this[ 0, j ],
                this[ 1, j ],
                this[ 2, j ]
            );
        }

        public void SetColumn( int j, Vector3f v )
        {
            this[ 0, j ] = v.x;
            this[ 1, j ] = v.y;
            this[ 2, j ] = v.z;
        }

        public Matrix2f GetSubMatrix2x2( int i0, int j0 )
        {
            if( i0 < 0 || i0 > 1 || j0 < 0 || j0 > 1 )
            {
                throw new ArgumentException( "Matrix3f.GetSubMatrix2x2: i0 and j0 can only be 0 or 1" );
            }

            Matrix2f output = Matrix2f.Zero;

            for( int i = 0; i < 2; ++i )
            {
	            for( int j = 0; j < 2; ++j )
	            {
		            output[ i, j ] = this[ i + i0, j + j0 ];
	            }
            }

            return output;
        }

        public void SetSubMatrix2x2( int i0, int j0, Matrix2f m )
        {
            if( i0 < 0 || i0 > 1 || j0 < 0 || j0 > 1 )
            {
                throw new ArgumentException( "Matrix3f.SetSubMatrix2x2: i0 and j0 can only be 0 or 1" );
            }

            for( int i = 0; i < 2; ++i )
            {
	            for( int j = 0; j < 2; ++j )
	            {
		            this[ i + i0, j + j0 ] = m[ i, j ];
	            }
            }
        }

        public Matrix3f Transposed()
        {
            return new Matrix3f
            (
                m00, m10, m20,
                m01, m11, m21,
                m02, m12, m22                
            );
        }

        public override string ToString()
        {
            return string.Format( "[ {0:F6} {1:F6} {2:F6} ]\n[ {3:F6} {4:F6} {5:F6} ]\n[ {6:F6} {7:F6} {8:F6} ]",
                m00, m01, m02,
                m10, m11, m12,
                m20, m21, m22 );
        }

        public Matrix3f Inverse()
        {
            float cofactor00 =  MatrixUtil.Determinant2x2f( m11, m12, m21, m22 );
	        float cofactor01 = -MatrixUtil.Determinant2x2f( m10, m12, m20, m22 );
	        float cofactor02 =  MatrixUtil.Determinant2x2f( m10, m11, m20, m21 );

	        float cofactor10 = -MatrixUtil.Determinant2x2f( m01, m02, m21, m22 );
	        float cofactor11 =  MatrixUtil.Determinant2x2f( m00, m02, m20, m22 );
	        float cofactor12 = -MatrixUtil.Determinant2x2f( m00, m01, m20, m21 );

	        float cofactor20 =  MatrixUtil.Determinant2x2f( m01, m02, m11, m12 );
	        float cofactor21 = -MatrixUtil.Determinant2x2f( m00, m02, m10, m12 );
	        float cofactor22 =  MatrixUtil.Determinant2x2f( m00, m01, m10, m11 );

            float determinant = m00 * cofactor00 + m01 * cofactor01 + m02 * cofactor02;

            if( determinant == 0.0f ) // exactly
            {
                throw new ArgumentException( "Matrix is singular." );
            }
            else
            {
                float reciprocalDeterminant = 1.0f / determinant;

                Matrix3f inverse = new Matrix3f
                (
                    cofactor00 * reciprocalDeterminant, cofactor10 * reciprocalDeterminant, cofactor20 * reciprocalDeterminant,
                    cofactor01 * reciprocalDeterminant, cofactor11 * reciprocalDeterminant, cofactor21 * reciprocalDeterminant,
                    cofactor02 * reciprocalDeterminant, cofactor12 * reciprocalDeterminant, cofactor22 * reciprocalDeterminant
                );

                return inverse;
            }
        }

        public static Matrix3f operator + ( Matrix3f lhs, Matrix3f rhs )
        {
            Matrix3f m = new Matrix3f();

            for( int i = 0; i < 3; ++i )
            {
                for( int j = 0; j < 3; ++j )
                {
                    m[ i, j ] = lhs[ i, j ] + rhs[ i, j ];
                }
            }

            return m;
        }

        public static Matrix3f operator - ( Matrix3f lhs, Matrix3f rhs )
        {
            Matrix3f m = new Matrix3f();

            for( int i = 0; i < 3; ++i )
            {
                for( int j = 0; j < 3; ++j )
                {
                    m[ i, j ] = lhs[ i, j ] - rhs[ i, j ];
                }
            }

            return m;
        }

        public static Matrix3f operator - ( Matrix3f m )
        {
            return new Matrix3f
            (
                -m.m00, -m.m01, -m.m02,
                -m.m10, -m.m11, -m.m12,
                -m.m20, -m.m21, -m.m22
            );
        }

        public static Matrix3f operator * ( float a, Matrix3f rhs )
        {
            // TODO: make more efficient
            Matrix3f m = new Matrix3f();

            for( int i = 0; i < 3; ++i )
            {
                for( int j = 0; j < 3; ++j )
                {
                    m[ i, j ] = a * rhs[ i, j ];
                }
            }

            return m;
        }

        public static Matrix3f operator * ( Matrix3f lhs, float a )
        {
            return a * lhs;
        }

        public static Matrix3f operator * ( Matrix3f lhs, Matrix3f rhs )
        {
            Matrix3f m = new Matrix3f();

            for( int i = 0; i < 3; ++i )
            {
                for( int j = 0; j < 3; ++j )
                {
                    for( int k = 0; k < 3; ++k )
                    {
                        m[ i, k ] += lhs[ i, j ] * rhs[ j, k ];
                    }
                }
            }

            return m;
        }

        public static Vector3f operator * ( Matrix3f lhs, Vector3f rhs )
        {
            return new Vector3f
            (
                lhs.m00 * rhs.x + lhs.m01 * rhs.y + lhs.m02 * rhs.z,
                lhs.m10 * rhs.x + lhs.m11 * rhs.y + lhs.m12 * rhs.z,
                lhs.m20 * rhs.x + lhs.m21 * rhs.y + lhs.m22 * rhs.z
            );
        }

        public static Matrix3f operator / ( Matrix3f lhs, float rhs )
        {
            return new Matrix3f
            (
                lhs.m00 / rhs, lhs.m01 / rhs, lhs.m02 / rhs,
                lhs.m10 / rhs, lhs.m11 / rhs, lhs.m12 / rhs,
                lhs.m20 / rhs, lhs.m21 / rhs, lhs.m22 / rhs
            );
        }
    }
}

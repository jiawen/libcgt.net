using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable, StructLayout( LayoutKind.Sequential )]
    public struct Matrix4f
    {
        public float m00;
        public float m10;
        public float m20;
        public float m30;

        public float m01;
        public float m11;
        public float m21;
        public float m31;

        public float m02;
        public float m12;
        public float m22;
        public float m32;

        public float m03;
        public float m13;
        public float m23;
        public float m33;

        public static Matrix4f Zero
        {
            get
            {
                return new Matrix4f( 0 );
            }
        }

        public static Matrix4f Identity
        {
            get
            {
                Matrix4f m = new Matrix4f( 0 );
                
                m[ 0, 0 ] = 1;
                m[ 1, 1 ] = 1;
                m[ 2, 2 ] = 1;
                m[ 3, 3 ] = 1;

                return m;
            }
        }

        public static Matrix4f Translation( float tx, float ty, float tz )
        {
            var m = Matrix4f.Identity;
            
            m.m03 = tx;
            m.m13 = ty;
            m.m23 = tz;

            return m;
        }
        
        public static Matrix4f Translation( Vector3f t )
        {
            var m = Matrix4f.Identity;
            
            m.m03 = t.x;
            m.m13 = t.y;
            m.m23 = t.z;

            return m;
        }

        public static Matrix4f UniformScaling( float s )
        {
            return Scaling( s, s, s );
        }

        public static Matrix4f Scaling( float sx, float sy, float sz )
        {
            var m = Matrix4f.Identity;
            
            m.m00 = sx;
            m.m11 = sy;
            m.m22 = sz;

            return m;
        }

        public static Matrix4f LookAt( Vector3f eye, Vector3f center, Vector3f up )
        {
            var z = ( eye - center ).Normalized();
            var y = up;
            var x = Vector3f.Cross( y, z );

            var view = new Matrix4f();

            view.SetRow( 0, new Vector4f( x, -Vector3f.Dot( x, eye ) ) );
	        view.SetRow( 1, new Vector4f( y, -Vector3f.Dot( y, eye ) ) );
	        view.SetRow( 2, new Vector4f( z, -Vector3f.Dot( z, eye ) ) );
	        view.SetRow( 3, new Vector4f( 0, 0, 0, 1 ) );

            return view;
        }

        /// <summary>
        /// Generate a rotation matrix R such that to = R * from
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Matrix4f RotateBetween( Vector3f from, Vector3f to )
        {
            if( from.Equals( to ) )
            {
                return Matrix4f.Identity;
            }

            var crossProduct = Vector3f.Cross( from, to );            
            var normalizedAxis = crossProduct.Normalized();
            float sinTheta = crossProduct.Norm();
            float cosTheta = Vector3f.Dot( from, to );
            
            float x = normalizedAxis.x;
            float y = normalizedAxis.y;
            float z = normalizedAxis.z;
            
            var m = new Matrix4f();

            m[ 0, 0 ] = x * x * ( 1.0f - cosTheta ) + cosTheta;
            m[ 0, 1 ] = y * x * ( 1.0f - cosTheta ) - z * sinTheta;
            m[ 0, 2 ] = z * x * ( 1.0f - cosTheta ) + y * sinTheta;
            m[ 0, 3 ] = 0.0f;

            m[ 1, 0 ] = x * y * ( 1.0f - cosTheta ) + z * sinTheta;
            m[ 1, 1 ] = y * y * ( 1.0f - cosTheta ) + cosTheta;
            m[ 1, 2 ] = z * y * ( 1.0f - cosTheta ) - x * sinTheta;
            m[ 1, 3 ] = 0.0f;

            m[ 2, 0 ] = x * z * ( 1.0f - cosTheta ) - y * sinTheta;
            m[ 2, 1 ] = y * z * ( 1.0f - cosTheta ) + x * sinTheta;
            m[ 2, 2 ] = z * z * ( 1.0f - cosTheta ) + cosTheta;
            m[ 2, 3 ] = 0.0f;

            m[ 3, 0 ] = 0.0f;
            m[ 3, 1 ] = 0.0f;
            m[ 3, 2 ] = 0.0f;
            m[ 3, 3 ] = 1.0f;

            return m;
        }

        /// <summary>
        /// Generate a rotation matrix R about axis through the origin.
        /// TODO: rotate about point and axis?
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Matrix4f RotateAxis( Vector3f axis, float radians )
        {
            var normalizedAxis = axis.Normalized();
            float cosTheta = ( float )( Math.Cos( radians ) );
            float sinTheta = ( float )( Math.Sin( radians ) );

            float x = normalizedAxis.x;
            float y = normalizedAxis.y;
            float z = normalizedAxis.z;

            var m = new Matrix4f();

            m[ 0, 0 ] = x * x * ( 1.0f - cosTheta ) + cosTheta;
            m[ 0, 1 ] = y * x * ( 1.0f - cosTheta ) - z * sinTheta;
            m[ 0, 2 ] = z * x * ( 1.0f - cosTheta ) + y * sinTheta;
            m[ 0, 3 ] = 0.0f;

            m[ 1, 0 ] = x * y * ( 1.0f - cosTheta ) + z * sinTheta;
            m[ 1, 1 ] = y * y * ( 1.0f - cosTheta ) + cosTheta;
            m[ 1, 2 ] = z * y * ( 1.0f - cosTheta ) - x * sinTheta;
            m[ 1, 3 ] = 0.0f;

            m[ 2, 0 ] = x * z * ( 1.0f - cosTheta ) - y * sinTheta;
            m[ 2, 1 ] = y * z * ( 1.0f - cosTheta ) + x * sinTheta;
            m[ 2, 2 ] = z * z * ( 1.0f - cosTheta ) + cosTheta;
            m[ 2, 3 ] = 0.0f;

            m[ 3, 0 ] = 0.0f;
            m[ 3, 1 ] = 0.0f;
            m[ 3, 2 ] = 0.0f;
            m[ 3, 3 ] = 1.0f;

            return m;
        }

        public static Matrix4f RotateX( float radians )
        {            
            float c = ( float ) ( Math.Cos( radians ) );
            float s = ( float ) ( Math.Sin( radians ) );

            return new Matrix4f
            (
                1, 0, 0, 0,
                0, c, -s, 0,
                0, s, c, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4f RotateY( float radians )
        {
            float c = ( float ) ( Math.Cos( radians ) );
            float s = ( float ) ( Math.Sin( radians ) );

            return new Matrix4f
            (
                c, 0, s, 0,
                0, 1, 0, 0,
                -s, 0, c, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4f RotateZ( float radians )
        {
            float c = ( float ) ( Math.Cos( radians ) );
            float s = ( float ) ( Math.Sin( radians ) );

            return new Matrix4f
            (
                c, -s, 0, 0,
                s, c, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1
            );
        }

        /// <summary>
        /// fovy = field of view in the y direction, *in radians*
        /// aspect = width / height
        /// </summary>
        /// <param name="fovYRadians"></param>
        /// <param name="aspect"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <returns></returns>
        public static Matrix4f PerspectiveOpenGL( float fovYRadians, float aspect, float zNear, float zFar )
        {
            var m = Matrix4f.Zero;
            
            float yScale = MathUtils.Cot( 0.5f * fovYRadians );
            float xScale = yScale / aspect;

            m.m00 = xScale;
            m.m11 = yScale;
            m.m22 = ( zFar + zNear ) / ( zNear - zFar );
            
            m.m23 = 2 * zFar * zNear / ( zNear - zFar );
            m.m32 = -1;

            return m;
        }

        public static Matrix4f PerspectiveOffCenterOpenGL( float left, float right, float bottom, float top, float zNear, float zFar )
        {
            var m = Matrix4f.Zero;

            m.m00 = 2.0f * zNear / ( right - left );
            m.m11 = 2.0f * zNear / ( top - bottom );
            m.m22 = ( zFar + zNear ) / ( zNear - zFar );

            m.m02 = ( left + right ) / ( right - left );
            m.m12 = ( top + bottom ) / ( top - bottom );
            m.m23 = 2 * zNear * zFar / ( zNear - zFar );
            m.m32 = -1;

            return m;
        }

        /// <summary>
        /// fovy = field of view in the y direction, *in radians*
        /// aspect = width / height
        /// with zFar set to infinity        
        /// </summary>
        /// <param name="fovYRadians"></param>
        /// <param name="aspect"></param>
        /// <param name="zNear"></param>
        /// <returns></returns>
        public static Matrix4f PerspectiveOpenGLInfiniteZFar( float fovYRadians, float aspect, float zNear )
        {
            var m = Matrix4f.Zero;
            
            float yScale = MathUtils.Cot( 0.5f * fovYRadians );
            float xScale = yScale / aspect;

            m.m00 = xScale;
            m.m11 = yScale;
            m.m22 = -1;
            m.m23 = -2 * zNear;
            m.m32 = -1;

            return m;
        }

        public static Matrix4f PerspectiveOffCenterOpenGLInfiniteZFar( float left, float right, float bottom, float top, float zNear )
        {
            var m = Matrix4f.Zero;

            m.m00 = 2.0f * zNear / ( right - left );
            m.m11 = 2.0f * zNear / ( top - bottom );
            m.m22 = -1;

            m.m02 = ( left + right ) / ( right - left );
            m.m12 = ( top + bottom ) / ( top - bottom );
            m.m23 = -2 * zNear;
            m.m32 = -1;

            return m;
        }

        public static Matrix4f PerspectiveD3D( float fovYRadians, float aspect, float zNear, float zFar )
        {
            var m = Matrix4f.Zero;
            
            float yScale = MathUtils.Cot( 0.5f * fovYRadians );
            float xScale = yScale / aspect;

            m.m00 = xScale;
            m.m11 = yScale;
            m.m22 = zFar / ( zNear - zFar );
            m.m23 = zNear * zFar / ( zNear - zFar );
            m.m32 = -1;

            return m;
        }

        public static Matrix4f PerspectiveD3DInfiniteZFar( float fovYRadians, float aspect, float zNear )
        {
            var m = Matrix4f.Zero;
            
            float yScale = MathUtils.Cot( 0.5f * fovYRadians );
            float xScale = yScale / aspect;

            m.m00 = xScale;
            m.m11 = yScale;
            m.m22 = -1;
            m.m23 = -zNear;
            m.m32 = -1;

            return m;
        }        

        public static Matrix4f PerspectiveOffCenterD3D( float left, float right, float bottom, float top, float zNear, float zFar )
        {
            var m = Matrix4f.Zero;

            m.m00 = 2.0f * zNear / ( right - left );
            m.m11 = 2.0f * zNear / ( top - bottom );
            m.m22 = zFar / ( zNear - zFar );

            m.m02 = ( left + right ) / ( right - left );
            m.m12 = ( top + bottom ) / ( top - bottom );
            m.m23 = zNear * zFar / ( zNear - zFar );
            m.m32 = -1;

            return m;
        }

        public static Matrix4f PerspectiveOffCenterD3DInfiniteZFar( float left, float right, float bottom, float top, float zNear )
        {
            var m = Matrix4f.Zero;

            m.m00 = 2.0f * zNear / ( right - left );
            m.m11 = 2.0f * zNear / ( top - bottom );
            m.m22 = -1;

            m.m02 = ( left + right ) / ( right - left );
            m.m12 = ( top + bottom ) / ( top - bottom );
            m.m23 = -zNear;
            m.m32 = -1;

            return m;
        }

        public static Matrix4f OrthoD3D( float width, float height, float zNear, float zFar )
        {
            var m = Matrix4f.Zero;

            m.m00 = 2.0f / width;
            m.m11 = 2.0f / height;
            m.m22 = 1.0f / ( zNear - zFar );
            m.m33 = 1.0f;

            m.m03 = -1;
            m.m13 = -1;
            m.m23 = zNear / ( zNear - zFar );

            return m;
        }

        public static Matrix4f OrthoD3D( float left, float right, float bottom, float top, float zNear, float zFar )
        {
            var m = Matrix4f.Zero;

            m.m00 = 2.0f / ( right - left );
            m.m11 = 2.0f / ( top - bottom );
            m.m22 = 1.0f / ( zNear - zFar );
            m.m33 = 1.0f;

            m.m03 = ( left + right ) / ( left - right );
            m.m13 = ( top + bottom ) / ( bottom - top );
            m.m23 = zNear / ( zNear - zFar );

            return m;
        }

        /// <summary>
        /// Returns the matrix for a viewport transformation:
        /// mapping coordinates in [-1,1] x [-1,1] to [0,screenWidth] x [0,screenHeight]
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <returns></returns>
        public static Matrix4f Viewport( float screenWidth, float screenHeight )
        {
            var m = Matrix4f.Identity;

            m.m00 = 0.5f * screenWidth;
            m.m11 = 0.5f * screenHeight;
            m.m03 = 0.5f * screenWidth;
            m.m13 = 0.5f * screenHeight;

            return m;
        }

        public static Matrix4f InverseViewport( float screenWidth, float screenHeight )
        {
            var m = Matrix4f.Identity;

            m.m00 = 2.0f / screenWidth;
            m.m11 = 2.0f / screenHeight;
            m.m03 = -1;
            m.m13 = -1;

            return m;
        }

        public Matrix4f( float fillValue )
        {
            m00 = fillValue;
            m10 = fillValue;
            m20 = fillValue;
            m30 = fillValue;

            m01 = fillValue;
            m11 = fillValue;
            m21 = fillValue;
            m31 = fillValue;

            m02 = fillValue;
            m12 = fillValue;
            m22 = fillValue;
            m32 = fillValue;

            m03 = fillValue;
            m13 = fillValue;
            m23 = fillValue;
            m33 = fillValue;
        }

        public Matrix4f( float m00, float m01, float m02, float m03,
                        float m10, float m11, float m12, float m13,
                        float m20, float m21, float m22, float m23,
                        float m30, float m31, float m32, float m33 )
        {
            this.m00 = m00;
            this.m10 = m10;
            this.m20 = m20;
            this.m30 = m30;

            this.m01 = m01;
            this.m11 = m11;
            this.m21 = m21;
            this.m31 = m31;

            this.m02 = m02;
            this.m12 = m12;
            this.m22 = m22;
            this.m32 = m32;

            this.m03 = m03;
            this.m13 = m13;
            this.m23 = m23;
            this.m33 = m33;
        }

        public Matrix4f( float[ , ] elems )
        {
            m00 = elems[ 0, 0 ];
            m10 = elems[ 1, 0 ];
            m20 = elems[ 2, 0 ];
            m30 = elems[ 3, 0 ];

            m01 = elems[ 0, 1 ];
            m11 = elems[ 1, 1 ];
            m21 = elems[ 2, 1 ];
            m31 = elems[ 3, 1 ];

            m02 = elems[ 0, 2 ];
            m12 = elems[ 1, 2 ];
            m22 = elems[ 2, 2 ];
            m32 = elems[ 3, 2 ];

            m03 = elems[ 0, 3 ];
            m13 = elems[ 1, 3 ];
            m23 = elems[ 2, 3 ];
            m33 = elems[ 3, 3 ];
        }

        /// <summary>
        /// Constructs a Matrix4f given 4 column vectors
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        public Matrix4f( Vector4f v0, Vector4f v1, Vector4f v2, Vector4f v3 )
        {
            m00 = v0.x;
            m10 = v0.y;
            m20 = v0.z;
            m30 = v0.w;
            
            m01 = v1.x;
            m11 = v1.y;
            m21 = v1.z;
            m31 = v1.w;
            
            m02 = v2.x;
            m12 = v2.y;
            m22 = v2.z;
            m32 = v2.w;
            
            m03 = v3.x;
            m13 = v3.y;
            m23 = v3.z;
            m33 = v3.w;            
        }

        public Matrix4f( Matrix4f m )
        {
            this.m00 = m.m00;
            this.m10 = m.m10;
            this.m20 = m.m20;
            this.m30 = m.m30;

            this.m01 = m.m01;
            this.m11 = m.m11;
            this.m21 = m.m21;
            this.m31 = m.m31;

            this.m02 = m.m02;
            this.m12 = m.m12;
            this.m22 = m.m22;
            this.m32 = m.m32;

            this.m03 = m.m03;
            this.m13 = m.m13;
            this.m23 = m.m23;
            this.m33 = m.m33;
        }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public float[] ColumnMajorElements
        {
            get
            {
                float[] elements = new float[ 16 ];

                elements[ 0 ] = m00;
                elements[ 1 ] = m10;
                elements[ 2 ] = m20;
                elements[ 3 ] = m30;

                elements[ 4 ] = m01;
                elements[ 5 ] = m11;
                elements[ 6 ] = m21;
                elements[ 7 ] = m31;

                elements[ 8 ] = m02;
                elements[ 9 ] = m12;
                elements[ 10 ] = m22;
                elements[ 11 ] = m32;

                elements[ 12 ] = m03;
                elements[ 13 ] = m13;
                elements[ 14 ] = m23;
                elements[ 15 ] = m33;

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
                float[] elements = new float[ 16 ];

                elements[ 0 ] = m00;
                elements[ 1 ] = m01;
                elements[ 2 ] = m02;
                elements[ 3 ] = m03;

                elements[ 4 ] = m10;
                elements[ 5 ] = m11;
                elements[ 6 ] = m12;
                elements[ 7 ] = m13;

                elements[ 8 ] = m20;
                elements[ 9 ] = m21;
                elements[ 10 ] = m22;
                elements[ 11 ] = m23;

                elements[ 12 ] = m30;
                elements[ 13 ] = m31;
                elements[ 14 ] = m32;
                elements[ 15 ] = m33;

                return elements;
            }
        }

        public unsafe float this[ int k ]
        {
            get
            {
                fixed( float* arr = &m00 )
                {
                    return arr[ k ];
                }
            }
            set
            {
                fixed( float* arr = &m00 )
                {
                    arr[ k ] = value;
                }                
            }
        }

        public unsafe float this[ int i, int j ]
        {
            get
            {
                fixed( float* arr = &m00 )
                {
                    int k = 4 * j + i;
                    return arr[ k ];
                }                
            }
            set
            {
                fixed( float* arr = &m00 )
                {
                    int k = 4 * j + i;
                    arr[ k ] = value;
                }                
            }
        }

#if false
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
                        return m30;
                    
                    case 4:
                        return m01;
                    case 5:
                        return m11;
                    case 6:
                        return m21;
                    case 7:
                        return m31;

                    case 8:
                        return m02;
                    case 9:
                        return m12;
                    case 10:
                        return m22;
                    case 11:
                        return m32;

                    case 12:
                        return m03;
                    case 13:
                        return m13;
                    case 14:
                        return m23;
                    case 15:
                        return m33;

                    default:
                        throw new IndexOutOfRangeException( "k must be between 0 and 15 for a 4x4 matrix." );
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
                        m30 = value;
                        break;
                    
                    case 4:
                        m01 = value;
                        break;
                    case 5:
                        m11 = value;
                        break;
                    case 6:
                        m21 = value;
                        break;
                    case 7:
                        m31 = value;
                        break;

                    case 8:
                        m02 = value;
                        break;
                    case 9:
                        m12 = value;
                        break;
                    case 10:
                        m22 = value;
                        break;
                    case 11:
                        m32 = value;
                        break;

                    case 12:
                        m03 = value;
                        break;
                    case 13:
                        m13 = value;
                        break;
                    case 14:
                        m23 = value;
                        break;
                    case 15:
                        m33 = value;
                        break;
                }
            }
        }

        public float this[ int i, int j ]
        {
            get
            {
                return this[ 4 * j + i ];
            }
            set
            {
                this[ 4 * j + i ] = value;
            }
        }
#endif


        public float Determinant()
        {
            return
            (
                m00 * MatrixUtil.Determinant3x3f( m11, m12, m13, m21, m22, m23, m31, m32, m33 ) -
                m01 * MatrixUtil.Determinant3x3f( m10, m12, m13, m20, m22, m23, m30, m32, m33 ) +
                m02 * MatrixUtil.Determinant3x3f( m10, m11, m13, m20, m21, m23, m30, m31, m33 ) -
                m03 * MatrixUtil.Determinant3x3f( m10, m11, m12, m20, m21, m22, m30, m31, m32 )
            );
        }        

        public void Scale( float a )
        {
            m00 *= a;
            m10 *= a;
            m20 *= a;
            m30 *= a;
                  
            m01 *= a;
            m11 *= a;
            m21 *= a;
            m31 *= a;
                  
            m02 *= a;
            m12 *= a;
            m22 *= a;
            m32 *= a;
                  
            m03 *= a;
            m13 *= a;
            m23 *= a;
            m33 *= a;
        }

        public Matrix4f Transposed()
        {
            return new Matrix4f
            (
                m00, m10, m20, m30,
                m01, m11, m21, m31,
                m02, m12, m22, m32,
                m03, m13, m23, m33
            );
        }

        public Vector4f GetRow( int i )
        {
	        return new Vector4f
	        (
		        this[ i ],
		        this[ i + 4 ],
		        this[ i + 8 ],
		        this[ i + 12 ]
	        );
        }

        public void SetRow( int i, Vector4f v )
        {
            this[ i ] = v.x;
            this[ i + 4 ] = v.y;
            this[ i + 8 ] = v.z;
            this[ i + 12 ] = v.w;
        }

        public Vector4f GetColumn( int j )
        {
            int colStart = 4 * j;

	        return new Vector4f
	        (
		        this[ colStart ],
		        this[ colStart + 1 ],
		        this[ colStart + 2 ],
		        this[ colStart + 3 ]
	        );
        }

        public void SetColumn( int j, Vector4f v )
        {
            int colStart = 4 * j;

            this[ colStart ] = v.x;
            this[ colStart + 1 ] = v.y;
            this[ colStart + 2 ] = v.z;
	        this[ colStart + 3 ] = v.w;
        }

        public Matrix3f GetSubmatrix3x3( int i0, int j0 )
        {
            var m = new Matrix3f();

	        for( int i = 0; i < 3; ++i )
	        {
		        for( int j = 0; j < 3; ++j )
		        {
			        m[ i, j ] = this[ i + i0, j + j0 ];
		        }
	        }

	        return m;
        }
        
        public void SetSubmatrix3x3( int i0, int j0, Matrix3f m )
        {
            for( int i = 0; i < 3; ++i )
	        {
		        for( int j = 0; j < 3; ++j )
		        {
			        this[ i + i0, j + j0 ] = m[ i, j ];
		        }
	        }
        }

        public override string ToString()
        {
            return string.Format( "[ {0:F6} {1:F6} {2:F6} {3:F6} ]\n[ {4:F6} {5:F6} {6:F6} {7:F6} ]\n[ {8:F6} {9:F6} {10:F6} {11:F6} ]\n[ {12:F6} {13:F6} {14:F6} {15:F6} ]",
                this[ 0, 0 ], this[ 0, 1 ], this[ 0, 2 ], this[ 0, 3 ], this[ 1, 0 ], this[ 1, 1 ], this[ 1, 2 ], this[ 1, 3 ], this[ 2, 0 ], this[ 2, 1 ], this[ 2, 2 ], this[ 2, 3 ], this[ 3, 0 ], this[ 3, 1 ], this[ 3, 2 ], this[ 3, 3 ] );
        }

        public Matrix4f Inverse()
        {
            float cofactor00 = MatrixUtil.Determinant3x3f( m11, m12, m13, m21, m22, m23, m31, m32, m33 );
            float cofactor01 = -MatrixUtil.Determinant3x3f( m12, m13, m10, m22, m23, m20, m32, m33, m30 );
            float cofactor02 = MatrixUtil.Determinant3x3f( m13, m10, m11, m23, m20, m21, m33, m30, m31 );
            float cofactor03 = -MatrixUtil.Determinant3x3f( m10, m11, m12, m20, m21, m22, m30, m31, m32 );

            float cofactor10 = -MatrixUtil.Determinant3x3f( m21, m22, m23, m31, m32, m33, m01, m02, m03 );
            float cofactor11 = MatrixUtil.Determinant3x3f( m22, m23, m20, m32, m33, m30, m02, m03, m00 );
            float cofactor12 = -MatrixUtil.Determinant3x3f( m23, m20, m21, m33, m30, m31, m03, m00, m01 );
            float cofactor13 = MatrixUtil.Determinant3x3f( m20, m21, m22, m30, m31, m32, m00, m01, m02 );

            float cofactor20 = MatrixUtil.Determinant3x3f( m31, m32, m33, m01, m02, m03, m11, m12, m13 );
            float cofactor21 = -MatrixUtil.Determinant3x3f( m32, m33, m30, m02, m03, m00, m12, m13, m10 );
            float cofactor22 = MatrixUtil.Determinant3x3f( m33, m30, m31, m03, m00, m01, m13, m10, m11 );
            float cofactor23 = -MatrixUtil.Determinant3x3f( m30, m31, m32, m00, m01, m02, m10, m11, m12 );

            float cofactor30 = -MatrixUtil.Determinant3x3f( m01, m02, m03, m11, m12, m13, m21, m22, m23 );
            float cofactor31 = MatrixUtil.Determinant3x3f( m02, m03, m00, m12, m13, m10, m22, m23, m20 );
            float cofactor32 = -MatrixUtil.Determinant3x3f( m03, m00, m01, m13, m10, m11, m23, m20, m21 );
            float cofactor33 = MatrixUtil.Determinant3x3f( m00, m01, m02, m10, m11, m12, m20, m21, m22 );

            float determinant = m00 * cofactor00 + m01 * cofactor01 + m02 * cofactor02 + m03 * cofactor03;

            if( determinant == 0.0f ) // exactly
            {
                throw new ArgumentException( "Matrix is singular." );
            }
            else
            {
                float reciprocalDeterminant = 1.0f / determinant;

                Matrix4f inverse = new Matrix4f( cofactor00 * reciprocalDeterminant,
                    cofactor10 * reciprocalDeterminant,
                    cofactor20 * reciprocalDeterminant,
                    cofactor30 * reciprocalDeterminant,
                    cofactor01 * reciprocalDeterminant,
                    cofactor11 * reciprocalDeterminant,
                    cofactor21 * reciprocalDeterminant,
                    cofactor31 * reciprocalDeterminant,
                    cofactor02 * reciprocalDeterminant,
                    cofactor12 * reciprocalDeterminant,
                    cofactor22 * reciprocalDeterminant,
                    cofactor32 * reciprocalDeterminant,
                    cofactor03 * reciprocalDeterminant,
                    cofactor13 * reciprocalDeterminant,
                    cofactor23 * reciprocalDeterminant,
                    cofactor33 * reciprocalDeterminant );

                return inverse;
            }
        }

        /// <summary>
        /// Transforms v using this matrix:
        /// out = ( this * float4( v, 1 ) ).xyz        
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3f Transform( Vector3f v )
        {
            var u = new Vector4f( v, 1 );
            var o = this * u;
            return o.XYZ;
        }

        /// <summary>
        /// Transforms and homogenizes v using this matrix:
        /// out = ( this * float4( v, 1 ) ).Homogenized()
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3f TransformHomogenized( Vector3f v )
        {
            /*
            var u = new Vector4f( v, 1 );
            var o = this * u;
            return o.Homogenized();
            */

            float x = m00 * v.x + m01 * v.y + m02 * v.z + m03;
            float y = m10 * v.x + m11 * v.y + m12 * v.z + m13;
            float z = m20 * v.x + m21 * v.y + m22 * v.z + m23;
            float w = m30 * v.x + m31 * v.y + m32 * v.z + m33;

            float rw = 1.0f / w;
            return new Vector3f( rw * x, rw * y, rw * z );
        }

        public static Matrix4f operator + ( Matrix4f lhs, Matrix4f rhs )
        {
            Matrix4f m = new Matrix4f();

            for( int i = 0; i < 4; ++i )
            {
                for( int j = 0; j < 4; ++j )
                {
                    m[ i, j ] = lhs[ i, j ] + rhs[ i, j ];
                }
            }

            return m;
        }

        public static Matrix4f operator - ( Matrix4f lhs, Matrix4f rhs )
        {
            Matrix4f m = new Matrix4f();

            for( int i = 0; i < 4; ++i )
            {
                for( int j = 0; j < 4; ++j )
                {
                    m[ i, j ] = lhs[ i, j ] - rhs[ i, j ];
                }
            }

            return m;
        }

        public static Matrix4f operator - ( Matrix4f m )
        {
            return new Matrix4f
            (
                -m.m00, -m.m01, -m.m02, -m.m03,
                -m.m10, -m.m11, -m.m12, -m.m13,
                -m.m20, -m.m21, -m.m22, -m.m23,
                -m.m30, -m.m31, -m.m32, -m.m33
            );
        }

        public static Matrix4f operator * ( float a, Matrix4f rhs )
        {
            // TODO: make more efficient
            Matrix4f m = new Matrix4f();

            for( int i = 0; i < 4; ++i )
            {
                for( int j = 0; j < 4; ++j )
                {
                    m[ i, j ] = a * rhs[ i, j ];
                }
            }

            return m;
        }

        public static Matrix4f operator * ( Matrix4f lhs, float a )
        {
            return a * lhs;
        }

        public static Matrix4f operator * ( Matrix4f lhs, Matrix4f rhs )
        {
            Matrix4f m = new Matrix4f();

            for( int i = 0; i < 4; ++i )
            {
                for( int j = 0; j < 4; ++j )
                {
                    for( int k = 0; k < 4; ++k )
                    {
                        m[ i, k ] += lhs[ i, j ] * rhs[ j, k ];
                    }
                }
            }

            return m;
        }

        public static Vector4f operator * ( Matrix4f lhs, Vector4f rhs )
        {
            return new Vector4f
            (
                lhs.m00 * rhs.x + lhs.m01 * rhs.y + lhs.m02 * rhs.z + lhs.m03 * rhs.w,
                lhs.m10 * rhs.x + lhs.m11 * rhs.y + lhs.m12 * rhs.z + lhs.m13 * rhs.w,
                lhs.m20 * rhs.x + lhs.m21 * rhs.y + lhs.m22 * rhs.z + lhs.m23 * rhs.w,
                lhs.m30 * rhs.x + lhs.m31 * rhs.y + lhs.m32 * rhs.z + lhs.m33 * rhs.w
            );
        }

#if false
        public static Vector4f operator * ( Matrix4f lhs, Vector4f rhs )
        {
            return new Vector4f
            (
                lhs[ 0, 0 ] * rhs.X + lhs[ 0, 1 ] * rhs.Y + lhs[ 0, 2 ] * rhs.Z + lhs[ 0, 3 ] * rhs.W,
                lhs[ 1, 0 ] * rhs.X + lhs[ 1, 1 ] * rhs.Y + lhs[ 1, 2 ] * rhs.Z + lhs[ 1, 3 ] * rhs.W,
                lhs[ 2, 0 ] * rhs.X + lhs[ 2, 1 ] * rhs.Y + lhs[ 2, 2 ] * rhs.Z + lhs[ 2, 3 ] * rhs.W,
                lhs[ 3, 0 ] * rhs.X + lhs[ 3, 1 ] * rhs.Y + lhs[ 3, 2 ] * rhs.Z + lhs[ 3, 3 ] * rhs.W
            );
        }
#endif
    }
}

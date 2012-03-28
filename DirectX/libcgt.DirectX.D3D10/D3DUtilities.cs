using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;
using libcgt.DirectX.D3D10;
using libcgt.DirectX.D3D10.VertexFormats;

using SlimDX;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10
{
    public static class D3DUtilities
    {
        public static Matrix ToD3DMatrix( this Matrix4f matrix )
        {
            return new Matrix
            {
                M11 = matrix[ 0, 0 ], M12 = matrix[ 0, 1 ], M13 = matrix[ 0, 2 ], M14 = matrix[ 0, 3 ],
                M21 = matrix[ 1, 0 ], M22 = matrix[ 1, 1 ], M23 = matrix[ 1, 2 ], M24 = matrix[ 1, 3 ],
                M31 = matrix[ 2, 0 ], M32 = matrix[ 2, 1 ], M33 = matrix[ 2, 2 ], M34 = matrix[ 2, 3 ],
                M41 = matrix[ 3, 0 ], M42 = matrix[ 3, 1 ], M43 = matrix[ 3, 2 ], M44 = matrix[ 3, 3 ]
            };
        }

        public static Vector2 ToD3DVector3( this Vector2f vector )
        {
            return new Vector2( vector.x, vector.y );
        }

        public static Vector3 ToD3DVector3( this Vector3f vector )
        {
            return new Vector3( vector.x, vector.y, vector.z );
        }

        // TODO: implicit conversions
        // TODO: memory cast using Marshal
        public static Color4 ToD3DColor4( this Vector4f vector )
        {
            return new Color4( vector.w, vector.x, vector.y, vector.z );
        }

        public static Vector4 ToD3DVector4( this Vector4f vector )
        {
            return new Vector4( vector.x, vector.y, vector.z, vector.w );
        }

        public static DynamicVertexBuffer CreateScreenAlignedQuad( int width, int height )
        {
            return CreateScreenAlignedQuad( width, height, 0 );
        }

        public static DynamicVertexBuffer CreateScreenAlignedQuad( Vector2i size )
        {
            return CreateScreenAlignedQuad( size, 0 );
        }

        public static DynamicVertexBuffer CreateScreenAlignedQuad( int width, int height, float z )
        {
            var buffer = new DynamicVertexBuffer( VertexPosition3fTexture2f.SizeInBytes, 6 );

            var stream = buffer.MapForWriteDiscard();
                
            stream.Write( new VertexPosition3fTexture2f( 0, 0, z, 0, 0 ) );
            stream.Write( new VertexPosition3fTexture2f( width, 0, z, 1, 0 ) );
            stream.Write( new VertexPosition3fTexture2f( 0, height, z, 0, 1 ) );

            stream.Write( new VertexPosition3fTexture2f( 0, height, z, 0, 1 ) );
            stream.Write( new VertexPosition3fTexture2f( width, 0, z, 1, 0 ) );
            stream.Write( new VertexPosition3fTexture2f( width, height, z, 1, 1 ) );
            
            stream.Close();
            buffer.Unmap();

            return buffer;
        }

        public static DynamicVertexBuffer CreateScreenAlignedQuad( Vector2i size, float z )
        {
            return CreateScreenAlignedQuad( size.x, size.y, z );
        }

        public static Result Set( this EffectVectorVariable var, Vector3f v )
        {
            return var.Set( v.ToD3DVector3() );
        }

        public static Result Set( this EffectVectorVariable var, Vector4f v )
        {
            return var.Set( v.ToD3DVector4() );
        }

        public static Result Set( this EffectVectorVariable var, Vector4f[] v )
        {
            var arr = new Vector4[ v.Length ];
            for( int i = 0; i < v.Length; ++i )
            {
                arr[ i ] = v[ i ].ToD3DVector4();
            }

            return var.Set( arr );
        }

        public static Result SetMatrix( this EffectMatrixVariable var, Matrix4f m )
        {
            return var.SetMatrix( m.ToD3DMatrix() );
        }

        public static ResourceRegion Rect2iToRegion( Rect2i rect )
        {
            return new ResourceRegion
            {
                Left = rect.Origin.x,
                Right = rect.TopRight.x,
                Top = rect.Origin.y,
                Bottom = rect.TopRight.y,
                Front = 0,
                Back = 1
            };
        }

        public static ResourceRegion CalculateResourceRegion2D( int width, int height )
        {
            return new ResourceRegion
            {
                Left = 0,
                Right = width,
                Top = 0,
                Bottom = height,
                Front = 0,
                Back = 1
            };
        }

        public static void CalculateDataBoxPitches2D( int elementSizeBytes, int width, int height,
            out int rowPitch, out int slicePitch )            
        {
            // Source Row Pitch = [size of one element in bytes] * [number of elements in one row] 
            // Source Depth Pitch = [Source Row Pitch] * [number of rows (height)] 
            rowPitch = elementSizeBytes * width;
            slicePitch = rowPitch * height;            
        }

        /// <summary>
        /// Calculate a subresource index for a texture
        /// </summary>
        /// <param name="mipSlice">A zero-based index into an array of subtextures; 0 indicates the first, most detailed subtexture (or mipmap level).</param>
        /// <param name="arraySlice">The zero-based index of the first texture to use (in an array of textures).</param>
        /// <param name="mipLevels">Number of mipmap levels (or subtextures) to use.</param>
        /// <returns></returns>
        public static int CalculateSubresource( int mipSlice, int arraySlice, int mipLevels )
        {
            return mipSlice + ( arraySlice * mipLevels );
        }
    }
}

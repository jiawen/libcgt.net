using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;
using SlimDX;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using MapFlags=SlimDX.Direct3D10.MapFlags;

namespace libcgt.DirectX.D3D10
{
    /// <summary>
    /// A 8-bit per component, RGBA texture, that's GPU access only
    /// and GPU shader resource only
    /// </summary>
    public class StaticTexture2D
    {
        public Vector2i Size
        {
            get
            {
                return new Vector2i( Width, Height );
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Texture2D Texture { get; private set; }
        public ShaderResourceView ShaderResourceView { get; private set; }

        // TODO: other formats: read file type
        public static StaticTexture2D FromFile( string filename )
        {
            var image = Image4ub.FromFile( filename );
            return FromImage( image );
        }

        // TODO: other formats
        public static StaticTexture2D FromFileMipMapped( string filename )
        {
            var image = Image4ub.FromFile( filename );
            var tex = CreateUnsignedByte4MipMapped( image.Width, image.Height, 0 );
            tex.Update( image );
            D3D10Wrapper.Instance.Device.GenerateMips( tex.ShaderResourceView );
            return tex;
        }

        public static StaticTexture2D FromImage( Image4ub image )
        {
            var tex = CreateUnsignedByte4( image.Size );
            tex.Update( image );
            return tex;
        }

        public static StaticTexture2D FromImage( Image1f image )
        {
            var tex = CreateFloat1( image.Size );
            tex.Update( image );
            return tex;
        }

        public static StaticTexture2D FromImage( Image4f image )
        {
            var tex = CreateFloat4( image.Size );
            tex.Update( image );
            return tex;
        }

        public static StaticTexture2D CreateUnsignedByte4( Vector2i size )
        {
            return CreateUnsignedByte4( size.x, size.y );
        }
        
        public static StaticTexture2D CreateUnsignedByte4( int width, int height )
        {
            var texture = TextureFactory.CreateUnsignedByte4( width, height,
                ResourceUsage.Default, BindFlags.ShaderResource,
                CpuAccessFlags.None );     
            return new StaticTexture2D( width, height, texture );
        }

        public static StaticTexture2D CreateUnsignedByte4MipMapped( int width, int height, int mipLevels )
        {
            var texture = TextureFactory.CreateUnsignedByte4MipMapped( width, height, mipLevels,
                ResourceUsage.Default, BindFlags.ShaderResource,
                CpuAccessFlags.None );     
            return new StaticTexture2D( width, height, texture );
        }

        public static StaticTexture2D CreateFloat1( Vector2i size )
        {
            return CreateFloat1( size.x, size.y );
        }

        public static StaticTexture2D CreateFloat1( int width, int height )
        {
            var texture = TextureFactory.CreateFloat1( width, height,
                ResourceUsage.Default, BindFlags.ShaderResource,
                CpuAccessFlags.None );
            return new StaticTexture2D( width, height, texture );
        }

        public static StaticTexture2D CreateFloat4( Vector2i size )
        {
            return CreateFloat4( size.x, size.y );
        }

        public static StaticTexture2D CreateFloat4( int width, int height )
        {
            var texture = TextureFactory.CreateFloat4( width, height,
                ResourceUsage.Default, BindFlags.ShaderResource,
                CpuAccessFlags.None );
            return new StaticTexture2D( width, height, texture );
        }

        // TODO: this can't really handle mipmaps
        // it works for now because it's the zero-eth
        /// <summary>
        /// Updates the entire static texture with an image
        /// </summary>
        /// <param name="src"></param>
        public unsafe void Update( Image4ub src )
        {
            // Source Row Pitch = [size of one element in bytes] * [number of elements in one row] 
            // Source Depth Pitch = [Source Row Pitch] * [number of rows (height)] 
            var srcRowPitch = 4 * src.Width;
            var srcDepthPitch = srcRowPitch * src.Height;            

            var dstRect = new Rect2i( src.Width, src.Height );
            var dstRegion = D3DUtilities.Rect2iToRegion( dstRect );

            fixed( byte* p = src.Pixels )
            {
                var buffer = ( IntPtr ) p;
                var srcStream = new DataStream( buffer, 4 * src.NumPixels, true, false );                                
                var srcBox = new DataBox( srcRowPitch, srcDepthPitch, srcStream );

                D3D10Wrapper.Instance.Device.UpdateSubresource( srcBox, Texture, 0, dstRegion );
            }
        }

        // TODO: this can't really handle mipmaps
        // it works for now because it's the zero-eth
        /// <summary>
        /// Updates the entire static texture with an image
        /// </summary>
        /// <param name="src"></param>
        public unsafe void Update( Image1f src )
        {
            // Source Row Pitch = [size of one element in bytes] * [number of elements in one row] 
            // Source Depth Pitch = [Source Row Pitch] * [number of rows (height)] 
            var srcRowPitch = sizeof( float ) * src.Width;
            var srcDepthPitch = srcRowPitch * src.Height;            

            var dstRect = new Rect2i( src.Width, src.Height );
            var dstRegion = D3DUtilities.Rect2iToRegion( dstRect );

            fixed( float* p = src.Pixels )
            {
                var buffer = ( IntPtr ) p;
                var srcStream = new DataStream( buffer, sizeof( float ) * src.NumPixels, true, false );                                
                var srcBox = new DataBox( srcRowPitch, srcDepthPitch, srcStream );

                D3D10Wrapper.Instance.Device.UpdateSubresource( srcBox, Texture, 0, dstRegion );
            }
        }

        // TODO: this can't really handle mipmaps
        // it works for now because it's the zero-eth
        /// <summary>
        /// Updates the entire static texture with an image
        /// </summary>
        /// <param name="src"></param>
        public unsafe void Update( Image4f src )
        {
            // Source Row Pitch = [size of one element in bytes] * [number of elements in one row] 
            // Source Depth Pitch = [Source Row Pitch] * [number of rows (height)] 
            var srcRowPitch = 4 * sizeof( float ) * src.Width;
            var srcDepthPitch = srcRowPitch * src.Height;            

            var dstRect = new Rect2i( src.Width, src.Height );
            var dstRegion = D3DUtilities.Rect2iToRegion( dstRect );

            fixed( float* p = src.Pixels )
            {
                var buffer = ( IntPtr ) p;
                var srcStream = new DataStream( buffer, 4 * sizeof( float ) * src.NumPixels, true, false );                                
                var srcBox = new DataBox( srcRowPitch, srcDepthPitch, srcStream );

                D3D10Wrapper.Instance.Device.UpdateSubresource( srcBox, Texture, 0, dstRegion );
            }
        }
        
        public unsafe void UpdateRectangle( Image4ub src, Rect2i dstRect )
        {
            // Source Row Pitch = [size of one element in bytes] * [number of elements in one row] 
            // Source Depth Pitch = [Source Row Pitch] * [number of rows (height)] 
            var srcRowPitch = 4 * src.Width;
            var srcDepthPitch = srcRowPitch * src.Height;            
            
            var dstRegion = D3DUtilities.Rect2iToRegion( dstRect );

            fixed( byte* p = src.Pixels )
            {
                var buffer = ( IntPtr ) p;
                var srcStream = new DataStream( buffer, 4 * src.NumPixels, true, false );                                
                var srcBox = new DataBox( srcRowPitch, srcDepthPitch, srcStream );

                D3D10Wrapper.Instance.Device.UpdateSubresource( srcBox, Texture, 0, dstRegion );
            }
        }
        
        public void SavePNG( string filename )
        {
            Texture.SavePNG( filename );
        }

        private StaticTexture2D( int width, int height, Texture2D texture )
        {
            Width = width;
            Height = height;
            Texture = texture;

            // ShaderResourceView = new ShaderResourceView( D3D10Wrapper.Instance.Device, Texture );

            var srvd = new ShaderResourceViewDescription
            {
                Format = texture.Description.Format,
                Dimension = ShaderResourceViewDimension.Texture2D,
                MostDetailedMip = 0,
                MipLevels = texture.Description.MipLevels
            };

            ShaderResourceView = new ShaderResourceView( D3D10Wrapper.Instance.Device, texture, srvd );
        }        
    }
}

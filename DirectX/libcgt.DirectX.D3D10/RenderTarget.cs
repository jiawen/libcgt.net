using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10
{
    public class RenderTarget : IDisposable
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
        public RenderTargetView RenderTargetView { get; private set; }

        // creates a standard render target
        public static RenderTarget CreateFloat4( Vector2i size )
        {
            return CreateFloat4( size.x, size.y );
        }

        // creates a standard render target
        public static RenderTarget CreateFloat4( int width, int height )
        {
            var texture = TextureFactory.CreateFloat4( width, height,
                ResourceUsage.Default,
                BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags.None );
            return new RenderTarget( width, height, texture );
        }

        // creates a standard render target
        public static RenderTarget CreateFloat2( Vector2i size )
        {
            return CreateFloat2( size.x, size.y );
        }

        // creates a standard render target
        public static RenderTarget CreateFloat2( int width, int height )
        {
            var texture = TextureFactory.CreateFloat2( width, height,
                ResourceUsage.Default,
                BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags.None );            
            return new RenderTarget( width, height, texture );
        }

        // creates a standard render target
        public static RenderTarget CreateFloat1( Vector2i size )
        {
            return CreateFloat1( size.x, size.y );
        }

        // creates a standard render target
        public static RenderTarget CreateFloat1( int width, int height )
        {
            var texture = TextureFactory.CreateFloat1( width, height,
                ResourceUsage.Default,
                BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags.None );            
            return new RenderTarget( width, height, texture );
        }

        // creates a standard render target that's
        // a render target that can also be used as a texture (shader resource)
        public static RenderTarget CreateUnsignedByte4( Vector2i size )
        {
            return CreateUnsignedByte4( size.x, size.y );
        }

        // creates a standard render target that's
        // a render target that can also be used as a texture (shader resource)
        public static RenderTarget CreateUnsignedByte4( int width, int height )
        {            
            var texture = TextureFactory.CreateUnsignedByte4( width, height,
                ResourceUsage.Default,
                BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags.None );
            return new RenderTarget( width, height, texture );
        }        

        // TODO: this is duplicated
        // TODO: make a nice way of creating 2D DataBoxes
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

        // TODO: do this properly
        public void Dispose()
        {
            ShaderResourceView.Dispose();
            RenderTargetView.Dispose();
            Texture.Dispose();
        }

        private RenderTarget( int width, int height, Texture2D texture )
        {
            Width = width;
            Height = height;

            Texture = texture;

            var wrapper = D3D10Wrapper.Instance;
            RenderTargetView = new RenderTargetView( wrapper.Device, texture );
            ShaderResourceView = new ShaderResourceView( wrapper.Device, texture );
        }
    }
}

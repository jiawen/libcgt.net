using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10
{
    public class DepthStencilTarget : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture2D Texture { get; private set; }
        public DepthStencilView DepthStencilView { get; private set; }

        // creates a standard depth+stencil buffer
        // which cannot be read by the cpu
        public static DepthStencilTarget CreateDepth( Vector2i size )
        {
            return CreateDepth( size.x, size.y );
        }

        // creates a standard depth+stencil buffer
        // which cannot be read by the cpu
        public static DepthStencilTarget CreateDepth( int width, int height )
        {
            var texture = TextureFactory.CreateDepth( width, height,
                ResourceUsage.Default,
                BindFlags.DepthStencil,
                CpuAccessFlags.None );

            return new DepthStencilTarget( width, height, texture );
        }

        // TODO: Clear()?

        // TODO: do this properly
        public void Dispose()
        {
            DepthStencilView.Dispose();
            Texture.Dispose();
        }

        private DepthStencilTarget( int width, int height, Texture2D texture )
        {
            Width = width;
            Height = height;

            Texture = texture;

            var wrapper = D3D10Wrapper.Instance;
            DepthStencilView = new DepthStencilView( wrapper.Device, texture );
        }
    }
}

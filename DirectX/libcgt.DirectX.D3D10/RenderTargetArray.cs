using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10
{
    public class RenderTargetArray
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ArraySize { get; private set; }

        public Texture2D Texture { get; private set; }

        public RenderTargetView FullRenderTargetView { get; private set; }
        public ShaderResourceView FullShaderResourceView { get; private set; }
        
        /// <summary>
        /// Returns a new render target view for array indices [i0, ..., i0 + count)
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public RenderTargetView SliceRenderTargetView( int i0, int count )
        {
            var desc = new RenderTargetViewDescription
            {
               Format = Texture.Description.Format,
               Dimension = RenderTargetViewDimension.Texture2DArray,
               MipSlice = 0,
               FirstArraySlice = i0,
               ArraySize = count
            };

            return new RenderTargetView( D3D10Wrapper.Instance.Device, Texture, desc );
        }

        /// <summary>
        /// Returns a new shader resource view for array indices [i0, ..., i0 + count)
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ShaderResourceView SliceShaderResourceView( int i0, int count )
        {
            var desc = new ShaderResourceViewDescription
            {
                Format = Texture.Description.Format,
                Dimension = ShaderResourceViewDimension.Texture2DArray,
                
                // D3D10_TEX2D_ARRAY_SRV
                MostDetailedMip = 0,
                MipLevels = 1,
                FirstArraySlice = i0,
                ArraySize = count
            };

            return new ShaderResourceView( D3D10Wrapper.Instance.Device, Texture, desc );
        }

        // TODO: resize()
        // creates a standard render target
        public static RenderTargetArray CreateFloat2( Vector2i size, int arraySize )
        {
            return CreateFloat2( size.x, size.y, arraySize );
        }

        // creates a standard render target
        public static RenderTargetArray CreateFloat2( int width, int height, int arraySize )
        {
            var texture = TextureFactory.CreateFloat2Array( width, height, arraySize,
                ResourceUsage.Default,
                BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags.None );            
            return new RenderTargetArray( width, height, arraySize, texture );
        }

        // TODO: do this properly
        public void Dispose()
        {
            FullShaderResourceView.Dispose();
            FullRenderTargetView.Dispose();
            Texture.Dispose();
        }

        private RenderTargetArray( int width, int height, int arraySize, Texture2D texture )
        {
            Width = width;
            Height = height;
            ArraySize = arraySize;

            Texture = texture;

            FullRenderTargetView = SliceRenderTargetView( 0, arraySize );
            FullShaderResourceView = SliceShaderResourceView( 0, arraySize );
        }
    }
}

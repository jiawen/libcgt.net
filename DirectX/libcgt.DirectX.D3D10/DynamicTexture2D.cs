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
    /// A 8-bit per component, RGBA texture, that's CPU write-only
    /// and GPU shader resource only
    /// </summary>
    public class DynamicTexture2D
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

        public static DynamicTexture2D FromImage( Image4f im )
        {
            var tex = CreateFloat4( im.Size );
            tex.Update( im );
            return tex;
        }

        public static DynamicTexture2D FromImage( Image4ub im )
        {
            var tex = CreateUnsignedByte4( im.Size );
            tex.Update( im );
            return tex;
        }

        public static DynamicTexture2D CreateUnsignedByte4( Vector2i size )
        {
            return CreateUnsignedByte4( size.x, size.y );
        }

        public static DynamicTexture2D CreateUnsignedByte4( int width, int height )
        {
            var texture = TextureFactory.CreateUnsignedByte4( width, height,
                ResourceUsage.Dynamic, BindFlags.ShaderResource,
                CpuAccessFlags.Write );     
            return new DynamicTexture2D( width, height, texture );
        }

        public static DynamicTexture2D CreateFloat4( Vector2i size )
        {
            return CreateFloat4( size.x, size.y );
        }

        public static DynamicTexture2D CreateFloat4( int width, int height )
        {
            var texture = TextureFactory.CreateFloat4( width, height,
                ResourceUsage.Dynamic, BindFlags.ShaderResource,
                CpuAccessFlags.Write );
            return new DynamicTexture2D( width, height, texture );
        }

        public void Resize( int width, int height )
        {
            Width = width;
            Height = height;

            Texture2D newTexture;
            if( Texture.Description.Format == Format.R32G32B32A32_Float )
            {
                newTexture = TextureFactory.CreateFloat4( width, height,
                    ResourceUsage.Dynamic, BindFlags.ShaderResource,
                    CpuAccessFlags.Write );
            }
            else
            {
                newTexture = TextureFactory.CreateUnsignedByte4( width, height,
                    ResourceUsage.Dynamic, BindFlags.ShaderResource,
                    CpuAccessFlags.Write );
            }

            ShaderResourceView.Dispose();
            Texture.Dispose();

            Texture = newTexture;
            ShaderResourceView = new ShaderResourceView( D3D10Wrapper.Instance.Device, Texture );
        }

        // then the pitch actually matters
        public void Update( Image4ub im )
        {
            if( !( im.Size.Equals( Size ) ) )
            {
                throw new ArgumentException( "Image size mismatch" );
            }

            var rect = Texture.Map( 0, MapMode.WriteDiscard, MapFlags.None );            
            int pitch = rect.Pitch;

            int width = im.Width;
            int height = im.Height;
            var pixels = im.Pixels;
            const int pixelSizeBytes = 4;

            // if the pitch matches, then just write it all at once
            if( pitch == pixelSizeBytes * width )
            {
                rect.Data.Write( pixels, 0, pixels.Length );
            }
            else
            {
                // otherwise, write one row at a time
                for( int y = 0; y < height; ++y )
                {
                    rect.Data.Position = y * pitch;
                    rect.Data.Write( pixels, y * pixelSizeBytes * width, pixelSizeBytes * width );
                }
            }

            rect.Data.Close();
            Texture.Unmap( 0 );
        }
        
        public void Update( Image4f im )
        {
            if( Texture.Description.Format != Format.R32G32B32A32_Float )
            {
                throw new ArgumentException( "Can only update a float texture with a float image" );
            }
            
            var rect = Texture.Map( 0, MapMode.WriteDiscard, MapFlags.None );
            int pitch = rect.Pitch;

            int width = im.Width;
            int height = im.Height;
            var pixels = im.Pixels;
            const int pixelSizeBytes = 4 * sizeof( float );

            // if the pitch matches, then just write it all at once
            if( pitch == pixelSizeBytes * width )
            {
                rect.Data.WriteRange( pixels );
            }
            else
            {
                // otherwise, write one row at a time
                for( int y = 0; y < height; ++y )
                {
                    rect.Data.Position = y * pitch;
                    rect.Data.WriteRange( pixels, 4 * y * width, 4 * width );
                }
            }

            rect.Data.Close();
            Texture.Unmap( 0 );
        }

        public void UpdateRectangles( IList< Image4ub > images, IList< Rect2i > targets )
        {
#if false
            var wrapper = D3D10Wrapper.Instance;

            var im = new Image4ub( this.Size, Color4ub.Red );
            fixed( void* ptr = im.Pixels )
            {
                var userBuffer = new IntPtr( ptr );
                var dataStream = new DataStream( userBuffer, 4 * im.NumPixels, true, true );
                var sourceBox = new DataBox( 4 * im.Width, 4 * im.Width * im.Height, dataStream );

                var targetRegion = new ResourceRegion
                {
                    Back = 1,
                    Front = 0,
                    Bottom = 4096,
                    Top = 0,
                    Left = 0,
                    Right = 4096
                };

                wrapper.Device.UpdateSubresource( sourceBox, Texture, 0, targetRegion );
                dataStream.Close();
            }

            return;
#endif
            if( images.Count != targets.Count )
            {
                throw new ArgumentException( "images and targets must be of the same length" );
            }

            var rect = Texture.Map( 0, MapMode.WriteDiscard, MapFlags.None );

            for( int i = 0; i < images.Count; ++i )
            {
                var im = images[ i ];
                var target = targets[ i ];

                for( int y = 0; y < im.Height; ++y )
                {
                    int sourceOffset = 4 * y * im.Width;
                    int sourceCount = 4 * im.Width;

                    rect.Data.Position = 4 * ( ( y + target.Origin.y ) * Width + target.Origin.x );
                    rect.Data.Write( im.Pixels, sourceOffset, sourceCount );
                }
            }

            rect.Data.Close();
            Texture.Unmap( 0 );
        }

        // TODO: if it's not ub4 or float4
        public void UpdateRectangle( Image4ub im, Rect2i source, Rect2i target )
        {
            // TODO: check boundaries

            if( source.Width != target.Width ||
                source.Height != target.Height )
            {
                throw new ArgumentException( "Mismatched rectangle sizes." );
            }

            var rect = Texture.Map( 0, MapMode.Write, MapFlags.None );

            // find the byte array offsets and the count for the rectangles            
            int sourceOffset = 4 * ( source.Origin.y * im.Width + source.Origin.x );
            int sourceCount = 4 * source.Area;
            int targetOffset = 4 * ( target.Origin.y * im.Width + target.Origin.x );

            rect.Data.Position = targetOffset;
            rect.Data.Write( im.Pixels, sourceOffset, sourceCount );
            rect.Data.Close();
            Texture.Unmap( 0 );
        }
        
        public void SavePNG( string filename )
        {
            Texture.SavePNG( filename );
        }

        private DynamicTexture2D( int width, int height, Texture2D texture )
        {
            Width = width;
            Height = height;
            Texture = texture;

            ShaderResourceView = new ShaderResourceView( D3D10Wrapper.Instance.Device, Texture );
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10
{
    public class StagingTexture2D
    {        
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Texture2D Texture { get; private set; }        

        public static StagingTexture2D CreateUnsignedByte4( Vector2i size )
        {
            return CreateUnsignedByte4( size.x, size.y );
        }

        public static StagingTexture2D CreateUnsignedByte4( int width, int height )
        {
            var st = new StagingTexture2D( width, height );
            st.Texture = TextureFactory.CreateUnsignedByte4( width, height,
                ResourceUsage.Staging, BindFlags.None,
                CpuAccessFlags.Read | CpuAccessFlags.Write );
            return st;
        }

        public static StagingTexture2D CreateFloat1( Vector2i size )
        {
            return CreateFloat1( size.x, size.y );
        }

        public static StagingTexture2D CreateFloat1( int width, int height )
        {
            var st = new StagingTexture2D( width, height );
            st.Texture = TextureFactory.CreateFloat1( width, height,
                ResourceUsage.Staging, BindFlags.None,
                CpuAccessFlags.Read | CpuAccessFlags.Write );
            return st;
        }

        public static StagingTexture2D CreateFloat2( Vector2i size )
        {
            return CreateFloat2( size.x, size.y );
        }

        public static StagingTexture2D CreateFloat2( int width, int height )
        {
            var st = new StagingTexture2D( width, height );
            st.Texture = TextureFactory.CreateFloat2( width, height,
                ResourceUsage.Staging, BindFlags.None,
                CpuAccessFlags.Read | CpuAccessFlags.Write );
            return st;
        }

        public static StagingTexture2D CreateFloat4( Vector2i size )
        {
            return CreateFloat4( size.x, size.y );
        }

        public static StagingTexture2D CreateFloat4( int width, int height )
        {
            var st = new StagingTexture2D( width, height );
            st.Texture = TextureFactory.CreateFloat4( width, height,
                ResourceUsage.Staging, BindFlags.None,
                CpuAccessFlags.Read | CpuAccessFlags.Write );
            return st;
        }

        public Vector2i Size
        {
            get
            {
                return new Vector2i( Width, Height );
            }
        }

        public void CopyFrom( Texture2D source )
        {
            var device = D3D10Wrapper.Instance.Device;
            device.CopyResource( source, Texture );
        }

        public void CopyTo( Texture2D destination )
        {
            var device = D3D10Wrapper.Instance.Device;
            device.CopyResource( Texture, destination );
        }

        /// <summary>
        /// Fills im with the data from the this texture
        /// </summary>
        /// <param name="im"></param>
        public void CopyTo( Image4ub im )
        {
            if( Texture.Description.Format != SlimDX.DXGI.Format.R8G8B8A8_UNorm )
            {
                throw new FormatException( "Cannot copy to an Image4ub unless the format is R8G8B8A8_UNorm" );
            }

            var pixels = im.Pixels;

            var rect = Texture.Map( 0, MapMode.Read, MapFlags.None );
            int pitch = rect.Pitch;
            int w = im.Width;
            int h = im.Height;

            // if it's aligned, read it all at once
            if( pitch == 4 * w )
            {
                rect.Data.Read( pixels, 0, 4 * im.NumPixels );
            }
            // read it one row at a time
            else
            {
                for( int y = 0; y < h; ++y )
                {
                    rect.Data.Seek( y * pitch, SeekOrigin.Begin );
                    rect.Data.Read( pixels, 4 * y * w, 4 * w );
                }
            }

            rect.Data.Close();
            Texture.Unmap( 0 );
        }

        /// <summary>
        /// Copies a rectangle of this texture into im, which has size rect
        /// </summary>
        /// <param name="region"></param>
        /// <param name="im"></param>
        public void CopyRectangleTo( Rect2i region, Image4ub im )
        {
            if( Texture.Description.Format != SlimDX.DXGI.Format.R8G8B8A8_UNorm )
            {
                throw new FormatException( "Cannot copy to an Image4ub unless the format is R8G8B8A8_UNorm" );
            }

            var rect = Texture.Map( 0, MapMode.Read, MapFlags.None );
            int pitch = rect.Pitch;
            if( pitch != 4 * Width )
            {
                throw new FormatException( "pitch != width" );
            }

            for( int y = 0; y < im.Height; ++y )
            {                               
                int targetOffset = 4 * y * im.Width;
                int targetCount = 4 * im.Width;

                rect.Data.Position = 4 * ( ( y + region.Origin.y ) * Width + region.Origin.x );
                rect.Data.ReadRange( im.Pixels, targetOffset, targetCount );
            }
                        
            rect.Data.Close();
            Texture.Unmap( 0 );
        }

        public void CopyTo( Image1f im )
        {
            if( Texture.Description.Format != SlimDX.DXGI.Format.R32_Float )
            {
                throw new FormatException( "Cannot copy to an Image1f unless the format is R32_Float" );
            }

            var rect = Texture.Map( 0, MapMode.Read, MapFlags.None );
            int pitch = rect.Pitch;
            if( pitch != Width * sizeof( float ) )
            {
                throw new FormatException( "pitch != width" );
            }

            rect.Data.ReadRange( im.Pixels, 0, im.NumPixels );

            rect.Data.Close();
            Texture.Unmap( 0 );
        }

        public void CopyTo( Image4f im )
        {
            if( !( Size.Equals( im.Size ) ) )
            {
                throw new ArgumentException( "Size mismatch" );
            }

            // TODO: make an image2f and 3f?            
            if( Texture.Description.Format == SlimDX.DXGI.Format.R32G32B32A32_Float )
            {
                CopyFloat4ToFloat4( im );
            }
            else if( Texture.Description.Format == SlimDX.DXGI.Format.R32G32_Float )
            {
                CopyFloat2ToFloat4( im );
            }
            else
            {
                throw new ArgumentException( "Format mismatch" );
            }
        }

        private void CopyFloat4ToFloat4( Image4f im )
        {
            var dataRect = Texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = 4 * sizeof( float ) * Width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;            

            for( int y = 0; y < Height; ++y )
            {
                stream.ReadRange( im.Pixels, 4 * y * Width, 4 * Width );

                if( nBytesToSkip > 0 )
                {
                    stream.ReadRange< byte >( nBytesToSkip );
                }
            }

            stream.Close();
            Texture.Unmap( 0 );
        }

        private void CopyFloat2ToFloat4( Image4f im )
        {
            var dataRect = Texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = 2 * sizeof( float ) * Width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;            

            for( int y = 0; y < Height; ++y )
            {
                for( int x = 0; x < Width; ++x )
                {
                    float r = stream.Read< float >();
                    float g = stream.Read< float >();
                    im[ x, y ] = new Vector4f( r, g, 0, 1 );
                }

                if( nBytesToSkip > 0 )
                {
                    stream.ReadRange< byte >( nBytesToSkip );
                }
            }

            stream.Close();
            Texture.Unmap( 0 );            
        }

        public void CopyFrom( Image4ub im )
        {
            var rect = Texture.Map( 0, MapMode.Write, MapFlags.None );
            int pitch = rect.Pitch;
            if( pitch != 4 * Width )
            {
                throw new FormatException( "pitch != width" );
            }

            var pixels = im.Pixels;
            rect.Data.Write( pixels, 0, pixels.Length );
            rect.Data.Close();
            Texture.Unmap( 0 );
        }

        public void Transfer( Image4ub from, Texture2D to )
        {
            CopyFrom( from );
            CopyTo( to );
        }

        public void Transfer( Texture2D from, Image4ub to )
        {
            CopyFrom( from );
            CopyTo( to );
        }

        public void Transfer( Texture2D from, Image4f to )
        {
            CopyFrom( from );
            CopyTo( to );
        }

        private StagingTexture2D( int width, int height )
        {
            Width = width;
            Height = height;
        }
    }
}

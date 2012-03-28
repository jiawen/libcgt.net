using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device=SlimDX.Direct3D10.Device;
using MapFlags=SlimDX.Direct3D10.MapFlags;

namespace libcgt.DirectX.D3D10
{
    public static class TextureExtensions
    {
        public static void Fill( this Texture2D texture, Image4ub im )
        {
            if( texture.Description.Format != Format.R8G8B8A8_UNorm )
            {
                throw new FormatException( "Can only fill an Image4ub with a texture of format R8G8B8A8_UNorm" );
            }

            int width = texture.Description.Width;
            int height = texture.Description.Height;
            var dataRect = texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = 4 * sizeof( byte ) * width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;

            // int k = 0;
            for( int y = 0; y < height; ++y )
            {
                stream.Read( im.Pixels, 4 * y * width, 4 * width );
                /*
                for( int x = 0; x < width; ++x )
                {                            
                    byte r = stream.Read< byte >();
                    byte g = stream.Read< byte >();
                    byte b = stream.Read< byte >();
                    byte a = stream.Read< byte >();

                    im[ x, y ] = new Vector4ub( r, g, b, a );
                    
                    ++k;
                }
                */

                if( nBytesToSkip > 0 )
                {
                    stream.ReadRange< byte >( nBytesToSkip );
                }
            }
            stream.Close();
            texture.Unmap( 0 );
        }

        public static void Fill( this Texture2D texture, Image1f im )
        {
            if( texture.Description.Format != Format.R32_Float )
            {
                throw new FormatException( "Can only fill an Image1f with a texture of format R32_Float" );
            }

            int width = texture.Description.Width;
            int height = texture.Description.Height;
            var dataRect = texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = sizeof( float ) * width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;

            for( int y = 0; y < height; ++y )
            {
                // OPTIMIZE: float[] cast to byte[]
                for( int x = 0; x < width; ++x )
                {
                    float val = stream.Read< float >();
                    im[ x, y ] = val;
                }

                if( nBytesToSkip > 0 )
                {
                    stream.ReadRange< byte >( nBytesToSkip );
                }
            }
            stream.Close();
            texture.Unmap( 0 );
        }

        public static void Fill( this Texture2D texture, Image4f im )
        {
            if( texture.Description.Format != Format.R32G32B32A32_Float )
            {
                throw new FormatException( "Can only fill an Image4f with a texture of format R32G32B32A32_Float" );
            }

            int width = texture.Description.Width;
            int height = texture.Description.Height;
            var dataRect = texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = 4 * sizeof( float ) * width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;

            for( int y = 0; y < height; ++y )
            {
                // OPTIMIZE: float[] cast to byte[]
                for( int x = 0; x < width; ++x )
                {
                    float r = stream.Read< float >();
                    float g = stream.Read< float >();
                    float b = stream.Read< float >();
                    float a = stream.Read< float >();
                    im[ x, y ] = new Vector4f( r, g, b, a );
                }

                if( nBytesToSkip > 0 )
                {
                    stream.ReadRange< byte >( nBytesToSkip );
                }
            }
            stream.Close();
            texture.Unmap( 0 );
        }

        public static void SaveTXT( this Texture2D texture, string filename )
        {
            int width = texture.Description.Width;
            int height = texture.Description.Height;

            switch( texture.Description.Format )
            {
                case Format.R8G8B8A8_UNorm:
                {
                    var st = StagingTexture2D.CreateUnsignedByte4( width, height );
                    var im = new Image4ub( width, height );
                    st.Transfer( texture, im );
                    im.SaveTXT( filename );
                    break;
                }
                case Format.R32G32_Float:
                {
                    var st = StagingTexture2D.CreateFloat2( width, height );
                    var im = new Image4f( width, height );
                    st.Transfer( texture, im );
                    im.SaveTXT( filename );
                    break;
                }
                case Format.R32G32B32A32_Float:
                {
                    var st = StagingTexture2D.CreateFloat4( width, height );
                    var im = new Image4f( width, height );
                    st.Transfer( texture, im );
                    im.SaveTXT( filename );
                    break;
                }
                default:
                {
                    throw new ArgumentException( "Format is unsupported" );
                }
            }

#if false
            var device = D3D10Wrapper.Instance.Device;

            // copy to CPU
            var ctd = texture.Description;
            ctd.Usage = ResourceUsage.Staging;
            ctd.BindFlags = BindFlags.None;
            ctd.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;

            using( var cpuTexture = new Texture2D( device, ctd ) )
            {
                device.CopyResource( texture, cpuTexture );
                
                int width = texture.Description.Width;
                int height = texture.Description.Height;
                var dataRect = cpuTexture.Map( 0, MapMode.Read, MapFlags.None );
                var stream = dataRect.Data;
                
                // TODO: this is spaghetti
                // consolidate the copying texture code
                // and the reading into a few format classes
                // and then just have a save
                using( var sw = new StreamWriter( filename ) )
                {
                    if( texture.Description.Format == Format.R32G32B32A32_Float )
                    {
                        sw.WriteLine( "float4 texture: width = {0}, height = {1}", width, height );
                        sw.WriteLine( "[index] (x,y_dx) ((x,y_gl)): r g b a" );

                        int nUsedBytesRow = 4 * sizeof( float ) * width;
                        int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;

                        int k = 0;
                        for( int y = 0; y < height; ++y )
                        {
                            int yy = height - y - 1;

                            for( int x = 0; x < width; ++x )
                            {
                                float r = stream.Read< float >();
                                float g = stream.Read< float >();
                                float b = stream.Read< float >();
                                float a = stream.Read< float >();

                                sw.WriteLine( "[{0}] ({1},{2}) (({3},{4})): {5} {6} {7} {8}",
                                              k, x, y, x, yy,
                                              r, g, b, a );
                                ++k;
                            }

                            if( nBytesToSkip > 0 )
                            {
                                stream.ReadRange< byte >( nBytesToSkip );
                            }
                        }
                    }
                    else if( texture.Description.Format == Format.R32G32_Float )
                    {
                        sw.WriteLine( "float2 texture: width = {0}, height = {1}", width, height );
                        sw.WriteLine( "[index] (x,y_dx) ((x,y_gl)): r g" );

                        int nUsedBytesRow = 2 * sizeof( float ) * width;
                        int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;

                        int k = 0;
                        for( int y = 0; y < height; ++y )
                        {
                            int yy = height - y - 1;

                            for( int x = 0; x < width; ++x )
                            {
                                float r = stream.Read< float >();                           
                                float g = stream.Read< float >();

                                sw.WriteLine( "[{0}] ({1},{2}) (({3},{4})): {5} {6}",
                                              k, x, y, x, yy,
                                              r, g );
                                ++k;
                            }

                            if( nBytesToSkip > 0 )
                            {
                                stream.ReadRange< byte >( nBytesToSkip );
                            }
                        }
                    }
                    else if( texture.Description.Format == Format.R32_Float )
                    {
                        sw.WriteLine( "float1 texture: width = {0}, height = {1}", width, height );
                        sw.WriteLine( "[index] (x,y_dx) ((x,y_gl)): r" );

                        int nUsedBytesRow = sizeof( float ) * width;
                        int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;

                        int k = 0;
                        for( int y = 0; y < height; ++y )
                        {
                            int yy = height - y - 1;

                            for( int x = 0; x < width; ++x )
                            {
                                float r = stream.Read< float >();                                

                                sw.WriteLine( "[{0}] ({1},{2}) (({3},{4})): {5}",
                                              k, x, y, x, yy,
                                              r );
                                ++k;
                            }

                            if( nBytesToSkip > 0 )
                            {
                                stream.ReadRange< byte >( nBytesToSkip );
                            }
                        }
                    }                    
                }
                stream.Close();
                cpuTexture.Unmap( 0 );
            }
#endif
        }

        public static void SavePNG( this Texture2D texture, string filename )
        {
            int width = texture.Description.Width;
            int height = texture.Description.Height;

            switch( texture.Description.Format )
            {
                case Format.R8G8B8A8_UNorm:
                {
                    var st = StagingTexture2D.CreateUnsignedByte4( width, height );
                    var im = new Image4ub( width, height );
                    st.Transfer( texture, im );
                    im.SavePNG( filename );
                    break;
                }
                case Format.R32G32_Float:
                {
                    var st = StagingTexture2D.CreateFloat2( width, height );
                    var im = new Image4f( width, height );
                    st.Transfer( texture, im );
                    im.SavePNG( filename );
                    break;
                }
                case Format.R32G32B32A32_Float:
                {
                    var st = StagingTexture2D.CreateFloat4( width, height );
                    var im = new Image4f( width, height );
                    st.Transfer( texture, im );
                    im.SavePNG( filename );
                    break;
                }
                default:
                {
                    throw new ArgumentException( "Format is unsupported" );
                }
            }
        }

#if false
    if( texture.Description.Format != Format.R8G8B8A8_UNorm &&                
                texture.Description.Format != Format.R32_Float &&
                texture.Description.Format != Format.R32G32_Float &&
                texture.Description.Format != Format.R32G32B32A32_Float )
            {
                throw new FormatException( "Can only save to PNG if the format is Format.R8G8B8A8_UNorm, Format.R32_Float, Format.R32G32_Float, or Format.R32G32B32A32_Float" );
            }            

            using( var cpuTexture = CopyToCPUTexture( texture ) )
            {
                switch( texture.Description.Format )
                {                    
                    case Format.R8G8B8A8_UNorm:
                    {
                        ReadImage4ub( cpuTexture ).SavePNG( filename );
                        break;
                    }
                    case Format.R32_Float:
                    {
                        ReadImage1f( cpuTexture ).SavePNG( filename );
                        break;
                    }
                    case Format.R32G32_Float:
                    {
                        ReadImage2f( cpuTexture ).SavePNG( filename );
                        break;
                    }
                    case Format.R32G32B32A32_Float:
                    {
                        ReadImage4f( cpuTexture ).SavePNG( filename );
                        break;
                    }                
                }                  
            }
        }
#endif

        public static void SavePFM4( this Texture2D texture, string filename )
        {
            if( texture.Description.Format != Format.R32G32B32A32_Float )
            {                
                throw new FormatException( "Can only save to PFM4 if the format is Format.R32G32B32A32_Float" );
            }

            // copy to CPU
            var ctd = texture.Description;
            ctd.Usage = ResourceUsage.Staging;
            ctd.BindFlags = BindFlags.None;
            ctd.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;

            using( var cpuTexture = CopyToCPUTexture( texture ) )
            {                
                var im = new Image4f( texture.Description.Width, texture.Description.Height );
                cpuTexture.Fill( im );
                im.SavePFM4( filename );
            }
        }

        private static Texture2D CopyToCPUTexture( Texture2D texture )
        {
            var device = D3D10Wrapper.Instance.Device;
            var ctd = texture.Description;
            ctd.Usage = ResourceUsage.Staging;
            ctd.BindFlags = BindFlags.None;
            ctd.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;

            var cpuTexture = new Texture2D( device, ctd );
            device.CopyResource( texture, cpuTexture );
            return cpuTexture;
        }

        private static Image4ub ReadImage4ub( Texture2D texture )
        {
            int width = texture.Description.Width;
            int height = texture.Description.Height;
            var dataRect = texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = 4 * sizeof( byte ) * width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;
            var im = new Image4ub( width, height );

            for( int y = 0; y < height; ++y )
            {
                stream.Read( im.Pixels, 4 * y * width, 4 * width );
                
                if( nBytesToSkip > 0 )
                {
                    stream.ReadRange< byte >( nBytesToSkip );
                }
            }

            stream.Close();
            texture.Unmap( 0 );

            return im;
        }

        private static Image1f ReadImage1f( Texture2D texture )
        {
            int width = texture.Description.Width;
            int height = texture.Description.Height;
            var dataRect = texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = sizeof( float ) * width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;
            var im = new Image1f( width, height );

            for( int y = 0; y < height; ++y )
            {
                stream.ReadRange( im.Pixels, y * width, nUsedBytesRow );

                /*
                for( int x = 0; x < width; ++x )
                {                    
                    im[ x, y ] = stream.Read< float >();
                }
                */

                if( nBytesToSkip > 0 )
                {
                    stream.ReadRange< byte >( nBytesToSkip );
                }
            }

            stream.Close();
            texture.Unmap( 0 );

            return im;
        }

        private static Image4f ReadImage2f( Texture2D texture )
        {
            int width = texture.Description.Width;
            int height = texture.Description.Height;
            var dataRect = texture.Map( 0, MapMode.Read, MapFlags.None );
            var stream = dataRect.Data;

            int nUsedBytesRow = 2 * sizeof( float ) * width;
            int nBytesToSkip = dataRect.Pitch - nUsedBytesRow;
            var im = new Image4f( width, height );

            for( int y = 0; y < height; ++y )
            {
                for( int x = 0; x < width; ++x )
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
            texture.Unmap( 0 );

            return im;
        }
    }
}

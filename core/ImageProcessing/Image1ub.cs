using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    [Serializable]
    public class Image1ub
    {
        private int width;
        private int height;
        private byte[] pixels;

        public static Image1ub FromPNG( string filename, int channel )
        {
            return Image4ub.FromFile( filename ).GetChannel( channel );
        }

        public Image1ub( int width, int height ) :
            this( width, height, 0 )
        {

        }

        public Image1ub( int width, int height, byte fill )
        {
            this.width = width;
            this.height = height;

            int nPixels = width * height;
            pixels = new byte[ nPixels ];

            Fill( fill );
        }

        public Image1ub( Vector2i size )
            : this( size.x, size.y )
        {

        }

        public Image1ub( Vector2i size, byte fill )
            : this( size.x, size.y, fill )
        {

        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public Vector2i Size
        {
            get
            {
                return new Vector2i( width, height );
            }
        }

        public int NumPixels
        {
            get
            {
                return Width * Height;
            }
        }

        public byte[] Pixels
        {
            get
            {
                return pixels;
            }
        }

        public byte this[ int k ]
        {
            get
            {
                return pixels[ k ];
            }
            set
            {
                pixels[ k ] = value;
            }
        }

        public byte this[ Vector2i xy ]
        {
            get
            {
                return this[ xy.x, xy.y ];
            }
            set
            {
                this[ xy.x, xy.y ] = value;
            }
        }        

        /// <summary>
        /// Gets/Sets the color of the pixel at (x,y)
        /// x and y are clamped before addressing
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public byte this[ int x, int y ]
        {
            get
            {
                x = x.Clamp( 0, width );
                y = y.Clamp( 0, height );
                int k = GetIndex( x, y );
                
                return pixels[ k ];
            }
            set
            {
                x = x.Clamp( 0, width );
                y = y.Clamp( 0, height );
                int k = GetIndex( x, y );
                
                pixels[ k ] = value;
            }
        }

        public byte GetPixel( int x, int y )
        {
            return this[ x, y ];
        }

        public byte GetPixelFlipped( int x, int y )
        {
            return this[ x, height - y - 1 ];
        }

        public void SetPixel( int x, int y, byte c )
        {
            this[ x, y ] = c;
        }

        public void SetPixelFlipped( int x, int y, byte c )
        {
            this[ x, height - y - 1 ] = c;
        }

        public void Fill( byte c )
        {
            for( int i = 0; i < pixels.Length; ++i )
            {
                pixels[ i ] = c;
            }
        }

        // TODO: convert to an image4ub?
        public void SavePNG( string filename )
        {
            int stride = 3 * width; // number of bytes in one scanline
            byte[] buffer = new byte[ 3 * width * height ];
            
            for( int k = 0; k < pixels.Length; ++k )
            {
                byte c = pixels[ k ];
                
                int index = 3 * k;
                buffer[ index ] = c;
                buffer[ index + 1 ] = c;
                buffer[ index + 2 ] = c;
            }

            var pixelFormat = System.Windows.Media.PixelFormats.Rgb24;
            
            var stream = new FileStream( filename, FileMode.Create );
            var image = BitmapSource.Create( width, height, 96, 96, pixelFormat, null, buffer, stride );
            
            var encoder = new PngBitmapEncoder();
            encoder.Interlace = PngInterlaceOption.Off;
            encoder.Frames.Add( BitmapFrame.Create( image ) );
            encoder.Save( stream );
            stream.Dispose();
        }

        public void SavePNG2( string filename )
        {
            int stride = width; // number of bytes in one scanline
            var pixelFormat = System.Windows.Media.PixelFormats.Gray8;
            
            var stream = new FileStream( filename, FileMode.Create );
            var image = BitmapSource.Create( width, height, 96, 96, pixelFormat, null, pixels, stride );
            
            var encoder = new PngBitmapEncoder();
            encoder.Interlace = PngInterlaceOption.Off;
            encoder.Frames.Add( BitmapFrame.Create( image ) );
            encoder.Save( stream );
            stream.Dispose();
        }

        /*
        public void SavePFM( string filename )
        {
            int stride = 4 * width; // number of bytes in one scanline
            byte[] buffer = new byte[ 4 * width * height ];
            
            for( int k = 0; k < pixels.Length; ++k )
            {
                Color c = pixels[ k ];
                
                int index = 4 * k;
                buffer[ index ] = c.B;
                buffer[ index + 1 ] = c.G;
                buffer[ index + 2 ] = c.R;
                buffer[ index + 3 ] = c.A;
            }

            System.Windows.Media.PixelFormat pixelFormat = System.Windows.Media.PixelFormats.Bgra32;
            
            FileStream stream = new FileStream( filename, FileMode.Create );
            BitmapSource image = BitmapSource.Create( width, height, 96, 96, pixelFormat, null, buffer, stride );
            
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Interlace = PngInterlaceOption.Off;
            encoder.Frames.Add( BitmapFrame.Create( image ) );
            encoder.Save( stream );
            stream.Dispose();
        }
        */

        private int GetIndex( int x, int y )
        {
            return y * width + x;
        }
    }
}

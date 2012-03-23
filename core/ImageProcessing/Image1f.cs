using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    [Serializable]
    public class Image1f : IImage< float >
    {
        private int width;
        private int height;
        private float[] pixels;

        /// <summary>
        /// Constructs an Image1f from a standard grayscale PFM file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Image1f FromPFM( string filename )
        {
            // read header
            int width;
            int height;
            int offset;

            using( var sr = new StreamReader( filename ) )
            {
                string header = sr.ReadLine();
                if( header != "Pf" )
                {
                    throw new DataException( "Grayscale PFM files must begin with \"Pf\"" );
                }

                string dimensions = sr.ReadLine();
                string[] tokens = dimensions.Split( ' ' );
                width = int.Parse( tokens[ 0 ] );
                height = int.Parse( tokens[ 1 ] );

                string scale = sr.ReadLine();
                float fScale = float.Parse( scale );
                if( !( fScale < 0 ) )
                {
                    throw new DataException( "Big-endian PFM files are not supported." );
                }

                offset = ( 1 + header.Length ) + ( 1 + dimensions.Length ) + ( 1 + scale.Length );
            }

            FileStream stream = new FileStream( filename, FileMode.Open, FileAccess.Read );
            BinaryReader br = new BinaryReader( stream );
            stream.Seek( offset, SeekOrigin.Begin );

            Image1f im = new Image1f( width, height );
            for( int k = 0; k < im.NumPixels; ++k )
            {
                im[ k ] = br.ReadSingle();
            }

            br.Close();
            return im;
        }

        public Image1f( int width, int height ) :
            this( width, height, 0.0f )
        {

        }

        public Image1f( int width, int height, float fill )
        {
            this.width = width;
            this.height = height;

            int nPixels = width * height;
            pixels = new float[ nPixels ];

            Fill( fill );
        }

        public Image1f( Vector2i size )
            : this( size.x, size.y )
        {

        }

        public Image1f( Vector2i size, float fill )
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

        public float[] Pixels
        {
            get
            {
                return pixels;
            }
        }

        public float this[ int k ]
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

        public float this[ Vector2i xy ]
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
        public float this[ int x, int y ]
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

        public float BilinearSample( Vector2f xy )
        {
            return BilinearSample( xy.x, xy.y );
        }

        public float BilinearSample( float x, float y )
        {
            // TODO: check this, might not be exactly up to DX spec wrt edges
            // clamp to edge
            x = x.Clamp( 0, width );
            y = y.Clamp( 0, height );

            int x0 = x.FloorToInt().Clamp( 0, width );
            int x1 = ( x0 + 1 ).Clamp( 0, width );
            int y0 = y.FloorToInt().Clamp( 0, height );
            int y1 = ( y0 + 1 ).Clamp( 0, height );

            float xf = x - x0;
            float yf = y - y0;

            float v00 = this[ x0, y0 ];
            float v01 = this[ x0, y1 ];
            float v10 = this[ x1, y0 ];
            float v11 = this[ x1, y1 ];

            var v0i = Arithmetic.Lerp( v00, v01, yf ); // x = 0
            var v1i = Arithmetic.Lerp( v10, v11, yf ); // x = 1

            var vi = Arithmetic.Lerp( v0i, v1i, xf );
            return vi;
        }

        public void Fill( float c )
        {
            for( int i = 0; i < NumPixels; ++i )
            {
                pixels[ i ] = c;
            }
        }

        /// <summary>
        /// Save into a grayscale PFM float map.
        /// </summary>
        /// <param name="filename"></param>
        public void SavePFM( string filename )
        {
            // write header
            using( StreamWriter sw = new StreamWriter( filename ) )
            {
                sw.NewLine = "\n";
                sw.WriteLine( "Pf" );
                sw.WriteLine( "{0} {1}", width, height );
                sw.WriteLine( "-1" );
            }

            // write binary data to the end of stream
            FileStream stream = new FileStream( filename, FileMode.Append, FileAccess.Write );
            BinaryWriter bw = new BinaryWriter( stream );

            for( int k = 0; k < NumPixels; ++k )
            {
                bw.Write( pixels[ k ] );
            }

            bw.Close();
        }

        public void SavePNG( string filename )
        {
            int stride = 3 * width; // number of bytes in one scanline
            byte[] buffer = new byte[ 3 * width * height ];
            
            for( int k = 0; k < pixels.Length; ++k )
            {
                byte c = FormatConversion.ToUnsignedByte( pixels[ k ] );
                
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

        public void Preview()
        {
            var temp = System.IO.Path.GetTempPath();
            var filename = temp + "libcgt_debug.png";
            SavePNG( filename );
            Process.Start( filename );
        }

        private int GetIndex( int x, int y )
        {
            return y * width + x;
        }
    }
}

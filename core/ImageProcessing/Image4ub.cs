using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    /// <summary>
    /// 4-channel (RGBA) image with 32 bits per pixel (8 bits per channel)
    /// Stored in a *left handed* coordinate system.  Which means:
    /// - pixel (0,0) is the top left
    /// - address 0 is the top left
    /// </summary>
    
    [Serializable]
    public class Image4ub : IImage< Vector4ub >
    {        
        private int width;
        private int height;
        private byte[] pixels;

        public static Image4ub FromFile( string filename )
        {
            var bi = new BitmapImage( new Uri( filename ) );            
            var im = new Image4ub( bi.PixelWidth, bi.PixelHeight );
            bi.CopyPixels( im.pixels, 4 * bi.PixelWidth, 0 );

            // swizzle BGRA --> RGBA
            int nPixels = im.NumPixels;
            for( int k = 0; k < nPixels; ++k )
            {
                byte r = im.pixels[ 4 * k + 2 ];
                byte b = im.pixels[ 4 * k ];

                im.pixels[ 4 * k ] = r;
                im.pixels[ 4 * k + 2 ] = b;
            }

            return im;
        }

        /// <summary>
        /// Creates a new completely black image.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Image4ub( int width, int height )            
        {
            this.width = width;
            this.height = height;

            int nPixels = width * height;
            pixels = new byte[ 4 * nPixels ];
        }

        public Image4ub( int width, int height, Vector4ub fillColor )
        {
            this.width = width;
            this.height = height;

            int nPixels = width * height;
            pixels = new byte[ 4 * nPixels ];

            Fill( fillColor );
        }

        public Image4ub( Vector2i size )
            : this( size.x, size.y )
        {

        }

        public Image4ub( Vector2i size, Vector4ub fillColor )
            : this( size.x, size.y, fillColor )
        {

        }

        public Image4ub( Image1ub red, Image1ub green, Image1ub blue ) :
            this( red.Width, red.Height )
        {
            for( int k = 0; k < red.NumPixels; ++k )
            {
                this[ k ] = new Vector4ub( red[ k ], green[ k ], blue[ k ], 255 );
            }
        }

        public Image4ub( Image4ub im )
            : this( im.Size )
        {
            im.pixels.CopyTo( pixels, 0 );
        }

        public Image4ub( Image4f im )
            : this( im.Size )
        {
            for( int k = 0; k < im.NumPixels; ++k )
            {
                this[ k ] = im[ k ].ToUnsignedByte();
            }
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

        public Vector4ub this[ int k ]
        {
            get
            {
                int index = 4 * k;
                return new Vector4ub( pixels[ index ], pixels[ index + 1 ], pixels[ index + 2 ], pixels[ index + 3 ] );
            }
            set
            {
                int index = 4 * k;
                pixels[ index ] = value.x;
                pixels[ index + 1 ] = value.y;
                pixels[ index + 2 ] = value.z;
                pixels[ index + 3 ] = value.w;
            }
        }

        public Vector4ub this[ Vector2i xy ]
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

        public void Fill( Vector4ub c )
        {
            for( int i = 0; i < width * height; ++i )
            {
                pixels[ 4 * i ] = c.x;
                pixels[ 4 * i + 1 ] = c.y;
                pixels[ 4 * i + 2 ] = c.z;
                pixels[ 4 * i + 3 ] = c.w;
            }
        }

        public void Fill( Image4ub im )
        {
            if( !( this.Size.Equals( im.Size ) ) )
            {
                throw new ArgumentException( "Cannot fill an image with another that's not the same size" );
            }

            im.pixels.CopyTo( pixels, 0 );
        }
        
        /// <summary>
        /// Fills entire target with a rectangle from the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceRect"></param>
        /// <param name="target"></param>
        public static void FillFromSubImage( Image4ub source, Rect2i sourceRect,
            Image4ub target )
        {
            var targetRect = new Rect2i( 0, 0, target.width, target.height );
            FillSubImage( source, sourceRect, target, targetRect );
        }

        public static void FillSubImage( Image4ub source, Rect2i sourceRect,
            Image4ub target, Rect2i targetRect )
        {
            // TODO: check bounds
            // TODO: check rectangle equal sizes
            // TODO: image.containsRect()            
            // TODO: optimize: copy a number of rows

            int sourceX0 = sourceRect.Origin.x;
            int sourceY0 = sourceRect.Origin.y;
            int targetX0 = targetRect.Origin.x;
            int targetY0 = targetRect.Origin.y;

            for( int y = 0; y < sourceRect.Height; ++y )
            {
                for( int x = 0; x < sourceRect.Width; ++x )
                {
                    target[ targetX0 + x, targetY0 + y ] = source[ sourceX0 + x, sourceY0 + y ];
                }
            }
        }

        /// <summary>
        /// Gets/Sets the color of the pixel at (x,y)
        /// x and y are clamped before addressing
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector4ub this[ int x, int y ]
        {
            get
            {
                x = x.Clamp( 0, width );
                y = y.Clamp( 0, height );
                int k = GetIndex( x, y );
                
                return this[ k ];
            }
            set
            {
                x = x.Clamp( 0, width );
                y = y.Clamp( 0, height );
                int k = GetIndex( x, y );
                
                this[ k ] = value;
            }
        }

        public Vector4ub GetPixel( int x, int y )
        {
            return this[ x, y ];
        }

        public Vector4f BilinearSampleToFloat( Vector2f xy )
        {
            return BilinearSampleToFloat( xy.x, xy.y );
        }

        public Vector4f BilinearSampleToFloat( float x, float y )
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

            var v00 = this[ x0, y0 ].ToFloat();
            var v01 = this[ x0, y1 ].ToFloat();
            var v10 = this[ x1, y0 ].ToFloat();
            var v11 = this[ x1, y1 ].ToFloat();

            var v0i = Vector4f.Lerp( v00, v01, yf ); // x = 0
            var v1i = Vector4f.Lerp( v10, v11, yf ); // x = 1

            var vi = Vector4f.Lerp( v0i, v1i, xf );
            return vi;
        }

        /// <summary>
        /// *Samples* this image at floating point coordinates x and y
        /// The coordinate system is defined such that pixel centers are at *half-integer*
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector4ub BilinearSample( float x, float y )
        {
            // TODO: check this, might not be exactly up to DX spec wrt edges
            // clamp to edge
            return BilinearSampleToFloat( x, y ).ToUnsignedByte();
        }

        public Vector4ub BilinearSample( Vector2f xy )
        {
            return BilinearSample( xy.x, xy.y );
        }

        public Vector4ub GetPixelFlipped( int x, int y )
        {
            return this[ x, height - y - 1 ];
        }

        public void SetPixel( int x, int y, Vector4ub c )
        {
            this[ x, y ] = c;
        }

        public void SetPixelFlipped( int x, int y, Vector4ub c )
        {
            this[ x, height - y - 1 ] = c;
        }

        public Image1ub GetChannel( int channel )
        {
            if( channel < 0 || channel > 3 )
            {
                throw new ArgumentException( "channel must be 0, 1, 2, or 3" );
            }

            var im = new Image1ub( this.Size );

            if( channel == 0 )
            {
                for( int k = 0; k < NumPixels; ++k )
                {
                    im[ k ] = this[ k ].x;
                }
            }
            else if( channel == 1 )
            {
                for( int k = 0; k < NumPixels; ++k )
                {
                    im[ k ] = this[ k ].y;
                }
            }
            else if( channel == 2 )
            {
                for( int k = 0; k < NumPixels; ++k )
                {
                    im[ k ] = this[ k ].z;
                }
            }
            else // channel = 3
            {
                for( int k = 0; k < NumPixels; ++k )
                {
                    im[ k ] = this[ k ].w;
                }
            }

            return im;
        }
        
        public Image4ub FlipUD()
        {
            var output = new Image4ub( width, height );
            
            for( int y = 0; y < height; ++y )
            {
                int yy = height - y - 1;
                for( int x = 0; x < width; ++x )
                {
                    output[ x, yy ] = this[ x, y ];
                }
            }

            return output;
        }

        /// <summary>
        /// Creates a new WPF BitmapSource object out of this image so it can be displayed
        /// </summary>
        /// <returns></returns>
        public BitmapSource ToWPFBitmapSource()
        {
            int stride = 4 * width; // number of bytes in one scanline
            byte[] buffer = new byte[ 4 * NumPixels ];
            
            for( int k = 0; k < NumPixels; ++k )
            {
                int index = 4 * k;
                buffer[ index ] = pixels[ index + 2 ]; // b
                buffer[ index + 1 ] = pixels[ index + 1 ]; // g
                buffer[ index + 2 ] = pixels[ index ]; // r
                buffer[ index + 3 ] = pixels[ index + 3 ]; // a
            }

            var pixelFormat = System.Windows.Media.PixelFormats.Bgra32;
            
            return BitmapSource.Create( width, height, 96, 96, pixelFormat, null, buffer, stride );            
        }

        public void SavePNG( string filename )
        {
            var src = ToWPFBitmapSource();
            
            var stream = new FileStream( filename, FileMode.Create );
            var encoder = new PngBitmapEncoder();
            encoder.Interlace = PngInterlaceOption.Off;
            encoder.Frames.Add( BitmapFrame.Create( src ) );
            encoder.Save( stream );
            stream.Dispose();
        }

        public void SaveTXT( string filename )
        {
            using( var sw = new StreamWriter( filename ) )
            {
                sw.WriteLine( "unsigned_byte_4 image: width = {0}, height = {1}", width, height );
                sw.WriteLine( "[index] (x,y_dx) ((x,y_gl)): r g b a" );

                int k = 0;
                for( int y = 0; y < height; ++y )
                {
                    int yy = height - y - 1;

                    for( int x = 0; x < width; ++x )
                    {
                        byte r = pixels[ 4 * k ];
                        byte g = pixels[ 4 * k + 1 ];
                        byte b = pixels[ 4 * k + 2 ];
                        byte a = pixels[ 4 * k + 3 ];

                        sw.WriteLine( "[{0}] ({1},{2}) (({3},{4})): {5} {6} {7} {8}",
                                      k, x, y, x, yy,
                                      r, g, b, a );
                        ++k;
                    }
                }
            }
        }

        // TODO: IImage, or AbstractImage with SavePNG as an option
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

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
    public class Image4f : IImage< Vector4f >
    {
        private int width;
        private int height;
        private float[] pixels;

        /// <summary>
        /// Constructs an Image4f from a standard 3-channel PFM file, with alpha provided by the user.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Image4f FromPFM( string filename, float alpha )
        {
            // read header
            int width;
            int height;
            int offset;

            using( var sr = new StreamReader( filename ) )
            {
                string header = sr.ReadLine();
                if( header != "PF" )
                {
                    throw new DataException( "Color PFM files must begin with \"PF\"" );
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

            var stream = new FileStream( filename, FileMode.Open, FileAccess.Read );
            var br = new BinaryReader( stream );
            stream.Seek( offset, SeekOrigin.Begin );

            var im = new Image4f( width, height );
            for( int k = 0; k < im.NumPixels; ++k )
            {
                float r = br.ReadSingle();
                float g = br.ReadSingle();
                float b = br.ReadSingle();

                im[ k ] = new Vector4f( r, g, b, alpha );
            }

            br.Close();
            return im;
        }

        public static Image4f FromPFM4( string filename )
        {
            // read header
            int width;
            int height;
            int offset;

            using( var sr = new StreamReader( filename ) )
            {
                string header = sr.ReadLine();
                if( header != "PF4" )
                {
                    throw new DataException( "PFM4 files must begin with \"PF4\"" );
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

            var stream = new FileStream( filename, FileMode.Open, FileAccess.Read );
            var br = new BinaryReader( stream );
            stream.Seek( offset, SeekOrigin.Begin );

            var im = new Image4f( width, height );
            for( int i = 0; i < im.pixels.Length; ++i )
            {
                im.pixels[ i ] = br.ReadSingle();
            }
            br.Close();
            return im;
        }

        /// <summary>
        /// Loads an Image4f from a PNG, BMP, or JPEG file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Image4f FromFile( string filename )
        {
            var bi = new BitmapImage( new Uri( filename ) );
            var im = new Image4f( bi.PixelWidth, bi.PixelHeight );
            int nPixels = bi.PixelWidth * bi.PixelHeight;
            var buffer = new byte[ 4 * nPixels ];
            bi.CopyPixels( buffer, 4 * bi.PixelWidth, 0 );

            // swizzle BGRA --> RGBA and convert to float            
            for( int k = 0; k < nPixels; ++k )
            {
                byte r = buffer[ 4 * k + 2 ];
                byte g = buffer[ 4 * k + 1 ];
                byte b = buffer[ 4 * k ];
                byte a = buffer[ 4 * k + 3 ];

                im.pixels[ 4 * k ] = FormatConversion.ToFloat( r );
                im.pixels[ 4 * k + 1 ] = FormatConversion.ToFloat( g );
                im.pixels[ 4 * k + 2 ] = FormatConversion.ToFloat( b );
                im.pixels[ 4 * k + 3 ] = FormatConversion.ToFloat( a );
            }

            return im;
        }
        
        public Image4f( int width, int height ) :
            this( width, height, Vector4f.Zero )
        {

        }

        public Image4f( int width, int height, Vector4f fill )
        {
            this.width = width;
            this.height = height;

            int nPixels = width * height;
            pixels = new float[ 4 * nPixels ];

            Fill( fill );
        }

        public Image4f( Vector2i size )
            : this( size.x, size.y )
        {

        }

        public Image4f( Vector2i size, Vector4f fill )
            : this( size.x, size.y, fill )
        {

        }

        /// <summary>
        /// Constructs an Image4f from component Image1f images with alpha = 1.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public Image4f( Image1f red, Image1f green, Image1f blue ) :
            this( red.Width, red.Height )
        {
            for( int k = 0; k < red.NumPixels; ++k )
            {
                this[ k ] = new Vector4f( red[ k ], green[ k ], blue[ k ], 1.0f );
            }
        }

        public Image4f( Image1f red, Image1f green, Image1f blue, Image1f alpha )
            : this( red.Size )
        {
            for( int k = 0; k < red.NumPixels; ++k )
            {
                this[ k ] = new Vector4f( red[ k ], green[ k ], blue[ k ], alpha[ k ] );
            }
        }

        public Image4f( Image4f im )
            : this( im.Size )
        {
            im.pixels.CopyTo( pixels, 0 );
        }

        public Image4f( Image4ub im )
            : this( im.Size )
        {
            for( int k = 0; k < im.NumPixels; ++k )
            {
                this[ k ] = im[ k ].ToFloat();
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

        public float[] Pixels
        {
            get
            {
                return pixels;
            }
        }

        public Vector4f this[ int k ]
        {
            get
            {
                int i = 4 * k;
                return new Vector4f( pixels[ i ], pixels[ i + 1 ], pixels[ i + 2 ], pixels[ i + 3 ] );
            }
            set
            {
                int i = 4 * k;
                pixels[ i ] = value.x;
                pixels[ i + 1 ] = value.y;
                pixels[ i + 2 ] = value.z;
                pixels[ i + 3 ] = value.w;
            }
        }

        public Vector4f this[ Vector2i xy ]
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

        public Vector4f BilinearSample( Vector2f xy )
        {
            return BilinearSample( xy.x, xy.y );
        }

        public Vector4f BilinearSample( float x, float y )
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

            var v00 = this[ x0, y0 ];
            var v01 = this[ x0, y1 ];
            var v10 = this[ x1, y0 ];
            var v11 = this[ x1, y1 ];

            var v0i = Vector4f.Lerp( v00, v01, yf ); // x = 0
            var v1i = Vector4f.Lerp( v10, v11, yf ); // x = 1

            var vi = Vector4f.Lerp( v0i, v1i, xf );
            return vi;
        }

        public Vector4f BilinearSampleNormalized( float x, float y )
        {
            return BilinearSample( x * width, y * height );
        }

        public Vector4f BilinearSampleNormalized( Vector2f xy )
        {
            return BilinearSampleNormalized( xy.x, xy.y );
        }

        public Vector4f NearestNeighborSample( float x, float y )
        {
            int ix = ( x - 0.5f ).RoundToInt();
            int iy = ( y - 0.5f ).RoundToInt();

            ix = ix.Clamp( 0, width );
            iy = iy.Clamp( 0, height );

            return this[ ix, iy ];
        }

        public Vector4f NearestNeighborSample( Vector2f xy )
        {
            return NearestNeighborSample( xy.x, xy.y );
        }

        public Vector4f NearestNeighborSampleNormalized( float x, float y )
        {
            return NearestNeighborSample( x * width, y * height );
        }

        public Vector4f NearestNeighborSampleNormalized( Vector2f xy )
        {
            return NearestNeighborSampleNormalized( xy.x, xy.y );
        }

        public void Fill( Vector4f c )
        {
            for( int i = 0; i < NumPixels; ++i )
            {
                this[ i ] = c;
            }            
        }

        public void Fill( Image1f im, int channel )
        {
            if( channel < 0 || channel > 3 )
            {
                throw new ArgumentException( "Channel must be 0, 1, 2, or 3." );
            }

            for( int i = 0; i < NumPixels; ++i )
            {
                pixels[ 4 * i + channel ] = im[ i ];
            }
        }

        /// <summary>
        /// Gets/Sets the color of the pixel at (x,y)
        /// x and y are clamped before addressing
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector4f this[ int x, int y ]
        {
            get
            {
                x = x.Clamp( 0, width );
                y = y.Clamp( 0, height );
                int k = GetIndex( x, y ); // TODO: make GetIndex return 4 * ( y + width + x )
                
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

        public Image1f GetChannel( int channel )
        {
            if( channel < 0 || channel > 3 )
            {
                throw new ArgumentException( "channel must be 0, 1, 2, or 3" );
            }

            var im = new Image1f( this.Size );

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

        public void FlipUD()
        {
            for( int y = 0; y < height / 2; ++y )
            {
                int yy = height - y - 1;
                for( int x = 0; x < width; ++x )
                {
                    var p0 = this[ x, y ];
                    var p1 = this[ x, yy ];

                    this[ x, yy ] = p0;
                    this[ x, y ] = p1;
                }
            }
        }

        public Image4f FlippedUD()
        {
            var output = new Image4f( width, height );

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
        /// Creates a new WPF BitmapSource object out of this image so it can be displayed.
        /// </summary>
        /// <returns></returns>
        public BitmapSource ToWPFBitmapSource()
        {
            int stride = 4 * width * sizeof( float ); // number of bytes in one scanline
            var pixelFormat = System.Windows.Media.PixelFormats.Rgba128Float;
            return BitmapSource.Create( width, height, 96, 96, pixelFormat, null, pixels, stride );
        }

        /// <summary>
        /// Converts this floating point image to 8-bits first (clamping to [0,1] then * 255)
        /// and then saves out to PNG
        /// </summary>
        /// <param name="filename"></param>
        public void SavePNG( string filename )
        {
            ( new Image4ub( this ) ).SavePNG( filename );
        }

        public void SaveTXT( string filename )
        {
            using( var sw = new StreamWriter( filename ) )
            {
                sw.WriteLine( "float4 image: width = {0}, height = {1}", width, height );
                sw.WriteLine( "[index] (x,y_dx) ((x,y_gl)): r g b a" );

                int k = 0;
                for( int y = 0; y < height; ++y )
                {
                    int yy = height - y - 1;

                    for( int x = 0; x < width; ++x )
                    {
                        float r = pixels[ 4 * k ];
                        float g = pixels[ 4 * k + 1 ];
                        float b = pixels[ 4 * k + 2 ];
                        float a = pixels[ 4 * k + 3 ];

                        sw.WriteLine( "[{0}] ({1},{2}) (({3},{4})): {5} {6} {7} {8}",
                                      k, x, y, x, yy,
                                      r, g, b, a );
                        ++k;
                    }
                }
            }
        }

        /// <summary>
        /// Saves only the RGB components into a PFM float map.
        /// </summary>
        /// <param name="filename"></param>
        public void SavePFM( string filename )
        {
            // write header
            using( var sw = new StreamWriter( filename ) )
            {
                sw.NewLine = "\n";
                sw.WriteLine( "PF" );
                sw.WriteLine( "{0} {1}", width, height );
                sw.WriteLine( "-1" );
            }

            // write binary data to the end of stream
            var stream = new FileStream( filename, FileMode.Append, FileAccess.Write );
            var bw = new BinaryWriter( stream );

            for( int k = 0; k < NumPixels; ++k )
            {
                Vector4f p = this[ k ];
                bw.Write( p.x );
                bw.Write( p.y );
                bw.Write( p.z );
            }

            bw.Close();
        }

        /// <summary>
        /// Saves the RGBA components into a non-standard PFM4 float map.
        /// </summary>
        /// <param name="filename"></param>
        public void SavePFM4( string filename )
        {
            // write header
            using( var sw = new StreamWriter( filename ) )
            {
                sw.NewLine = "\n";
                sw.WriteLine( "PF4" );
                sw.WriteLine( "{0} {1}", width, height );
                sw.WriteLine( "-1" );
            }

            // write binary data to the end of stream
            var stream = new FileStream( filename, FileMode.Append, FileAccess.Write );
            var bw = new BinaryWriter( stream );
            
            for( int k = 0; k < pixels.Length; ++k )
            {
                bw.Write( pixels[ k ] );
            }
            bw.Close();
        }

        // TODO: IImage
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{    
    public class SparseImageFloat
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int NumComponents { get; set; }
        private Dictionary< Vector2i, float[] > values;

        public SparseImageFloat( Vector2i size, int nComponents )
            : this( size.x, size.y, nComponents )
        {
            
        }

        public SparseImageFloat( int width, int height, int nComponents )
        {
            Width = width;
            Height = height;
            NumComponents = nComponents;

            values = new Dictionary< Vector2i, float[] >();
        }

        /// <summary>
        /// Deep copies
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float[] this[ int x, int y ]
        {
            get
            {
                float[] output = new float[ NumComponents ];
                Array.Copy( values[ new Vector2i( x, y ) ], output, NumComponents );
                return output;
            }
            set
            {
                float[] arr = new float[ NumComponents ];
                Array.Copy( value, arr, NumComponents );
                values[ new Vector2i( x, y ) ] = arr;
            }
        }

        /*
        public static SparseImageFloat FromFile( string filename )
        {
            
        }
        */

        public void SaveSparsePFM( string filename )
        {
            // write header
            using( var sw = new StreamWriter( filename ) )
            {
                sw.NewLine = "\n";
                sw.WriteLine( "SparsePFM" );
                sw.WriteLine( "{0} {1} {2} {3}", Width, Height, NumComponents, values.Count );
                sw.WriteLine( "LittleEndian" );
            }

            // write binary data to the end of stream
            var stream = new FileStream( filename, FileMode.Append, FileAccess.Write );
            var bw = new BinaryWriter( stream );            

            var buffer = new byte[ ( 2 + NumComponents ) * sizeof( float ) * values.Count ];
            int offset = 0;
            foreach( var kvp in values )
            {
                int x = kvp.Key.x;
                int y = kvp.Key.y;

                var currentByteArray = BitConverter.GetBytes( x );
                Array.Copy( currentByteArray, 0, buffer, offset, currentByteArray.Length );
                offset += currentByteArray.Length;

                currentByteArray = BitConverter.GetBytes( y );
                Array.Copy( currentByteArray, 0, buffer, offset, currentByteArray.Length );
                offset += currentByteArray.Length;

                foreach( var v in kvp.Value )
                {
                    currentByteArray = BitConverter.GetBytes( v );
                    Array.Copy( currentByteArray, 0, buffer, offset, currentByteArray.Length );
                    offset += currentByteArray.Length;
                }
            }
            
            bw.Write( buffer );
            bw.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    /// <summary>
    /// Converts between bytes and unsigned normalized floats.
    /// </summary>
    public static class FormatConversion
    {
        /// <summary>
        /// Converts a byte into a float with a guarantee that byte 0 --> 0.0f and byte 255 --> 1.0f
        /// Such that == with literal constants will work.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float ToFloat( byte b )
        {
            return b / 255.0f;
        }        

        public static Vector3f ToFloat( this Vector3i color )
        {
            float r = ToFloat( ( byte ) color.x );
            float g = ToFloat( ( byte ) color.y );
            float b = ToFloat( ( byte ) color.z );

            return new Vector3f( r, g, b );
        }

        public static Vector4f ToFloat( this Vector4ub color )
        {
            float r = ToFloat( color.x );
            float g = ToFloat( color.y );
            float b = ToFloat( color.z );
            float a = ToFloat( color.w );

            return new Vector4f( r, g, b, a );
        }

        /// <summary>
        /// Converts a float color into a byte with saturation to [0,1]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static byte ToUnsignedByte( this float f )
        {
            return ( byte )( 255 * ( f.Saturate() ) );
        }

        /// <summary>
        /// Converts a float color into a byte with saturation to [0,1]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Vector4ub ToUnsignedByte( this Vector4f v )
        {
            byte r = ToUnsignedByte( v.x );
            byte g = ToUnsignedByte( v.y );
            byte b = ToUnsignedByte( v.z );
            byte a = ToUnsignedByte( v.w );

            return new Vector4ub( r, g, b, a );
        }        

        public static Image1ub ToUnsignedByte( this Image1f im )
        {
            var output = new Image1ub( im.Size );
            for( int k = 0; k < im.NumPixels; ++k )
            {
                output[ k ] = im[ k ].ToUnsignedByte();
            }
            return output;
        }

        public static Image4ub ToUnsignedByte( this Image4f im )
        {
            var output = new Image4ub( im.Size );
            for( int k = 0; k < im.NumPixels; ++k )
            {
                output[ k ] = im[ k ].ToUnsignedByte();
            }
            return output;
        }

        /// <summary>
        /// In-place format conversion between two images of the same size.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void ToUnsignedByte( Image4f input, Image4ub output )
        {            
            for( int k = 0; k < input.NumPixels; ++k )
            {
                output[ k ] = input[ k ].ToUnsignedByte();
            }            
        }
    }
}

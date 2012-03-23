using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing.ToneMapping
{
    public static class ShiftAndScale
    {
        /// <summary>
        /// TODO: C# 4.0: default to 0,0,0,0 and 1,1,1,1
        /// 
        /// intensities between blackPoint and whitePoint are linearly scaled between 0 and 1.
        /// intensities < blackPoint are clamped to 0, intensities > whitePoint are clamped to 1.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="blackPoint"></param>
        /// <param name="whitePoint"></param>
        public static void Apply( Image4f input, Image4ub output,
            Vector4f blackPoint, Vector4f whitePoint )
        {
            if( input.Size != output.Size )
            {
                throw new ArgumentException( "input and output must be of the same size" );
            }
            var range = whitePoint - blackPoint;

            for( int k = 0; k < input.NumPixels; ++k )
            {
                Vector4f p = input[ k ];
                Vector4f q = ( p - blackPoint ) / range;
                Vector4ub r = q.ToUnsignedByte();

                output[ k ] = r;
            }
        }

        public static void Apply( Image1f input, Image1ub output )
        {
            if( input.Size != output.Size )
            {
                throw new ArgumentException( "input and output must be of the same size" );
            }

            float min;
            float max;
            input.Pixels.MinMax( f => f, out min, out max );
            float reciprocalRange = 1.0f / ( max - min );

            for( int k = 0; k < input.NumPixels; ++k )
            {
                float vIn = input[ k ];
                float vOut = ( vIn - min ) * reciprocalRange;

                output[ k ] = FormatConversion.ToUnsignedByte( vOut );
            }
        }

        public static void Apply( Image1f input, Image1f output )
        {
            if( input.Size != output.Size )
            {
                throw new ArgumentException( "input and output must be of the same size" );
            }

            float min;
            float max;
            input.Pixels.MinMax( f => f, out min, out max );
            float reciprocalRange = 1.0f / ( max - min );

            for( int k = 0; k < input.NumPixels; ++k )
            {
                float vIn = input[ k ];
                float vOut = ( vIn - min ) * reciprocalRange;

                output[ k ] = vOut;
            }
        }
    }
}

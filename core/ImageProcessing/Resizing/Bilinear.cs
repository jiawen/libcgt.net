using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.ImageProcessing.Resizing
{
    public static class Bilinear
    {
        public static void Apply( Image1f input, Image1f output )
        {
            int w_out = output.Width;
            int h_out = output.Height;

            int w_in = input.Width;
            int h_in = input.Height;

            // scaling coefficients
            float x_outToIn = Arithmetic.DivideIntsToFloat( w_in, w_out );
            float y_outToIn = Arithmetic.DivideIntsToFloat( h_in, h_out );

            for( int yOut = 0; yOut < h_out; ++yOut )
            {
                float yInFloat = ( yOut + 0.5f ) * y_outToIn;

                for( int xOut = 0; xOut < w_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;

                    output[ xOut, yOut ] = input.BilinearSample( xInFloat, yInFloat );
                }
            }
        }

        public static void Apply( Image4f input, Image4f output )
        {
            int w_out = output.Width;
            int h_out = output.Height;

            int w_in = input.Width;
            int h_in = input.Height;

            // scaling coefficients
            float x_outToIn = Arithmetic.DivideIntsToFloat( w_in, w_out );
            float y_outToIn = Arithmetic.DivideIntsToFloat( h_in, h_out );

            for( int yOut = 0; yOut < h_out; ++yOut )
            {
                float yInFloat = ( yOut + 0.5f ) * y_outToIn;

                for( int xOut = 0; xOut < w_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;

                    output[ xOut, yOut ] = input.BilinearSample( xInFloat, yInFloat );
                }
            }
        }

        public static void Apply( Image4ub input, Image4ub output )
        {
            int w_out = output.Width;
            int h_out = output.Height;

            int w_in = input.Width;
            int h_in = input.Height;

            // scaling coefficients
            float x_outToIn = Arithmetic.DivideIntsToFloat( w_in, w_out );
            float y_outToIn = Arithmetic.DivideIntsToFloat( h_in, h_out );

            for( int yOut = 0; yOut < h_out; ++yOut )
            {
                float yInFloat = ( yOut + 0.5f ) * y_outToIn;

                for( int xOut = 0; xOut < w_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;

                    output[ xOut, yOut ] = input.BilinearSample( xInFloat, yInFloat );
                }
            }
        }
    }
}

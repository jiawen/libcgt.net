using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.ImageProcessing.Resizing
{
    public static class NearestNeighbor
    {
        public static void Apply( Image4f input, Image4f output )
        {
            int w_out = output.Width;
            int h_out = output.Height;

            int w_in = input.Width;
            int h_in = input.Height;

            float x_outToIn = Arithmetic.DivideIntsToFloat( w_in, w_out );
            float y_outToIn = Arithmetic.DivideIntsToFloat( h_in, h_out );

            for( int y = 0; y < output.Height; ++y )
            {
                float yInFloat = ( y + 0.5f ) * y_outToIn;
                int yIn = yInFloat.FloorToInt();

                for( int x = 0; x < output.Width; ++x )
                {
                    float xInFloat = ( x + 0.5f ) * x_outToIn;
                    int xIn = xInFloat.FloorToInt();

                    output[ x, y ] = input[ xIn, yIn ];
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing.Resizing
{
    public class BicubicMitchellNetravalli
    {
        private float p0;
        private float p1;
        private float p2;
        private float p3;

        private float q0;
        private float q1;
        private float q2;
        private float q3;

        public static BicubicMitchellNetravalli Default
        {
            get
            {
                return new BicubicMitchellNetravalli( 1.0f / 3.0f, 1.0f / 3.0f );
            }
        }

        public static BicubicMitchellNetravalli Notch
        {
            get
            {
                return new BicubicMitchellNetravalli( 3.0f / 2.0f, -0.25f );
            }
        }

        public BicubicMitchellNetravalli( float b, float c )
        {
            p0 = ( 6 - 2 * b ) / 6.0f;
            p1 = 0;
            p2 = ( -18 + 12 * b + 6 * c ) / 6.0f;
            p3 = ( 12 - 9 * b - 6 * c ) / 6.0f;

            q0 = ( 8 * b + 24 * c ) / 6.0f;
            q1 = ( -12 * b - 48 * c ) / 6.0f;
            q2 = ( 6 * b + 30 * c ) / 6.0f;
            q3 = ( -b - 6 * c ) / 6.0f;
        }

        public void Apply( Image1f input, Image1f output )
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
                int yIn0 = Arithmetic.Clamp( ( yInFloat - 2 ).FloorToInt(), 0, h_in );
                int yIn1 = Arithmetic.Clamp( ( yInFloat + 2 ).CeilToInt(), 0, h_in );

                for( int xOut = 0; xOut < h_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;
                    int xIn0 = Arithmetic.Clamp( ( xInFloat - 2 ).FloorToInt(), 0, w_in );
                    int xIn1 = Arithmetic.Clamp( ( xInFloat + 2 ).FloorToInt(), 0, w_in );

                    float sumWeights = 0;
                    float sumColors = 0;

                    for( int yIn = yIn0; yIn <= yIn1; ++yIn )
                    {
                        for( int xIn = xIn0; xIn <= xIn1; ++xIn )
                        {
                            // TODO: table lookup for filter coefficients

                            var inputValue = input[ xIn, yIn ];
                            float w = EvaluateFilter2D( xInFloat - xIn, yInFloat - yIn );

                            sumWeights += w;
                            sumColors += w * inputValue;
                        }
                    }
                
                    output[ xOut, yOut ] = sumColors / sumWeights;
                }
            }
        }

        public void Apply( Image4f input, Image4f output )
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
                int yIn0 = Arithmetic.Clamp( ( yInFloat - 2 ).FloorToInt(), 0, h_in );
                int yIn1 = Arithmetic.Clamp( ( yInFloat + 2 ).CeilToInt(), 0, h_in );

                for( int xOut = 0; xOut < h_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;
                    int xIn0 = Arithmetic.Clamp( ( xInFloat - 2 ).FloorToInt(), 0, w_in );
                    int xIn1 = Arithmetic.Clamp( ( xInFloat + 2 ).FloorToInt(), 0, w_in );

                    float sumWeights = 0;
                    var sumColors = Vector4f.Zero;

                    for( int yIn = yIn0; yIn <= yIn1; ++yIn )
                    {
                        for( int xIn = xIn0; xIn <= xIn1; ++xIn )
                        {
                            // TODO: table lookup for filter coefficients

                            var inputValue = input[ xIn, yIn ];
                            float w = EvaluateFilter2D( xInFloat - xIn, yInFloat - yIn );

                            sumWeights += w;
                            sumColors += w * inputValue;
                        }
                    }
                
                    output[ xOut, yOut ] = sumColors / sumWeights;
                }
            }
        }

        public void Apply( Image4ub input, Image4ub output )
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
                int yIn0 = Arithmetic.Clamp( ( yInFloat - 2 ).FloorToInt(), 0, h_in );
                int yIn1 = Arithmetic.Clamp( ( yInFloat + 2 ).CeilToInt(), 0, h_in );

                for( int xOut = 0; xOut < w_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;
                    int xIn0 = Arithmetic.Clamp( ( xInFloat - 2 ).FloorToInt(), 0, w_in );
                    int xIn1 = Arithmetic.Clamp( ( xInFloat + 2 ).CeilToInt(), 0, w_in );

                    float sumWeights = 0;
                    var sumColors = Vector4f.Zero;

                    for( int yIn = yIn0; yIn <= yIn1; ++yIn )
                    {
                        for( int xIn = xIn0; xIn <= xIn1; ++xIn )
                        {
                            // TODO: most of the time, the left-most / top-most sample in the input is ignored
                            // also, the right-most / bottom-most sample in the input is ignored                            

                            // TODO: table lookup for filter coefficients

                            var inputValue = FormatConversion.ToFloat( input[ xIn, yIn ] );
                            float w = EvaluateFilter2D( xInFloat - xIn, yInFloat - yIn );

                            sumWeights += w;
                            sumColors += w * inputValue;
                        }
                    }
                
                    output[ xOut, yOut ] = FormatConversion.ToUnsignedByte( sumColors / sumWeights );
                }
            }
        }

        private float EvaluateFilter2D( float x, float y )
        {
            return EvaluateFilter1D( x ) * EvaluateFilter1D( y );
        }

        private float EvaluateFilter1D( float x )
        {
            x = Math.Abs( x );
            if( x <= 1 )
            {
                return p0 + p2 * x * x + p3 * x * x * x;
            }
            else if( x <= 2 )
            {
                return q0 + q1 * x + q2 * x * x + q3 * x * x * x;
            }
            else
            {
                return 0;
            }
        }
    }
}

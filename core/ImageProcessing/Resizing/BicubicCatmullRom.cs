using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing.Resizing
{
    public class BicubicCatmullRom
    {
        public BicubicCatmullRom()
        {            
            /*
                coefficients = 
                {
                    controlPoints[ 1 ],
                    0.5f * ( controlPoints[ 2 ] - controlPoints[ 0 ] ),
                    controlPoints[ 0 ] - 2.5f * controlPoints[ 1 ] + 2 * controlPoints[ 2 ] - 0.5f * controlPoints[ 3 ],
                    -0.5f * controlPoints[ 0 ] + 1.5f * controlPoints[ 1 ] - 1.5f * controlPoints[ 2 ] + 0.5f * controlPoints[ 3 ]
                }
            */
        }

        public void Apply( Image1f input, Image1f output )
        {
            float[,] controlPoints = new float[ 4, 4 ];

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
                int yIn0 = yInFloat.FloorToInt() - 1;
                int yIn1 = yIn0 + 3;

                for( int xOut = 0; xOut < w_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;
                    int xIn0 = xInFloat.FloorToInt() - 1;
                    int xIn1 = xIn0 + 3;

                    // grab the 16 control points
                    for( int yc = yIn0; yc <= yIn1; ++yc )
                    {   
                        int dy = yc - yIn0;
                        int ycc = Arithmetic.Clamp( yc, 0, h_in );                        

                        for( int xc = xIn0; xc <= xIn1; ++xc )
                        {
                            int dx = xc - xIn0;
                            int xcc = Arithmetic.Clamp( xc, 0, w_in );
                            
                            controlPoints[ xc, yc ] = input[ xcc, ycc ];
                        }
                    }
                    
                    // compute Catmull-Rom splines in the x direction
                    float tx = Arithmetic.FractionalPart( xInFloat );
                    float ty = Arithmetic.FractionalPart( yInFloat );

                    var v0i = EvaluateSpline( controlPoints[ 0, 0 ], controlPoints[ 0, 1 ], controlPoints[ 0, 2 ], controlPoints[ 0, 3 ], tx );
                    var v1i = EvaluateSpline( controlPoints[ 1, 0 ], controlPoints[ 1, 1 ], controlPoints[ 1, 2 ], controlPoints[ 1, 3 ], tx );
                    var v2i = EvaluateSpline( controlPoints[ 2, 0 ], controlPoints[ 2, 1 ], controlPoints[ 2, 2 ], controlPoints[ 2, 3 ], tx );
                    var v3i = EvaluateSpline( controlPoints[ 3, 0 ], controlPoints[ 3, 1 ], controlPoints[ 3, 2 ], controlPoints[ 3, 3 ], tx );

                    var vii = EvaluateSpline( v0i, v1i, v2i, v3i, ty );
                    
                    output[ xOut, yOut ] = vii;
                }
            }
        }

        public void Apply( Image4f input, Image4f output )
        {
            Vector4f[,] controlPoints = new Vector4f[ 4, 4 ];

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
                int yIn0 = yInFloat.FloorToInt() - 1;
                int yIn1 = yIn0 + 3;

                for( int xOut = 0; xOut < w_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;
                    int xIn0 = xInFloat.FloorToInt() - 1;
                    int xIn1 = xIn0 + 3;

                    // grab the 16 control points
                    for( int yc = yIn0; yc <= yIn1; ++yc )
                    {   
                        int dy = yc - yIn0;
                        int ycc = Arithmetic.Clamp( yc, 0, h_in );                        

                        for( int xc = xIn0; xc <= xIn1; ++xc )
                        {
                            int dx = xc - xIn0;
                            int xcc = Arithmetic.Clamp( xc, 0, w_in );
                            
                            controlPoints[ xc, yc ] = input[ xcc, ycc ];
                        }
                    }
                    
                    // compute Catmull-Rom splines in the x direction
                    float tx = Arithmetic.FractionalPart( xInFloat );
                    float ty = Arithmetic.FractionalPart( yInFloat );

                    var v0i = EvaluateSpline( controlPoints[ 0, 0 ], controlPoints[ 0, 1 ], controlPoints[ 0, 2 ], controlPoints[ 0, 3 ], tx );
                    var v1i = EvaluateSpline( controlPoints[ 1, 0 ], controlPoints[ 1, 1 ], controlPoints[ 1, 2 ], controlPoints[ 1, 3 ], tx );
                    var v2i = EvaluateSpline( controlPoints[ 2, 0 ], controlPoints[ 2, 1 ], controlPoints[ 2, 2 ], controlPoints[ 2, 3 ], tx );
                    var v3i = EvaluateSpline( controlPoints[ 3, 0 ], controlPoints[ 3, 1 ], controlPoints[ 3, 2 ], controlPoints[ 3, 3 ], tx );

                    var vii = EvaluateSpline( v0i, v1i, v2i, v3i, ty );
                    
                    output[ xOut, yOut ] = vii;
                }
            }
        }
        
        public void Apply( Image4ub input, Image4ub output )
        {
            Vector4f[,] controlPoints = new Vector4f[ 4, 4 ];

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
                int yIn0 = yInFloat.FloorToInt() - 1;
                int yIn1 = yIn0 + 3;

                for( int xOut = 0; xOut < w_out; ++xOut )
                {
                    float xInFloat = ( xOut + 0.5f ) * x_outToIn;
                    int xIn0 = xInFloat.FloorToInt() - 1;
                    int xIn1 = xIn0 + 3;

                    // grab the 16 control points
                    for( int yc = yIn0; yc <= yIn1; ++yc )
                    {   
                        int dy = yc - yIn0;
                        int ycc = Arithmetic.Clamp( yc, 0, h_in );                        

                        for( int xc = xIn0; xc <= xIn1; ++xc )
                        {
                            int dx = xc - xIn0;
                            int xcc = Arithmetic.Clamp( xc, 0, w_in );
                            
                            controlPoints[ xc, yc ] = FormatConversion.ToFloat( input[ xcc, ycc ] );
                        }
                    }
                    
                    // compute Catmull-Rom splines in the x direction
                    float tx = Arithmetic.FractionalPart( xInFloat );
                    float ty = Arithmetic.FractionalPart( yInFloat );

                    var v0i = EvaluateSpline( controlPoints[ 0, 0 ], controlPoints[ 0, 1 ], controlPoints[ 0, 2 ], controlPoints[ 0, 3 ], tx );
                    var v1i = EvaluateSpline( controlPoints[ 1, 0 ], controlPoints[ 1, 1 ], controlPoints[ 1, 2 ], controlPoints[ 1, 3 ], tx );
                    var v2i = EvaluateSpline( controlPoints[ 2, 0 ], controlPoints[ 2, 1 ], controlPoints[ 2, 2 ], controlPoints[ 2, 3 ], tx );
                    var v3i = EvaluateSpline( controlPoints[ 3, 0 ], controlPoints[ 3, 1 ], controlPoints[ 3, 2 ], controlPoints[ 3, 3 ], tx );

                    var vii = EvaluateSpline( v0i, v1i, v2i, v3i, ty );
                    
                    output[ xOut, yOut ] = FormatConversion.ToUnsignedByte( vii );
                }
            }
        }        

        private static float EvaluateSpline( float p0, float p1, float p2, float p3, float t )
        {
            var c0 = p1;
            var c1 = 0.5f * ( p2 - p0 );
            var c2 = p0 - 2.5f * p1 + 2 * p2 - 0.5f * p3;
            var c3 = -0.5f * p0 + 1.5f * p1 - 1.5f * p2 + 0.5f * p3;
            
            float t2 = t * t;
            float t3 = t2 * t;

            return c0 + c1 * t + c2 * t2 + c3 * t3;
        }

        private static Vector4f EvaluateSpline( Vector4f p0, Vector4f p1, Vector4f p2, Vector4f p3, float t )
        {
            var c0 = p1;
            var c1 = 0.5f * ( p2 - p0 );
            var c2 = p0 - 2.5f * p1 + 2 * p2 - 0.5f * p3;
            var c3 = -0.5f * p0 + 1.5f * p1 - 1.5f * p2 + 0.5f * p3;
            
            float t2 = t * t;
            float t3 = t2 * t;

            return c0 + c1 * t + c2 * t2 + c3 * t3;
        }
    }
}

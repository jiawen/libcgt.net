using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public class LineDrawing2D
    {
        /// <summary>
        /// Draws a line from (x0,y0) to (x1,y1) using
        /// Bresenham's Line Rasterization algorithm.
        /// 
        /// It only supports integer endpoint coordinates.
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="im"></param>
        /// <param name="color"></param>
        public static void Bresenham( int x0, int y0, int x1, int y1,
            Image4f im, Vector4f color, bool yPointsUp )
        {
            if( yPointsUp )
            {
                y0 = im.Height - y0 - 1;
                y1 = im.Height - y1 - 1;
            }

            bool steep = Math.Abs( y1 - y0 ) > Math.Abs( x1 - x0 );
            if( steep )
            {
                Swap( ref x0, ref y0 );
                Swap( ref x1, ref y1 );
            }
            if( x0 > x1 )
            {
                Swap( ref x0, ref x1 );
                Swap( ref y0, ref y1 );
            }

            int deltaX = x1 - x0;
            int deltaY = Math.Abs( y1 - y0 );
            int error = deltaX / 2;
            int yStep;
            int y = y0;
            if( y0 < y1 )
            {
                yStep = 1;
            }
            else
            {
                yStep = -1;
            }

            for( int x = x0; x <= x1; ++x )
            {
                if( steep )
                {
                    im[ y, x ] = color;
                }
                else
                {
                    im[ x, y ] = color;
                }
                error -= deltaY;
                if( error < 0 )
                {
                    y += yStep;
                    error += deltaX;
                }
            }
        }

#if false
        // TODO: the endpoints aren't done yet
        public static void Wu( float x1, float y1, float x2, float y2,
            Image4f im, Vector3f color )
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            bool steep = ( Math.Abs( dx ) < Math.Abs( dy ) );
            if( steep )
            {
                Swap( ref x1, ref y1 );
                Swap( ref x2, ref y2 );
            }
            if( x2 < x1 )
            {
                Swap( ref x1, ref x2 );
                Swap( ref y1, ref y2 );
            }

            float gradient;
            if( steep )
            {
                gradient = dx / dy;
            }
            else
            {
                gradient = dy / dx;
            }
            
            // handle first endpoint
            float xEnd = MathUtils.Round( x1 );
            float yEnd = y1 + gradient * ( xEnd - x1 );
            float xGap = 1 - x1.FractionalPart();
            int xpxl1 = ( int )xEnd;
            int ypxl1 = yEnd.FloorToInt();
            
            //im[ xpxl1, ypxl1 ] = new Vector4f( color * xGap * ( 1 - yEnd.FractionalPart() ), 1 );
            //im[ xpxl1, ypxl1 + 1 ] = new Vector4f( color * xGap * yEnd.FractionalPart(), 1 );

            float interY = yEnd + gradient;

            // handle second endpoing
            xEnd = MathUtils.Round( x2 );
            yEnd = y2 + gradient * ( xEnd - x2 );
            xGap = ( x2 + 0.5f ).FractionalPart();
            int xpxl2 = ( int )xEnd;
            int ypxl2 = yEnd.FloorToInt();
            //im[ xpxl2, ypxl2 ] = new Vector4f( color * xGap * ( 1 - yEnd.FractionalPart() ), 1 );
            //im[ xpxl2, ypxl2 + 1 ] = new Vector4f( color * xGap * yEnd.FractionalPart(), 1 );

            // main loop
            for( int x = xpxl1 + 1; x < xpxl2; ++x )
            {
                //im[ x, interY.FloorToInt() ] = new Vector4f( color * ( 1.0f - interY.FractionalPart() ), 1 );
                //im[ x, interY.FloorToInt() + 1 ] = new Vector4f( color * ( interY.FractionalPart() ), 1 );

                if( steep )
                {
                    im[ interY.FloorToInt(), x ] = new Vector4f( color * ( 1.0f - interY.FractionalPart() ), 1 );
                    im[ interY.FloorToInt() + 1, x ] = new Vector4f( color * ( interY.FractionalPart() ), 1 );
                }
                else
                {
                    im[ x, interY.FloorToInt() ] = new Vector4f( color * ( 1.0f - interY.FractionalPart() ), 1 );
                    im[ x, interY.FloorToInt() + 1 ] = new Vector4f( color * ( interY.FractionalPart() ), 1 );
                }
                
                interY += gradient;
            }
        }
#endif

        private static void Swap< T >( ref T x, ref T y )
        {
            T z = x;
            x = y;
            y = z;
        }
    }
}

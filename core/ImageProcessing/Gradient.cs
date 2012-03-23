using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.ImageProcessing
{
    public static class Gradient
    {
        public static void ImageGradient( this Image1f image, out Image1f fx, out Image1f fy )
        {
            int width = image.Width;
            int height = image.Height;

            fx = new Image1f( width, height );
            fy = new Image1f( width, height );

            Image1f dx = fx;
            Image1f dy = fy;

            // edges: forward difference
            for( int y = 0; y < height; ++y )
            {
                dx[ 0, y ] = image[ 1, y ] - image[ 0, y ];
                dx[ width - 1, y ] = image[ width - 1, y ] - image[ width - 2, y ];
            }

            for( int x = 0; x < width; ++x )
            {
                dy[ x, 0 ] = image[ x, 1 ] - image[ x, 0 ];
                dy[ x, height - 1 ] = image[ x, height - 1 ] - image[ x, height - 2 ];
            }

            // rest: centered difference
            for( int y = 0; y < height; ++y )
            {
                for( int x = 1; x < width - 1; ++x )
                {
                    dx[ x, y ] = 0.5f * image[ x + 1, y ] - 0.5f * image[ x - 1, y ];
                }
            }

            for( int x = 0; x < width; ++x )                
            {
                for( int y = 1; y < height - 1; ++y )
                {
                    dy[ x, y ] = 0.5f * image[ x, y + 1 ] - 0.5f * image[ x, y - 1 ];
                }
            }            
        }        

        public static void ImageGradient( this Image4f image, out Image4f fx, out Image4f fy )
        {
            int width = image.Width;
            int height = image.Height;

            fx = new Image4f( width, height );
            fy = new Image4f( width, height );

            Image4f dx = fx;
            Image4f dy = fy;

            // edges: forward difference
            for( int y = 0; y < height; ++y )
            {
                dx[ 0, y ] = image[ 1, y ] - image[ 0, y ];
                dx[ width - 1, y ] = image[ width - 1, y ] - image[ width - 2, y ];
            }
            
            for( int x = 0; x < width; ++x )
            {
                dy[ x, 0 ] = image[ x, 1 ] - image[ x, 0 ];
                dy[ x, height - 1 ] = image[ x, height - 1 ] - image[ x, height - 2 ];
            }            

            // rest: centered difference
            for( int y = 0; y < height; ++y )
            {
                for( int x = 1; x < width - 1; ++x )
                {
                    dx[ x, y ] = 0.5f * image[ x + 1, y ] - 0.5f * image[ x - 1, y ];
                }
            }

            for( int x = 0; x < width; ++x )
            {
                for( int y = 1; y < height - 1; ++y )
                {
                    dy[ x, y ] = 0.5f * image[ x, y + 1 ] - 0.5f * image[ x, y - 1 ];
                }
            }            
        }

        public static Image1f ImageGradientNorm( this Image1f image )
        {
            int width = image.Width;
            int height = image.Height;

            Image1f dx;
            Image1f dy;
            Image1f gn = new Image1f( width, height );

            image.ImageGradient( out dx, out dy );
            
            for( int y = 0; y < height; ++y )
            {                    
                for( int x = 0; x < image.Width; ++x )
                {
                    gn[ x, y ] = MathUtils.Sqrt( dx[ x, y ] * dx[ x, y ] + dy[ x, y ] * dy[ x, y ] );
                }
            }

            return gn;
        }
    }
}

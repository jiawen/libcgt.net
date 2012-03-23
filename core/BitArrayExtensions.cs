using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class BitArrayExtensions
    {
        public static bool AllZero( this BitArray arr )
        {
            for( int i = 0; i < arr.Length; ++i )
            {
                if( arr[ i ] )
                {
                    return false;
                }
            }
            return true;
        }

        public static bool AllOne( this BitArray arr )
        {
            for( int i = 0; i < arr.Length; ++i )
            {
                if( !( arr[ i ] ) )
                {
                    return false;
                }
            }
            return true;
        }

        public static void SavePNG( this BitArray arr, Rect2i rect, Vector2i imageSize, string filename )
        {
            Image1ub im = new Image1ub( imageSize );
            
            int i = 0;
            int x0 = rect.Origin.x;
            int y0 = rect.Origin.y;
            for( int y = 0; y < rect.Height; ++y )
            {
                for( int x = 0; x < rect.Width; ++x )
                {
                    if( arr[ i ] )
                    {
                        im.SetPixelFlipped( x0 + x, y0 + y, 255 );
                    }
                    ++i;
                }
            }
            im.SavePNG( filename );
        }

        public static void CopyToImage( this BitArray arr, Rect2i rect, int channel, Image4ub im )
        {
            int i = 0;
            int x0 = rect.Origin.x;
            int y0 = rect.Origin.y;
            for( int y = 0; y < rect.Height; ++y )
            {
                for( int x = 0; x < rect.Width; ++x )
                {
                    if( arr[ i ] )
                    {
                        var color = im.GetPixelFlipped( x0 + x, y0 + y );
                        if( channel == 0 )
                        {
                            color.x = 255;
                        }
                        else if( channel == 1 )
                        {
                            color.y = 255;
                        }
                        else
                        {
                            color.z = 255;
                        }
                        im.SetPixelFlipped( x0 + x, y0 + y, color );
                    }
                    ++i;
                }
            }            
        }
    }
}

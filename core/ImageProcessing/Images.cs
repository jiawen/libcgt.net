using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    public static class Images
    {
        public static void CopySubImage( Image1f source, int sx, int sy,
            Image1f target, int tx, int ty,
            int width, int height )
        {
            int sw = source.Width;
            int sh = source.Height;
            int tw = target.Width;
            int th = target.Height;

            if( sx < 0 || sy < 0 || sx >= sw || sy >= sh )
            {
                throw new IndexOutOfRangeException( "Source origin must be inside source image" );
            }
            if( tx < 0 || ty < 0 || tx >= tw || ty >= th )
            {
                throw new IndexOutOfRangeException( "Target origin must be inside target image" );
            }

            int tx0 = tx;
            int tx1 = Math.Min( tx + width, tw );
            int ty0 = ty;
            int ty1 = Math.Min( ty + height, th );

            for( int y = ty0; y < ty1; ++y )
            {
                int dy = y - ty0;
                int iy = sy + dy.Clamp(  0, sh );

                for( int x = tx0; x < tx1; ++x )
                {
                    int dx = x - tx0;
                    int ix = ( sx + dx ).Clamp( 0, sw );

                    target[ x, y ] = source[ ix, iy ];
                }
            }
        }

        public static void CopySubImage( Image4f source, int sx, int sy,
            Image4f target, int tx, int ty,
            int width, int height )
        {
            int sw = source.Width;
            int sh = source.Height;
            int tw = target.Width;
            int th = target.Height;

            if( sx < 0 || sy < 0 || sx >= sw || sy >= sh )
            {
                throw new IndexOutOfRangeException( "Source origin must be inside source image" );
            }
            if( tx < 0 || ty < 0 || tx >= tw || ty >= th )
            {
                throw new IndexOutOfRangeException( "Target origin must be inside target image" );
            }

            int tx0 = tx;
            int tx1 = Math.Min( tx + width, tw );
            int ty0 = ty;
            int ty1 = Math.Min( ty + height, th );

            for( int y = ty0; y < ty1; ++y )
            {
                int dy = y - ty0;
                int iy = sy + dy.Clamp(  0, sh );

                for( int x = tx0; x < tx1; ++x )
                {
                    int dx = x - tx0;
                    int ix = ( sx + dx ).Clamp( 0, sw );

                    target[ x, y ] = source[ ix, iy ];
                }
            }
        }

        public static void CopySubImage( Image4ub source, int sx, int sy,
            Image4ub target, int tx, int ty,
            int width, int height )
        {
            int sw = source.Width;
            int sh = source.Height;
            int tw = target.Width;
            int th = target.Height;

            if( sx < 0 || sy < 0 || sx >= sw || sy >= sh )
            {
                throw new IndexOutOfRangeException( "Source origin must be inside source image" );
            }
            if( tx < 0 || ty < 0 || tx >= tw || ty >= th )
            {
                throw new IndexOutOfRangeException( "Target origin must be inside target image" );
            }

            int tx0 = tx;
            int tx1 = Math.Min( tx + width, tw );
            int ty0 = ty;
            int ty1 = Math.Min( ty + height, th );

            for( int y = ty0; y < ty1; ++y )
            {
                int dy = y - ty0;
                int iy = sy + dy.Clamp(  0, sh );

                for( int x = tx0; x < tx1; ++x )
                {
                    int dx = x - tx0;
                    int ix = ( sx + dx ).Clamp( 0, sw );

                    target[ x, y ] = source[ ix, iy ];
                }
            }
        }        
    }
}

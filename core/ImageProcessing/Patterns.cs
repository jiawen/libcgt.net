using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    public static class Patterns
    {
        public static void SetCheckerboard< T >( IImage< T > im, int checkerSize,
            T whiteColor, T blackColor )
        {
            int width = im.Width;
            int height = im.Height;

	        int nBlocksX = 1 + width / checkerSize;
	        int nBlocksY = 1 + height / checkerSize;

	        bool rowIsWhite = true;
	        bool isWhite;

	        for( int by = 0; by < nBlocksY; ++by )
	        {
		        isWhite = rowIsWhite;
		        for( int bx = 0; bx < nBlocksX; ++bx )
		        {
			        for( int y = by * checkerSize; ( y < ( by + 1 ) * checkerSize ) && ( y < height ); ++y )
			        {
				        for( int x = bx * checkerSize; ( x < ( bx + 1 ) * checkerSize ) && ( x < width ); ++x )
				        {
					        if( isWhite )
					        {
						        im[ x, y ] = whiteColor;
					        }
					        else
					        {
						        im[ x, y ] = blackColor;
					        }
				        }
			        }

			        isWhite = !isWhite;
		        }
		        rowIsWhite = !rowIsWhite;
	        }
        }
    }
}

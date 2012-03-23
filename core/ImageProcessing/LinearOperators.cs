using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    public static class LinearOperators
    {
        public static void Convolve( this Image4f image, Image1f kernel, Image4f output )
        {
            int kernelRadiusX = kernel.Width / 2;
            int kernelRadiusY = kernel.Height / 2;

            for( int y = 0; y < image.Height; ++y )
            {
                int y0 = ( y - kernelRadiusY ).Clamp( 0, image.Height );
                int y1 = ( y + kernelRadiusY ).Clamp( 0, image.Height );

                for( int x = 0; x < image.Width; ++x )
                {
                    int x0 = ( x - kernelRadiusX ).Clamp( 0, image.Width );
                    int x1 = ( x + kernelRadiusX ).Clamp( 0, image.Width );
                    
                    Vector4f sum = Vector4f.Zero;
                    
                    for( int yy = y0; yy <= y1; ++yy )
                    {
                        for( int xx = x0; xx <= x1; ++xx )
                        {
                            sum += image[ xx, yy ] * kernel[ xx - ( x - kernelRadiusX ), yy - ( y - kernelRadiusY ) ];
                        }
                    }

                    output[ x, y ] = sum;
                }
            }
        }
    }
}

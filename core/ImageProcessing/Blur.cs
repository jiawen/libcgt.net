using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    public static class Blur
    {
        public static Image1f CreateGaussiankernel( float sigma )
        {
            float sigmaSquared = sigma * sigma;
            float twoSigma = 2.0f * sigma;
            int radius = twoSigma.CeilToInt();
            int diameter = 2 * radius + 1;

            Image1f kernel = new Image1f( diameter, diameter );
            float sum = 0;

            for( int y = 0; y < diameter; ++y )
            {
                int dy = y - radius;

                for( int x = 0; x < diameter; ++x )
                {
                    int dx = x - radius;
                    float val = ( float )( Math.Exp( -0.5f * ( dx * dx + dy * dy ) / sigmaSquared ) );
                    sum += val;

                    kernel[ x, y ] = val;
                }
            }

            for( int y = 0; y < diameter; ++y )
            {
                for( int x = 0; x < diameter; ++x )
                {
                    kernel[ x, y ] /= sum;
                }
            }

            return kernel;
        }
    }
}

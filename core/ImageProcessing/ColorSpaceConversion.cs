using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    public static class ColorSpaceConversion
    {
        // TODO: xyz, lab, hsv, ...

        public static readonly Vector3f DEFAULT_RGB2LUMINANCE_COEFFICIENTS = new Vector3f( 0.3279f, 0.6557f, 0.0164f );
        public static readonly Vector3f MATLAB_RGB2GRAY_COEFFICIENTS = new Vector3f( 0.2989f, 0.5870f, 0.1140f );

        public static Image1f RGB2LuminanceProject( this Image4f rgba )
        {
            return RGB2LuminanceProject( rgba, DEFAULT_RGB2LUMINANCE_COEFFICIENTS );
        }

        public static Image1f RGB2LuminanceProject( this Image4f rgba, Vector3f coefficients )
        {
            int width = rgba.Width;
            int height = rgba.Height;
            Image1f gray = new Image1f( width, height );

            for( int y = 0; y < height; ++y )
            {
                for( int x = 0; x < width; ++x )
                {
                    gray[ x, y ] = Vector3f.Dot( rgba[ x, y ].XYZ, coefficients );
                }
            }

            return gray;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.ImageProcessing
{
    public static class Compositing
    {
        public static Vector4f PreMultiplyAlpha( this Vector4f v )
        {
            return new Vector4f( v.x * v.w, v.y * v.w, v.z * v.w, v.w );
        }

        // TODO: check for v.w > 0?
        public static Vector4f PostDivideAlpha( this Vector4f v )
        {
            return new Vector4f( v.x / v.w, v.y / v.w, v.z / v.w, v.w );            
        }

        public static Vector4ub Over( Vector4ub foreground, Vector4ub background )
        {
            return Over( foreground.ToFloat(), background.ToFloat() ).ToUnsignedByte();
        }

        public static Vector4f Over( Vector4f foreground, Vector4f background )
        {
            // c = composite, f = foreground, b = background
            // red channel:
	        // c_r = f_a * f_r + ( 1 - f_a ) * b_r
	        //
	        // alpha channel:
	        // c_a = f_a + b_a * ( 1 - f_a )
            Vector3f rgb = foreground.w * foreground.XYZ + ( 1.0f - foreground.w ) * background.XYZ;
            float a = foreground.w + background.w * ( 1 - foreground.w );

            return new Vector4f( rgb, a );
        }

        /// <summary>
        /// Given the two colors, picks the one with larger alpha
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static Vector4f MaximumAlpha( Vector4f foreground, Vector4f background )
        {
            return ( foreground.w > background.w ) ? foreground : background;
        }

        /*
        public static Color ExtractBackgroundColor( Color composite, Color foreground )
        {
            return ExtractBackgroundColor( composite.ToFloat(), foreground.ToFloat() ).ToUnsignedByte();
        }
        */

        public static Vector4ub ExtractBackgroundColor( Vector4ub composite, Vector4ub foreground )
        {
            return ExtractBackgroundColor( composite.ToFloat(), foreground.ToFloat() ).ToUnsignedByte();
        }

        public static Vector4f ExtractBackgroundColor( Vector4f composite, Vector4f foreground )
        {
            // c = composite, f = foreground, b = background
            // red channel:
	        // c_r = f_a * f_r + ( 1 - f_a ) * b_r
	        // ==> b_r = ( c_r - f_a * f_r ) / ( 1 - f_a )
	        //
	        // alpha channel:
	        // c_a = f_a + b_a * ( 1 - f_a )
	        // ==> b_a = ( c_a - f_a ) / ( 1 - f_a )

            Vector3f compositeRGB = composite.XYZ;
            Vector3f foregroundRGB = foreground.XYZ;
            float ca = composite.w;
            float fa = foreground.w;

            if( fa > 0 && fa < 1 ) // partially transparent
            {
                var backgroundRGB = new Vector3f( compositeRGB - fa * foregroundRGB ) / ( 1.0f - fa );
                var ba = ( ca - fa ) / ( 1 - fa );

                return new Vector4f( backgroundRGB, ba ).Saturate();
            }
            else if( fa < 0.5f ) // foreground is fully transparent: background = input
            {
                return composite;
            }
            else // fa == 1, foreground is completely opaque, have no idea what the background is, return opaque black
            {
                // return new Vector4f( 0, 0, 0, 1 );
                return Vector4f.Zero;
            }
        }
    }
}

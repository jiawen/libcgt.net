using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Vecmath
{
    class MatrixUtil
    {
        public static float Determinant2x2f( float m00, float m01, float m10, float m11 )
        {
            return ( m00 * m11 - m01 * m10 );
        }

        public static double Determinant2x2d( double m00, double m01, double m10, double m11 )
        {
            return ( m00 * m11 - m01 * m10 );
        }

        public static float Determinant3x3f( float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22 )
        {
            return
            (
                m00 * Determinant2x2f( m11, m12, m21, m22 ) -
                m01 * Determinant2x2f( m10, m12, m20, m22 ) +
                m02 * Determinant2x2f( m10, m11, m20, m21 )
            );
        }

        public static double Determinant3x3d( double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22 )
        {
            return ( m00 * Determinant2x2d( m11, m12, m21, m22 ) -
                    m01 * Determinant2x2d( m10, m12, m20, m22 ) +
                    m02 * Determinant2x2d( m10, m11, m20, m21 ) );
        }
    }
}

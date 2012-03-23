#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;
using libcgt.XNA;

namespace libcgt.core.Geometry
{
    public static class KleinBottle
    {
        public static VertexPosition3fTexture2f[] Create( int nu, int nv,
            double h, double r0, double d )
        {
            var output = new VertexPosition3fTexture2f[ 6 * ( nu - 1 ) * ( nv - 1 ) ];
            int index = 0;

            const double twoPi = 2 * Math.PI;
            double du = twoPi / ( nu - 1 );
            double dv = twoPi / ( nv - 1 );

            for( int iu = 0; iu < nu - 1; ++iu )
            {
                double u0 = iu * du;
                double u1 = ( iu + 1 ) * du;

                // HACK
                if( iu == 0 )
                {
                    u0 = 0.0001;
                }
                if( iu == nu - 1 )
                {
                    u1 = twoPi - 0.0001;
                }

                double a0 = ComputeA( u0, h );
                double a1 = ComputeA( u1, h );

                double r_0 = r0 * ( 1 + d * Math.Sin( u0 ) );
                double r_1 = r0 * ( 1 + d * Math.Sin( u1 ) );

                for( int iv = 0; iv < nv - 1; ++iv )
                {
                    double v0 = iv * dv;
                    double v1 = ( iv + 1 ) * dv;

                    // HACK
                    if( iv == 0 )
                    {
                        v0 = 0.0001;
                    }
                    if( iv == nv - 1 )
                    {
                        v1 = twoPi - 0.0001;
                    }

                    double x00 = ComputeX( u0, v0, r_0, h, a0 );
                    double x01 = ComputeX( u0, v1, r_0, h, a0 );
                    double x10 = ComputeX( u1, v0, r_1, h, a1 );
                    double x11 = ComputeX( u1, v1, r_1, h, a1 );

                    double y00 = ComputeY( r_0, v0 );
                    double y01 = ComputeY( r_0, v1 );
                    double y10 = ComputeY( r_1, v0 );
                    double y11 = ComputeY( r_1, v1 );
                    
                    double z00 = ComputeZ( u0, v0, r_0, h, a0 );
                    double z01 = ComputeZ( u0, v1, r_0, h, a0 );
                    double z10 = ComputeZ( u0, v0, r_1, h, a1 );
                    double z11 = ComputeZ( u0, v1, r_1, h, a1 );

                    Vector3f v00 = new Vector3f( ( float )x00, ( float )y00, ( float )z00 );
                    Vector3f v01 = new Vector3f( ( float )x01, ( float )y01, ( float )z01 );
                    Vector3f v10 = new Vector3f( ( float )x10, ( float )y10, ( float )z10 );
                    Vector3f v11 = new Vector3f( ( float )x11, ( float )y11, ( float )z11 );

                    Vector2f t00 = new Vector2f( ( float )( u0 / twoPi ), ( float )( v0 / twoPi ) );
                    Vector2f t01 = new Vector2f( ( float )( u0 / twoPi ), ( float )( v1 / twoPi ) );
                    Vector2f t10 = new Vector2f( ( float )( u1 / twoPi ), ( float )( v0 / twoPi ) );
                    Vector2f t11 = new Vector2f( ( float )( u1 / twoPi ), ( float )( v1 / twoPi ) );

                    output[ index ] = new VertexPosition3fTexture2f( v00, t00 );
                    output[ index + 1 ] = new VertexPosition3fTexture2f( v10, t10 );
                    output[ index + 2 ] = new VertexPosition3fTexture2f( v11, t11 );

                    output[ index + 3 ] = new VertexPosition3fTexture2f( v00, t00 );
                    output[ index + 4 ] = new VertexPosition3fTexture2f( v11, t11 );
                    output[ index + 5 ] = new VertexPosition3fTexture2f( v01, t01 );

                    index += 6;
                }
            }

            return output;
        }
        
        private static double ComputeA( double u, double h )
        {
            double cosineTerm = Math.Cos( u ) - Math.Cos( 2 * u );
            cosineTerm = cosineTerm * cosineTerm;

            double sineTerm = h * Math.Sin( u );
            sineTerm = sineTerm * sineTerm;
            return Math.Sqrt( cosineTerm + sineTerm );
        }

        private static double ComputeX( double u, double v, double r, double h, double a )
        {
            return Math.Sin( u ) - 0.5 * Math.Sin( 2 * u ) + h * r * Math.Sin( u ) * Math.Cos( v ) / a;
        }

        private static double ComputeY( double r, double v )
        {
            return r * Math.Sin( v );
        }

        private static double ComputeZ( double u, double v, double r, double h, double a )
        {
            return -h * Math.Cos( u ) + r * ( Math.Cos( 2 * u ) - Math.Cos( u ) ) * Math.Cos( v ) / a;
        }
    }
}
#endif
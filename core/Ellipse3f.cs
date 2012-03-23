using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public class Ellipse3f
    {
        public Vector3f Center { get; set; }
        public Vector3f Right { get; set; }
        public Vector3f Up { get; set; }

        public Ellipse3f( Vector3f center, Vector3f right, Vector3f up )
        {
            Center = center;
            Right = right;
            Up = up;
        }

        public Vector3f Normal
        {
            get
            {
                return Vector3f.Cross( Right, Up );
            }
        }

        public Plane3f Plane
        {
            get
            {
                return new Plane3f( Center, Normal );
            }
        }
        
        public Vector3f Sample( float theta )
        {
            float x = ( float ) ( Math.Cos( theta ) );
            float y = ( float ) ( Math.Sin( theta ) );

            return Center + x * Right + y * Up;
        }

        /// <summary>
        /// Returns nSamples samples of the ellipse in [0, 2pi)
        /// (The first sample is at theta = 0, the last sample is at 2pi * ( nSamples - 1 ) / nSamples)
        /// </summary>
        /// <param name="nSamples"></param>
        /// <returns></returns>
        public Vector3f[] Sample( int nSamples )
        {
            var output = new Vector3f[ nSamples ];

            for( int i = 0; i < nSamples; ++i )
            {
                float theta = i * MathUtils.TWO_PI / nSamples;
                output[ i ] = Sample( theta );
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public RayEllipseIntersectionRecord IntersectRay( Vector3f origin, Vector3f direction, float distanceEpsilon )
        {
            var plane = Plane;
            var pir = plane.IntersectRay( origin, direction );
            if( pir.Intersected )
            {
                var r = pir.p - Center;
                var unitX = Right.Normalized();
                var unitY = Up.Normalized();

                float x = Vector3f.ProjectionLength( r, unitX );
                float y = Vector3f.ProjectionLength( r, unitY );

                float theta = ( float ) ( Math.Atan2( y, x ) );

                var pointOnEllipse = Sample( theta );
                if( ( pir.p - pointOnEllipse ).Norm() < distanceEpsilon )
                {
                    return RayEllipseIntersectionRecord.Single( pir.t, pir.p, theta, pointOnEllipse );
                }                
            }

            return RayEllipseIntersectionRecord.None;
        }
    }
}

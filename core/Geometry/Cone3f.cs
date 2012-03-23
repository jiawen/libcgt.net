using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    /// <summary>
    /// Standard cone of radius r at the base and height h, with apex on the z-axis:
    /// x^2 + y^2 = ( r^2 / h^2 ) ( z - h )^2
    /// 
    /// A unit cone:
    /// x^2 + y^2 = ( z - 1 )^2
    /// </summary>
    public class Cone3f
    {
        private Vector3f origin;
        private Vector3f axisDirection;
        private float radius;
        private float height;

        private Matrix4f objectToWorld;
        private Matrix4f worldToObject;

        /// <summary>
        /// axisDirection needs to be a unit vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="axisDirection"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        public Cone3f( Vector3f origin, Vector3f axisDirection, float radius, float height )
        {
            // rotate the z-axis into axisDirection
            // TODO: use quaternions
            this.origin = origin;
            this.axisDirection = axisDirection;
            this.radius = radius;
            this.height = height;

            UpdateTransform();
        }        

        public Vector3f Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
                UpdateTransform();
            }
        }        

        public Vector3f Apex
        {
            get
            {
                return objectToWorld.Transform( new Vector3f( 0, 0, 1 ) );
            }
        }

        public Vector3f SampleBase( float theta )
        {
            float x = ( float ) ( Math.Cos( theta ) );
            float y = ( float ) ( Math.Sin( theta ) );
            return objectToWorld.Transform( new Vector3f( x, y, 0 ) );
        }

        public Vector3f[] SampleBase( int nSamples )
        {
            var output = new Vector3f[ nSamples ];

            for( int i = 0; i < nSamples; ++i )
            {
                float theta = i * MathUtils.TWO_PI / nSamples;
                output[ i ] = SampleBase( theta );
            }

            return output;
        }
        
        public RayIntersectionRecord IntersectRay( Vector3f rayOrigin, Vector3f rayDirection )
        {
            var pointAheadWS = rayOrigin + rayDirection;
            var pointAheadOS = worldToObject.Transform( pointAheadWS );

            var rayOriginOS = worldToObject.Transform( rayOrigin );
            var rayDirectionOS = ( pointAheadOS - rayOriginOS ).Normalized();
            
            float scaleWorldToObject = ( pointAheadOS - rayOriginOS ).Norm(); // 1 unit in world space is scaleWorldToObject in object space

            float a = rayDirectionOS.x * rayDirectionOS.x + rayDirectionOS.y * rayDirectionOS.y - rayDirectionOS.z * rayDirectionOS.z;
            float b = 2 * ( rayOriginOS.x * rayDirectionOS.x + rayOriginOS.y * rayDirectionOS.y - rayOriginOS.z * rayDirectionOS.z + rayDirectionOS.z );
            float c = rayOriginOS.x * rayOriginOS.x + rayOriginOS.y * rayOriginOS.y - rayOriginOS.z * rayOriginOS.z +
                      2 * rayOriginOS.z - 1;

            // TODO: what if a is zero?
            // when does this happen?
            // solve linear equation?

            bool intersectSide;
            float sideIntersectionT = 0;
            Vector3f sideIntersectionPoint = Vector3f.Zero;
            // solve quadratic
            float discrim = b * b - 4 * a * c;
            if( discrim < 0 )
            {
                intersectSide = false;
            }
            else
            {
                float t0 = ( -b + discrim ) / ( 2 * a );
                float t1 = ( -b - discrim ) / ( 2 * a );

                if( t0 < 0 && t1 < 0 )
                {
                    intersectSide = false;
                }
                else
                {
                    // get the minimum one that's greater than 0
                    if( t0 > 0 && t1 > 0 )
                    {
                        sideIntersectionT = Math.Min( t0, t1 );
                    }
                    else if( t0 > 0 )
                    {
                        sideIntersectionT = t0;
                    }
                    else
                    {
                        sideIntersectionT = t1;
                    }

                    sideIntersectionPoint = rayOriginOS + sideIntersectionT * rayDirectionOS;
                    if( sideIntersectionPoint.z > 0 )
                    {
                        intersectSide = true;
                    }
                    else
                    {
                        intersectSide = false;
                    }
                }
            }

            // also intersect the plane
            var plane = Plane3f.XY;            
            var hitPlane = plane.IntersectRay( rayOriginOS, rayDirectionOS );
            bool intersectPlane = hitPlane.Intersected;

            // make sure it's within the cap
            if( hitPlane.t < 0 || hitPlane.p.NormSquared() > 1 )
            {
                intersectPlane = false;
            }

            float t;
            Vector3f intersectionPoint;
            
            // if they both intersect
            if( intersectPlane && intersectSide )
            {
                if( hitPlane.t < sideIntersectionT )
                {                    
                    t = hitPlane.t;
                    intersectionPoint = hitPlane.p;
                }
                else
                {
                    t = sideIntersectionT;
                    intersectionPoint = sideIntersectionPoint;
                }
            }
            else if( intersectPlane )
            {
                t = hitPlane.t;
                intersectionPoint = hitPlane.p;
            }
            else
            {
                t = sideIntersectionT;
                intersectionPoint = sideIntersectionPoint;
            }

            if( intersectPlane || intersectSide )
            {
                // transform t back into world space
                t = t / scaleWorldToObject;
                intersectionPoint = objectToWorld.Transform( intersectionPoint );

                return RayIntersectionRecord.Single( t, intersectionPoint );
            }
            else
            {
                return RayIntersectionRecord.None;
            }
        }

        private void UpdateTransform()
        {
            var z = new Vector3f( 0, 0, 1 );
        }
    }
}

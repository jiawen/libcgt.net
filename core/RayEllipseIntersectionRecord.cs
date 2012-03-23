using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public class RayEllipseIntersectionRecord : RayIntersectionRecord
    {
        public static new RayEllipseIntersectionRecord None
        {
            get
            {
                return new RayEllipseIntersectionRecord { nIntersections = 0 };
            }
        }

        public static RayEllipseIntersectionRecord Single( float t, Vector3f p, float theta, Vector3f closestPointOnEllipse )
        {
            return new RayEllipseIntersectionRecord
            {
                nIntersections = 1,
                t = t,
                p = p,
                
                theta = theta,
                closestPointOnEllipse = closestPointOnEllipse
            };
        }

        public float theta;
        public Vector3f closestPointOnEllipse;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public class RayTriangleIntersectionRecord : RayIntersectionRecord
    {
        public static new RayTriangleIntersectionRecord None
        {
            get
            {
                return new RayTriangleIntersectionRecord { nIntersections = 0 };
            }
        }

        public static RayTriangleIntersectionRecord Single( float t, Vector3f p, Vector3f barycentrics )
        {
            return new RayTriangleIntersectionRecord
            {
                nIntersections = 1,
                t = t,
                p = p,
                barycentrics = barycentrics
            };
        }

        public Vector3f barycentrics;
    }
}

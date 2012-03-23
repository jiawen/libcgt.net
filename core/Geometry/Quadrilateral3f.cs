using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    [Serializable]
    public class Quadrilateral3f
    {
        public Vector3f v00;
        public Vector3f v10;
        public Vector3f v01;
        public Vector3f v11;

        public RayIntersectionRecord IntersectRay( Vector3f origin, Vector3f direction )
        {
            var r0 = TriangleIntersection.IntersectRay( origin, direction, v00, v10, v01 );
            if( r0.Intersected )
            {
                return RayIntersectionRecord.Single( r0.t, r0.p );
            }

            var r1 = TriangleIntersection.IntersectRay( origin, direction, v01, v10, v11 );
            if( r1.Intersected )
            {
                return RayIntersectionRecord.Single( r1.t, r1.p );
            }

            return RayIntersectionRecord.None;
        }

        public Vector3f Normal
        {
            get
            {
                return Vector3f.Cross( v10 - v00, v01 - v00 ).Normalized();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    /// <summary>
    /// A record of the intersection between a ray and any object
    /// 
    /// If nIntersections == 1, then t and p are populated, but the Lists are not
    /// Otherwise, the lists are populated, and t and p are filled with the first non-negative hit.
    /// </summary>
    public class RayIntersectionRecord
    {
        public static RayIntersectionRecord None
        {
            get
            {
                return new RayIntersectionRecord { nIntersections = 0 };
            }
        }

        public static RayIntersectionRecord Single( float t, Vector3f p )
        {
            return new RayIntersectionRecord
            {
                nIntersections = 1,
                t = t,
                p = p
            };
        }

        public int nIntersections;
        public float t;
        public Vector3f p;

        public List< float > intersectionParameters;
        public List< Vector3f > intersectionPoints;

        public bool Intersected
        {
            get
            {
                return nIntersections > 0;
            }
        }
    }
}

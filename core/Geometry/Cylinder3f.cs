using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public class Cylinder3f
    {
        public Vector3f origin;
        public Vector3f axis;
        public float radius;
        public float height;
        
        public Cylinder3f( Vector3f origin, Vector3f axis, float radius, float height )
        {
            this.origin = origin;
            this.axis = axis;
            this.radius = radius;
            this.height = height;
        }

        /*
        public bool IntersectRay( Vector3f rayOrigin, Vector3f rayDirection, out float t, out Vector3f intersectionPoint )
        {
            
        }
        */
    }
}

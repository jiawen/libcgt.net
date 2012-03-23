using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    /// <summary>
    /// A (hyper)-plane in 4D, defined by Ax + By + Cz + Dw + E = 0
    /// </summary>
    [Serializable]
    public class Plane4f
    {
        public float a;
        public float b;
        public float c;
        public float d;
        public float e;

        /// <summary>
        /// Construct a plane directly from its coefficients
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public Plane4f( float a, float b, float c, float d, float e )
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
        }

        // TODO: from 4 points?

        /// <summary>
        /// Construct a plane given a point on the plane and its normal (does not need to be unit length)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="normal"></param>
        public Plane4f( Vector4f p, Vector4f normal )
        {
            var unitNormal = normal.Normalized();
            e = -Vector4f.Dot( unitNormal, p );

            a = unitNormal.x;
            b = unitNormal.y;
            c = unitNormal.z;
            d = unitNormal.w;
        }

        public Plane4f( Plane4f p )
        {
            this.a = p.a;
            this.b = p.b;
            this.c = p.c;
            this.d = p.d;
            this.e = p.e;
        }

        public Vector4f Normal
        {
            get
            {
                return new Vector4f( a, b, c, d );
            }
        }

        public Vector4f UnitNormal
        {
            get
            {
                return Normal.Normalized();
            }
        }        

        /// <summary>
        /// Projects the point p to the point q closest to p on the plane.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector4f ClosestPointOnPlane( Vector4f p )
        {
            float d = Distance( p );
            return( p - d * UnitNormal );
        }

        /// <summary>
        /// Returns the *signed* shortest distance between p and the plane.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public float Distance( Vector4f p )
        {
            // pick a point x on the plane
            // get the vector x --> p
            // distance is the projection on the unit normal
            // xp dot unitNormal
            var x = PointOnPlane();
            var xp = p - x;
            return Vector4f.Dot( xp, UnitNormal );
        }

        /// <summary>
        /// Returns the point on the plane closest to the origin
        /// http://en.wikipedia.org/wiki/Point_on_plane_closest_to_origin 
        /// </summary>
        /// <returns></returns>
        public Vector4f PointOnPlane()
        {
            float den = a * a + b * b + c * c + d * d;
            float rcpDen = 1.0f / den;
            return new Vector4f( -a * e * rcpDen, -b * e * rcpDen, -c * e * rcpDen, -d * e * rcpDen );
        }

#if false
        /// <summary>
        /// Returns an orthonormal basis u, v, and n on the plane
        /// given a preferred u vector.  If u is not on the plane,
        /// then u is projected onto the plane first
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public Matrix3f Basis( Vector3f u )
        {
            // normalize u first
            u = u.Normalized();
            var n = UnitNormal;

            // u is the vector triple product: ( n x u ) x n
            //
            // u is the preferred direction, which may be be in the plane
            // we want u to be the projection of u in the plane
            // and v to be perpendicular to both
            // but v is n cross u regardless of whether u is in the plane or not
            // so compute v, then cross v with n to get u in the plane
            var v = Vector3f.Cross( n, u ).Normalized();
            u = Vector3f.Cross( v, n ).Normalized();

            return new Matrix3f( u, v, n );
        }

        // TODO: use GeometryUtils
        /// <summary>
        /// Returns an orthonormal basis u, v, n on the plane        
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="n"></param>
        public Matrix3f Basis()
        {
            var n = UnitNormal;
            var u = n.OrthogonalVector().Normalized();
            var v = Vector3f.Cross( n, u ).Normalized();

            return new Matrix3f( u, v, n );
        }
#endif

        // TODO:
        // give a preferred vector, will do the best it can to find u and v
        // public void Basis

        /// <summary>
        /// Returns the same plane, but with its normal flipped
        /// </summary>
        /// <returns></returns>
        public Plane4f Flipped()
        {
            return new Plane4f( -a, -b, -c, -d, -e );
        }

        /// <summary>
        /// Returns a plane parallel this this at distance z in the direction of the normal
        /// </summary>
        /// <returns></returns>
        public Plane4f Offset( float z )
        {
            return new Plane4f( a, b, c, d, e - z * ( a + b + c + d ) );
        }

#if false
        // TODO: set an epsilon for vd = 0
        /// <summary>
        /// Given a ray defined by origin + t * direction
        /// (direction need not be normalized)
        /// returns true if the ray hits the plane, along with the appropriate t and intersection point (t can be negative)
        /// and false if the ray misses the plane
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="intersectionT"></param>
        /// <param name="intersectionPoint"></param>
        /// <returns></returns>
        public RayIntersectionRecord IntersectRay( Vector3f origin, Vector3f direction )
        {
            var unitNormal = Normal.Normalized();
            var vd = Vector3f.Dot( unitNormal, direction );

            // ray is parallel to plane
            if ( vd == 0 )
            {
                return RayIntersectionRecord.None;
            }

            var v0 = -( Vector3f.Dot( unitNormal, origin ) + D );
            float t = v0 / vd;

            var p = origin + t * direction;
            return RayIntersectionRecord.Single( t, p );
        }

        // TODO
        /// <summary>
        /// Returns the 2d plane of intersection between two hyperplanes:
        /// http://mathworld.wolfram.com/Plane-PlaneIntersection.html
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static bool Intersection( Plane3f p0, Plane3f p1 )
        {
            var n0 = p0.UnitNormal;
            var n1 = p1.UnitNormal;
            var n2 = Vector3f.Cross( n0, n1 );
            if( n2.Equals( Vector3f.Zero ) )
            {
                return false;
            }
        }
#endif

        public static float CosineDihedralAngle( Plane3f p0, Plane3f p1 )
        {
            return Vector3f.Dot( p0.UnitNormal, p1.UnitNormal );
        }
    }
}

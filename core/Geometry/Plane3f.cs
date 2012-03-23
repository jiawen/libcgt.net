using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    /// <summary>
    /// A plane in 3D, defined by Ax + By + Cz + D = 0
    /// </summary>
    [Serializable]
    public class Plane3f
    {
        /// <summary>
        /// The XY plane, with the normal pointing up (0,0,1)
        /// </summary>
        public static Plane3f XY = new Plane3f( 0, 0, 1, 0 );
        
        /// <summary>
        /// The YZ plane, with the normal pointing out of the page (1,0,0)
        /// </summary>
        public static Plane3f YZ = new Plane3f( 1, 0, 0, 0 );

        /// <summary>
        /// The ZX plane, with the normal pointing right (0,1,0)
        /// </summary>
        public static Plane3f ZX = new Plane3f( 0, 1, 0, 0 );

        public Vector4f ABCD;

        /// <summary>
        /// Construct a plane directly from its coefficients
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public Plane3f( float a, float b, float c, float d )
        {
            ABCD = new Vector4f( a, b, c, d );
        }

        public Plane3f( Vector4f abcd )
        {
            ABCD = abcd;
        }

        /// <summary>
        /// Construct a plane given 3 points
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Plane3f( Vector3f p0, Vector3f p1, Vector3f p2 )
        {
            var unitNormal = Vector3f.Cross( p1 - p0, p2 - p0 ).Normalized();
            var d = -Vector3f.Dot( unitNormal, p0 );
            ABCD = new Vector4f( unitNormal, d );
        }

        /// <summary>
        /// Construct a plane given a point on the plane and its normal (does not need to be unit length)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="normal"></param>
        public Plane3f( Vector3f p, Vector3f normal )
        {
            var unitNormal = normal.Normalized();
            var d = -Vector3f.Dot( unitNormal, p );
            ABCD = new Vector4f( unitNormal, d );
        }

        public Plane3f( Plane3f p )
        {
            ABCD = p.ABCD;
        }

        public Vector3f Normal
        {
            get
            {
                return ABCD.XYZ;
            }
        }

        public Vector3f UnitNormal
        {
            get
            {
                return Normal.Normalized();
            }
        }

        public float D
        {
            get
            {
                return ABCD.w;
            }
            set
            {
                ABCD.w = value;
            }
        }

        /// <summary>
        /// Projects the point p to the point q closest to p on the plane.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3f ClosestPointOnPlane( Vector3f p )
        {
            float d = Distance( p );
            return ( p - d * UnitNormal );
        }

        /// <summary>
        /// Returns the *signed* shortest distance between p and the plane.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public float Distance( Vector3f p )
        {
            // pick a point x on the plane
            // get the vector x --> p
            // distance is the projection on the unit normal
            // xp dot unitNormal
            var x = PointOnPlane();
            var xp = p - x;
            return Vector3f.Dot( xp, UnitNormal );
        }

        /// <summary>
        /// Returns the point on the plane closest to the origin
        /// http://en.wikipedia.org/wiki/Point_on_plane_closest_to_origin 
        /// </summary>
        /// <returns></returns>
        public Vector3f PointOnPlane()
        {
            float a = ABCD.x;
            float b = ABCD.y;
            float c = ABCD.z;
            float d = ABCD.w;

            float den = a * a + b * b + c * c;
            return new Vector3f( -a * d / den, -b * d / den, -c * d / den );
        }

#if false
        // TODO: use epsilon
        public Vector3f PointOnPlane()
        {
            if( ABCD.Z == 0 ) // c is 0, we have ax + by + d = 0
            {
                if( ABCD.Y == 0 )
                {
                    // b is 0, we have ax + d = 0
                    // x = -d / a
                    // any y and z will satisfy, choose y = z = 1
                    return new Vector3f( -ABCD.W / ABCD.X, 1, 1 );
                }
                else
                {
                    // choose x = 1
                    // a + by + d = 0
                    // by = -a - d
                    // y = ( -a - d ) / b
                    // and any z will satisfy, choose z = 1
                    return new Vector3f( 1, ( -ABCD.X - ABCD.W ) / ABCD.Y, 1 );
                }
            }
            else // c is not 0
            {                
                // pick a point on the plane, choose x = y = 1:
                // a + b + c * z + d = 0
                // cz = -a - b - d
                // z = ( -a - b - d ) / c
                return new Vector3f( 1, 1, ( -ABCD.X - ABCD.Y - ABCD.W ) / ABCD.Z );
            }            
        }
#endif

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

        // TODO:
        // give a preferred vector, will do the best it can to find u and v
        // public void Basis

        /// <summary>
        /// Returns the same plane, but with its normal flipped
        /// </summary>
        /// <returns></returns>
        public Plane3f Flipped()
        {
            return new Plane3f( new Vector4f( -ABCD ) );
        }

        /// <summary>
        /// Returns a plane parallel this this at distance z in the direction of the normal
        /// </summary>
        /// <returns></returns>
        public Plane3f Offset( float z )
        {
            float a = ABCD.x;
            float b = ABCD.y;
            float c = ABCD.z;
            float d = ABCD.w;

            return new Plane3f( a, b, c, d - z * ( a + b + c ) );
        }

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
            if( vd == 0 )
            {
                return RayIntersectionRecord.None;
            }

            var v0 = -( Vector3f.Dot( unitNormal, origin ) + D );
            float t = v0 / vd;

            var p = origin + t * direction;
            return RayIntersectionRecord.Single( t, p );
        }

        public static float CosineDihedralAngle( Plane3f p0, Plane3f p1 )
        {
            return Vector3f.Dot( p0.UnitNormal, p1.UnitNormal );
        }
    }
}

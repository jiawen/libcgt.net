using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Returns true if p is inside the unit square
        /// (including the boundary).
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool InsideUnitSquare( Vector2f p )
        {
            return( p.x >= 0 && p.y >= 0 && p.x <= 1 && p.y <= 1 );
        }

        /// <summary>
        /// Returns the *centroid* of a set of points in 3D
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Vector3f Centroid( IEnumerable< Vector3f > points )
        {
            int n = 0;
            var sum = Vector3f.Zero;
            foreach( var p in points )
            {
                ++n;
                sum += p;
            }
            return sum / n;
        }

        public static Vector2f RectangularToPolar( Vector2f xy )
        {
            if( xy.Equals( Vector2f.Zero ) )
            {
                return Vector2f.Zero;
            }

            float r = xy.Norm();
            float theta = ( float ) ( Math.Atan2( xy.y, xy.x ) );
            return new Vector2f( r, theta );
        }

        public static Vector2f PolarToRectangular( Vector2f rt )
        {
            if( rt.Equals( Vector2f.Zero ) )
            {
                return Vector2f.Zero;
            }

            float r = rt.x;
            float theta = rt.y;

            float ct = ( float ) ( Math.Cos( theta ) );
            float st = ( float ) ( Math.Sin( theta ) );

            float x = r * ct;
            float y = r * st;

            return new Vector2f( x, y );
        }

        /// <summary>
        /// Returns the (math version of) spherical coordinates given x, y, z in rectangular coordinates.
        /// theta is in [-pi, pi]
        /// phi is in [0, pi]
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public static Vector3f RectangularToSpherical( Vector3f xyz )
        {
            if( xyz.Equals( Vector3f.Zero ) )
            {
                return Vector3f.Zero;
            }

            float rho = xyz.Norm();
            
            // yes, use inverse cosine, since phi has a range between 0 and pi
            float cosPhi = xyz.z / rho;
            float phi = MathUtils.Acos( cosPhi );
            
            float sinPhi = MathUtils.Sin( phi );
            float rcpRhoSinPhi = 1.0f / ( rho * sinPhi );

            // use atan2 to get the full [-pi, pi] range for theta
            float theta = MathUtils.Atan2( xyz.y * rcpRhoSinPhi, xyz.x * rcpRhoSinPhi );

            return new Vector3f( rho, theta, phi );
        }

        /// <summary>
        /// Returns rectangular coordinates given the (math version of) spherical coordinates rho, theta, and phi        
        /// </summary>
        /// <param name="rtp"></param>
        /// <returns></returns>
        public static Vector3f SphericalToRectangular( Vector3f rtp )
        {
            float rho = rtp.x;
            if( rho.Equals( Vector3f.Zero ) )
            {
                return Vector3f.Zero;
            }

            float theta = rtp.y;
            float phi = rtp.z;

            float cp = MathUtils.Cos( phi );
            float sp = MathUtils.Sin( phi );
            float ct = MathUtils.Cos( theta );
            float st = MathUtils.Sin( theta );

            float x = rho * ct * sp;
            float y = rho * st * sp;
            float z = rho * cp;

            return new Vector3f( x, y, z );
        }

        public static void Basis( Vector3f normal, out Vector3f b0, out Vector3f b1 )
        {
            var n = normal.Normalized();
            b0 = n.OrthogonalVector().Normalized();
            b1 = Vector3f.Cross( n, b0 ).Normalized();
        }

        public static void Basis( Vector4f n, out Vector4f b0, out Vector4f b1, out Vector4f b2 )
        {
            // Handle the nearly 0 vector case
            if( n.NormSquared() < 1e-8f )
            {
                b0 = new Vector4f( 1, 0, 0, 0 );
                b1 = new Vector4f( 0, 1, 0, 0 );
                b2 = new Vector4f( 0, 0, 1, 0 );
                
                return;
            }

            Vector4f u = Vector4f.Zero;
            Vector4f v = Vector4f.Zero;

            // look for the two smallest components of n and set them to 1
            float anx = Math.Abs( n.x );
            float any = Math.Abs( n.y );
            float anz = Math.Abs( n.z );
            float anw = Math.Abs( n.w );

            // if x is the smallest
            if( ( anx <= any ) && ( anx <= anz ) && ( anx <= anw ) )
            {
                u = new Vector4f( 1, 0, 0, 0 );

                // look for the second smallest
                if( ( any <= anz ) && ( any <= anw ) )
                {
                    v = new Vector4f( 0, 1, 0, 0 );
                }
                else if( anz <= anw )
                {
                    v = new Vector4f( 0, 0, 1, 0 );
                }
                else
                {
                    v = new Vector4f( 0, 0, 0, 1 );
                }
            }
            // x isn't the smallest
            // y is the smallest
            else if( ( any <= anz ) && ( any <= anw ) )
            {
                u = new Vector4f( 0, 1, 0, 0 );
                
                // look for the second smallest
                if ( ( anx <= anz ) && ( anx <= anw ) )
                {
                    v = new Vector4f( 1, 0, 0, 0 );
                }
                else if ( anx <= anw )
                {
                    v = new Vector4f( 0, 0, 1, 0 );
                }
                else
                {
                    v = new Vector4f( 0, 0, 0, 1 );
                }
            }
            // x and y aren't the smallest either
            // z is the smallest
            else if( anz <= anw )
            {
                u = new Vector4f( 0, 0, 1, 0 );

                // look for the second smallest
                if ( ( anx <= any ) && ( anx <= anw ) )
                {
                    v = new Vector4f( 1, 0, 0, 0 );
                }
                else if ( any <= anw )
                {
                    v = new Vector4f( 0, 1, 0, 0 );
                }
                else
                {
                    v = new Vector4f( 0, 0, 0, 1 );
                }
            }
            // ok, w is the smallest
            else
            {
                u = new Vector4f( 0, 0, 0, 1 );

                // look for the second smallest
                if ( ( anx <= any ) && ( anx <= anz ) )
                {
                    v = new Vector4f( 1, 0, 0, 0 );
                }
                else if ( any <= anz )
                {
                    v = new Vector4f( 0, 1, 0, 0 );
                }
                else
                {
                    v = new Vector4f( 0, 0, 1, 0 );
                }
            }

            // the first basis vector
            b0 = Vector4f.Cross( n, u, v ).Normalized();

            // the second basis vector
            b1 = Vector4f.Cross( n, b0, v ).Normalized();

            // the third basis vector
            b2 = Vector4f.Cross( n, b0, b1 ).Normalized();
        }
    }
}

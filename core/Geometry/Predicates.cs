using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class Predicates
    {
        // Returns a positive number of p is on the "left" of the ray a --> b
        public static float Orientation( Vector2f p, Vector2f a, Vector2f b )
        {
            Vector2f ab = b - a;
            Vector2f ap = p - a;

            Vector3f cp = Vector2f.Cross( ab, ap );

            return cp.z;
        }

        public static bool PointInsideTriangle( Vector2f p, Vector2f[] vertices )
        {
            return PointInsideTriangle( p, vertices[ 0 ], vertices[ 1 ], vertices[ 2 ] );
        }

        public static bool PointInsideTriangle( Vector2f p, Vector2f t0, Vector2f t1, Vector2f t2 )
        {
            float e0 = Orientation( p, t0, t1 );
            float e1 = Orientation( p, t1, t2 );
            float e2 = Orientation( p, t2, t0 );

            return ( e0 > 0 && e1 > 0 && e2 > 0 ) || ( e0 < 0 && e1 < 0 && e2 < 0 );
        }

        /*
         * int pnpoly(int npol, float *xp, float *yp, float x, float y)
    {
      int i, j, c = 0;
      for (i = 0, j = npol-1; i < npol; j = i++) {
        if ((((yp[i] <= y) && (y < yp[j])) ||
             ((yp[j] <= y) && (y < yp[i]))) &&
            (x < (xp[j] - xp[i]) * (y - yp[i]) / (yp[j] - yp[i]) + xp[i]))
          c = !c;
      }
      return c;
    }
         */
        
        /// <summary>
        /// From: http://local.wasp.uwa.edu.au/~pbourke/geometry/insidepoly/
        /// </summary>
        /// <param name="p"></param>
        /// <param name="polygon"></param>
        /// <returns></returns> 
        public static bool PointInsidePolygon( Vector2f p, params Vector2f[] polygon )
        {
            int i;
            int j;
            bool counter = false;

            int npol = polygon.Length;

            for( i = 0, j = npol - 1; i < npol; j = i++ )
            {
                if( ( ( ( polygon[ i ].y <= p.y ) && ( p.y < polygon[ j ].y ) ) ||
                    ( ( polygon[ j ].y <= p.y ) && ( p.y < polygon[ i ].y ) ) ) &&
                    ( p.x < ( polygon[ j ].x - polygon[ i ].x ) * ( p.y - polygon[ i ].y ) / ( polygon[ j ].y - polygon[ i ].y ) + polygon[ i ].x ) )
                {
                    counter = !counter;                    
                }
            }

            return counter;
        }

        // TODO: clean this up, this is a huge mess
        // Intersection namespace, with one for each pair?
        public static bool HyperplaneHypercubeIntersection( Vector4f[] hcVertices,
            Plane4f plane )
        {
            bool foundNegative = false;
            bool foundPositive = false;

            for ( int i = 0; i < 16; ++i )
            {
                float val = plane.Distance( hcVertices[ i ] );

                if ( val < 0 )
                {
                    foundNegative = true;
                }
                else if ( val > 0 )
                {
                    foundPositive = true;
                }
            }

            return foundNegative && foundPositive;
        }
    }
}

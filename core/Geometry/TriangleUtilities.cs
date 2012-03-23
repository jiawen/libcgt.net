using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class TriangleUtilities
    {
        public static Rect2f BoundingBox( params Vector2f[] vertices )
        {
            if( vertices.Length != 3 )
            {
                throw new ArgumentException( "TriangleUtilities.BoundingBox only operates on triangles." );
            }

            float minX = Math.Min( Math.Min( vertices[ 0 ].x, vertices[ 1 ].x ), vertices[ 2 ].x );
            float minY = Math.Min( Math.Min( vertices[ 0 ].y, vertices[ 1 ].y ), vertices[ 2 ].y );

            float maxX = Math.Max( Math.Max( vertices[ 0 ].x, vertices[ 1 ].x ), vertices[ 2 ].x );
            float maxY = Math.Max( Math.Max( vertices[ 0 ].y, vertices[ 1 ].y ), vertices[ 2 ].y );

            return new Rect2f( minX, minY, maxX - minX, maxY - minY );
        }

        public static Vector3f EuclideanToBarycentric( Vector2f p,
            params Vector2f[] vertices )
        {
            if( vertices.Length != 3 )
            {
                throw new ArgumentException( "EuclideanToBarycentric only operates on triangles." );
            }

            // r = l0 v0 + l1 v1 + l2 v2
	        // this yields the linear system
	        // [ ( x0 - x2 )	( x1 - x2 ) ] [ l0 ] = [ x - x2 ]
	        // [ ( y0 - y2 )	( y1 - y2 ) ] [ l1 ]   [ y - y2 ]
	        // a l = b

            Matrix2f a = new Matrix2f
            (
                vertices[ 0 ].x - vertices[ 2 ].x, vertices[ 1 ].x - vertices[ 2 ].x,
                vertices[ 0 ].y - vertices[ 2 ].y, vertices[ 1 ].y - vertices[ 2 ].y
            );

            // TODO: b = p - vertices[ 2 ];
            Vector2f b = new Vector2f
            (
	            p.x - vertices[ 2 ].x,
	            p.y - vertices[ 2 ].y
            );

            Matrix2f ai = a.Inverse();            
            Vector2f l0l1 = ai * b;
            return new Vector3f( l0l1, 1 - l0l1.x - l0l1.y );
        }

        public static Vector3f EuclideanToBarycentric( Vector3f p, params Vector3f[] vertices )
        {
            // TODO: derivation
            // TODO: integrate it in ray / triangle intersection
            // TODO: look at how uv and barycentrics interact
            // from: http://tom.cs.byu.edu/~455/rt.pdf

            var edge0 = vertices[ 1 ] - vertices[ 0 ];
            var edge1 = vertices[ 2 ] - vertices[ 0 ];
            var denominator = Vector3f.Cross( edge0, edge1 ).Norm();

            var sNum = Vector3f.Cross( vertices[ 1 ] - p, vertices[ 2 ] - p ).Norm();
            var tNum = Vector3f.Cross( vertices[ 2 ] - p, vertices[ 0 ] - p ).Norm();
            var uNum = Vector3f.Cross( vertices[ 0 ] - p, vertices[ 1 ] - p ).Norm();

            return new Vector3f( sNum / denominator, tNum / denominator, uNum / denominator );
        }

        public static Vector2f BarycentricToEuclidean( Vector3f b,
            params Vector2f[] vertices )
        {
            if( vertices.Length != 3 )
            {
                throw new ArgumentException( "BarycentricToEuclidean only operates on triangles." );
            }

            return( b.x * vertices[ 0 ] + b.y * vertices[ 1 ] + b.z * vertices[ 2 ] );
        }

        public static Vector3f BarycentricToEuclidean( Vector3f b,
            params Vector3f[] vertices )
        {
            if( vertices.Length != 3 )
            {
                throw new ArgumentException( "BarycentricToEuclidean only operates on triangles." );
            }

            return( b.x * vertices[ 0 ] + b.y * vertices[ 1 ] + b.z * vertices[ 2 ] );
        }

        public static bool IsCounterClockwise( params Vector2f[] vertices )
        {
            Vector2f e0 = vertices[ 1 ] - vertices[ 0 ];
            Vector2f e1 = vertices[ 2 ] - vertices[ 1 ];

            return Vector2f.Cross( e0, e1 ).z > 0;
        }

        // returns true if flipped
        public static bool FlipIfClockwise( Vector2f[] vertices )
        {
            bool b = !( IsCounterClockwise( vertices ) );
            if( b )
            {
                SimpleAlgorithms.Swap( ref vertices[ 0 ], ref vertices[ 1 ] );
            }
            return b;
        }
    }
}

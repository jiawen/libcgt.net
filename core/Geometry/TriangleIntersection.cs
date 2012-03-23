using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public class TriangleIntersection
    {
        // TODO: clean up the code a bit
        public const float EPSILON = 0.00001f;

        public static RayTriangleIntersectionRecord IntersectRay( Vector3f rayOrigin, Vector3f rayDirection,
            Vector3f v0, Vector3f v1, Vector3f v2 )            
        {
            Vector3f edge1;
	        Vector3f edge2;
	        Vector3f tvec;
	        Vector3f pvec;
	        Vector3f qvec;
            
	        float det;
	        float inv_det;

	        // find vectors for two edges sharing vert0
            edge1 = v1 - v0;
            edge2 = v2 - v0;

	        // begin calculating determinant - also used to calculate U parameter
	        pvec = Vector3f.Cross( rayDirection, edge2 );
	        det = Vector3f.Dot( edge1, pvec );
	
            if( det > -EPSILON && det < EPSILON )
            {
                return RayTriangleIntersectionRecord.None;
            }
            inv_det = 1.0f / det;

            // calculate distance from vert0 to ray origin
            tvec = rayOrigin - v0;

            // calculate U parameter and test bounds
            float u = Vector3f.Dot( tvec, pvec ) * inv_det;
            if( u < 0.0f || u > 1.0f )
            {
                return RayTriangleIntersectionRecord.None;
            }             

            // prepare to test V parameter
            qvec = Vector3f.Cross( tvec, edge1 );

            // calculate V parameter and test bounds
            float v = Vector3f.Dot( rayDirection, qvec ) * inv_det;
            if( v < 0.0f || u + v > 1.0f )
            {
                return RayTriangleIntersectionRecord.None;
            }

            // calculate t, ray intersects triangle
            float t = Vector3f.Dot( edge2, qvec ) * inv_det;
            var p = rayOrigin + t * rayDirection;
            return RayTriangleIntersectionRecord.Single( t, p, new Vector3f( 1 - u - v, u, v ) );
        }

        public struct TriangleSegmentIntersectionRecord
        {
            // whether or not the segment overlaps the triangle
            public bool overlapping;

            // the number of intersections
            // Note that a segment can overlap a triangle
            // without intersecting any edges
            public int nEdgeIntersections;

            public List< int > edgeIndices;
            public List< float > edgeParameters;
            public List< float > segmentParameters;            
        }

        /// <summary>
        /// Returns true if the triangle defined by "vertices"
        /// overlaps the line segment p --> q
        /// A triangle overlaps a line segment if:
        /// 0. the face both p and q
        /// (otherwise, at least one of them is outside)
        /// 1. any of the three edges intersects the line segment
        /// (will return one or two edges)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static bool TriangleLineSegmentOverlap( Vector2f p, Vector2f q, params Vector2f[] vertices )
        {
            float tp;
            float tq;

            if( Predicates.PointInsideTriangle( p, vertices ) && Predicates.PointInsideTriangle( q, vertices ) )
            {
                return true;
            }

            var r0 = LineIntersection.SegmentSegmentIntersection( p, q, vertices[ 0 ], vertices[ 1 ], out tp, out tq );
            if( r0 == LineIntersection.IntersectionResult.INTERESECTING )
            {
                return true;
            }
            
            var r1 = LineIntersection.SegmentSegmentIntersection( p, q, vertices[ 1 ], vertices[ 2 ], out tp, out tq );
            if( r1 == LineIntersection.IntersectionResult.INTERESECTING )
            {
                return true;
            }

            var r2 = LineIntersection.SegmentSegmentIntersection( p, q, vertices[ 2 ], vertices[ 0 ], out tp, out tq );
            if( r2 == LineIntersection.IntersectionResult.INTERESECTING )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a record of an overlap between a line segment p --> q
        /// and a triangle defined by vertices.        
        /// 
        /// A triangle overlaps a line segment if:
        /// 0. the face both p and q
        /// (otherwise, at least one of them is outside)
        /// 1. any of the three edges intersects the line segment
        /// (will return one or two edges)
        /// 
        /// If there are two intersections, they are returned in the order of
        /// p --> q (segmentParameters[ 0 ] < segmentParameters[ 1 ])
        ///         
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static TriangleSegmentIntersectionRecord TriangleLineSegmentOverlaps( Vector2f p, Vector2f q, Vector2f[] vertices )            
        {
            var record = new TriangleSegmentIntersectionRecord
            {
                overlapping = false,
                nEdgeIntersections = 0,

                edgeIndices = new List< int >( 2 ),
                edgeParameters = new List< float >( 2 ),
                segmentParameters = new List< float >( 2 )
            };

            if( Predicates.PointInsideTriangle( p, vertices ) && Predicates.PointInsideTriangle( q, vertices ) )
            {
                record.overlapping = true;

                record.segmentParameters.Add( 0 );
                record.segmentParameters.Add( 1 );
                return record;
            }

            float tp;
            float tq;            

            var r0 = LineIntersection.SegmentSegmentIntersection( p, q, vertices[ 0 ], vertices[ 1 ], out tp, out tq );
            if( r0 == LineIntersection.IntersectionResult.INTERESECTING )
            {
                record.overlapping = true;
                ++( record.nEdgeIntersections );

                record.edgeIndices.Add( 0 );
                record.edgeParameters.Add( tq );
                record.segmentParameters.Add( tp );
            }
            
            var r1 = LineIntersection.SegmentSegmentIntersection( p, q, vertices[ 1 ], vertices[ 2 ], out tp, out tq );
            if( r1 == LineIntersection.IntersectionResult.INTERESECTING )
            {
                record.overlapping = true;
                ++( record.nEdgeIntersections );

                record.edgeIndices.Add( 1 );
                record.edgeParameters.Add( tq );
                record.segmentParameters.Add( tp );
            }

            var r2 = LineIntersection.SegmentSegmentIntersection( p, q, vertices[ 2 ], vertices[ 0 ], out tp, out tq );
            if( r2 == LineIntersection.IntersectionResult.INTERESECTING )
            {
                record.overlapping = true;
                ++( record.nEdgeIntersections );

                record.edgeIndices.Add( 2 );
                record.edgeParameters.Add( tq );
                record.segmentParameters.Add( tp );
            }

            // sort the intersections by segment parameter if necessary
            if( record.overlapping && ( record.nEdgeIntersections > 1 ) )
            {
                if( record.segmentParameters[ 0 ] > record.segmentParameters[ 1 ] )
                {
                    int ei = record.edgeIndices[ 0 ];
                    record.edgeIndices[ 0 ] = record.edgeIndices[ 1 ];
                    record.edgeIndices[ 1 ] = ei;

                    float et = record.edgeParameters[ 0 ];
                    record.edgeParameters[ 0 ] = record.edgeParameters[ 1 ];
                    record.edgeParameters[ 1 ] = et;

                    float st = record.segmentParameters[ 0 ];
                    record.segmentParameters[ 0 ] = record.segmentParameters[ 1 ];
                    record.segmentParameters[ 1 ] = st;
                }
            }

            return record;
        }

        /// <summary>
        /// Separating axes test
        /// Given two triangles in 2D defined by vertices0 and vertices1
        /// Returns true if the two overlap.
        /// </summary>
        /// <param name="vertices0"></param>
        /// <param name="vertices1"></param>
        /// <returns></returns>
        public static bool TriangleTriangleOverlap( Vector2f[] vertices0, Vector2f[] vertices1 )
        {
            TriangleUtilities.FlipIfClockwise( vertices0 );
            TriangleUtilities.FlipIfClockwise( vertices1 );

            int i0;
            int i1;
            Vector2f kDir = Vector2f.Zero;

            // test edges of triangle0 for separation
            for( i0 = 0, i1 = 2; i0 < 3; i1 = i0, i0++ )
            {
                // test axis V0[i1] + t*perp(V0[i0]-V0[i1]), perp(x,y) = (y,-x)
                kDir.x = vertices0[ i0 ].y - vertices0[ i1 ].y;
                kDir.y = vertices0[ i1 ].x - vertices0[ i0 ].x;
                if( WhichSide( vertices1, vertices0[ i1 ], kDir ) > 0 )
                {
                    // triangle1 is entirely on positive side of triangle0 edge
                    return false;
                }
            }

            // test edges of triangle1 for separation
            for( i0 = 0, i1 = 2; i0 < 3; i1 = i0, i0++ )
            {
                // test axis V1[i1] + t*perp(V1[i0]-V1[i1]), perp(x,y) = (y,-x)
                kDir.x = vertices1[ i0 ].y - vertices1[ i1 ].y;
                kDir.y = vertices1[ i1 ].x - vertices1[ i0 ].x;
                if( WhichSide( vertices0, vertices1[ i1 ], kDir ) > 0 )
                {
                    // triangle0 is entirely on positive side of triangle1 edge
                    return false;
                }
            }

            return true;
        }

        public static bool TriangleRectangleOverlap( Vector2f[] triangle, Rect2f rectangle )
        {
            Vector2f[] r0 = new Vector2f[]
                                {
                                    rectangle.Origin,
                                    rectangle.BottomRight,
                                    rectangle.TopRight
                                };

            Vector2f[] r1 = new Vector2f[]
                                {
                                    rectangle.Origin,
                                    rectangle.TopRight,
                                    rectangle.TopLeft
                                };

            bool overlap0 = TriangleTriangleOverlap( triangle, r0 );
            bool overlap1 = TriangleTriangleOverlap( triangle, r1 );

            return overlap0 || overlap1;
        }

        // Stolen from David Eberly
        private static int WhichSide( Vector2f[] v, Vector2f p, Vector2f d )
        {
            // Vertices are projected to the form P+t*D.  Return value is +1 if all
            // t > 0, -1 if all t < 0, 0 otherwise, in which case the line splits the
            // triangle.

            int iPositive = 0;
            int iNegative = 0;
            int iZero = 0;

            for (int i = 0; i < 3; i++)
            {
                float fT = Vector2f.Dot( d, v[i] - p );
                if( fT > 0.0f )
                {
                    iPositive++;
                }
                else if( fT < 0.0f )
                {
                    iNegative++;
                }
                else
                {
                    iZero++;
                }

                if (iPositive > 0 && iNegative > 0)
                {
                    return 0;
                }
            }

            return (iZero == 0 ? (iPositive > 0 ? 1 : -1) : 0);
        }
    }
}

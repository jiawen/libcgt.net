using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    /// <summary>
    /// Slightly modified copy of the code found here:
    /// http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
    /// </summary>
    public static class LineIntersection
    {
        public enum IntersectionResult
	    {
		    PARALLEL,
		    COINCIDENT,
		    NOT_INTERESECTING,
		    INTERESECTING
	    };

        // computes the intersection between lines p0 --> p1 and q0 --> q1
        // returns PARALLEL, COINCIDENT, or INTERSECTING
        // if they are INTERSECTING, tp contains the distance along p0 --> p1 (isectpoint = p0 + tp * ( p1 - p0 ) )
        // similarly for tq
        public static IntersectionResult LineLineIntersection( Vector2f p0, Vector2f p1,
            Vector2f q0, Vector2f q1 )            
        {
            float tp;
            float tq;

            return LineLineIntersection( p0, p1, q0, q1, out tp, out tq );
        }

        // computes the intersection between lines p0 --> p1 and q0 --> q1
	    // returns PARALLEL, COINCIDENT, or INTERSECTING
	    // if they are INTERSECTING, tp contains the distance along p0 --> p1 (isectpoint = p0 + tp * ( p1 - p0 ) )
	    // similarly for tq
	    public static IntersectionResult LineLineIntersection( Vector2f p0, Vector2f p1,
            Vector2f q0, Vector2f q1,
            out float tp, out float tq )
	    {
            float denom = ( q1.y - q0.y ) * ( p1.x - p0.x ) - ( q1.x - q0.x ) * ( p1.y - p0.y );
	        float nume_a = ( q1.x - q0.x ) * ( p0.y - q0.y ) - ( q1.y - q0.y ) * ( p0.x - q0.x );
	        float nume_b = ( p1.x - p0.x ) * ( p0.y - q0.y ) - ( p1.y - p0.y ) * ( p0.x - q0.x );

	        if( denom == 0.0f )
	        {
	            tp = float.NaN;
                tq = float.NaN;
		        if( nume_a == 0.0f && nume_b == 0.0f )
		        {
			        return IntersectionResult.COINCIDENT;
		        }
		        return IntersectionResult.PARALLEL;
	        }

	        tp = nume_a / denom;
	        tq = nume_b / denom;

	        return IntersectionResult.INTERESECTING;
	    }

        public static IntersectionResult LineLineIntersection( Vector2f p0, Vector2f p1,
            Vector2f q0, Vector2f q1,
            out Vector2f intersectionPoint )
        {
            float tp;
            float tq;
            intersectionPoint = Vector2f.Zero;

            var result = LineLineIntersection( p0, p1, q0, q1, out tp, out tq );
            if( result == IntersectionResult.INTERESECTING )
            {
                intersectionPoint.x = p0.x + tp * ( p1.x - p0.x );
                intersectionPoint.y = p0.y + tp * ( p1.y - p0.y );
            }
            return result;
        }

        public static IntersectionResult SegmentSegmentIntersection( Vector2f p0, Vector2f p1,
            Vector2f q0, Vector2f q1 )
        {
            float tp;
            float tq;

            return SegmentSegmentIntersection( p0, p1, q0, q1, out tp, out tq );
        }

        /// <summary>
        /// Computes the intersection between two line segments (of finite length) p0 --> p1 and q0 --> q1
        /// returns INTERSECTING or NOT_INTERSECTING
        /// if they are INTERSECTING, tp and tq contain the linear parameter along the directed segments
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="q0"></param>
        /// <param name="q1"></param>
        /// <param name="tp"></param>
        /// <param name="tq"></param>
        /// <returns></returns>
        public static IntersectionResult SegmentSegmentIntersection( Vector2f p0, Vector2f p1,
            Vector2f q0, Vector2f q1,
            out float tp, out float tq )
        {
            IntersectionResult result = LineLineIntersection( p0, p1, q0, q1, out tp, out tq );
            if( result == IntersectionResult.INTERESECTING )
            {
                if( tp >= 0.0f && tp <= 1.0f && tq >= 0.0f && tq <= 1.0f )
                {
                    return IntersectionResult.INTERESECTING;
                }
            }
            return IntersectionResult.NOT_INTERESECTING;
        }

        /// <summary>
        /// Computes the intersection between two line segments (of finite length) p0 --> p1 and q0 --> q1
        /// returns INTERSECTING or NOT_INTERSECTING
        /// if they are INTERSECTING, intersectionPoint contains the point of intersection
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="q0"></param>
        /// <param name="q1"></param>
        /// <param name="intersectionPoint"></param>
        /// <returns></returns>
        public static IntersectionResult SegmentSegmentIntersection( Vector2f p0, Vector2f p1,
            Vector2f q0, Vector2f q1,
            out Vector2f intersectionPoint )
        {
            float tp;
            float tq;
            intersectionPoint = Vector2f.Zero;

            IntersectionResult result = LineLineIntersection( p0, p1, q0, q1, out tp, out tq );
            if( result == IntersectionResult.INTERESECTING )
            {
                if( tp >= 0.0f && tp <= 1.0f && tq >= 0.0f && tq <= 1.0f )
                {
                    // get the intersection point
                    intersectionPoint.x = p0.x + tp * ( p1.x - p0.x );
                    intersectionPoint.y = p0.y + tp * ( p1.y - p0.y );

                    return IntersectionResult.INTERESECTING;
                }
            }
            return IntersectionResult.NOT_INTERESECTING;
        }

        public static IntersectionResult RaySegmentIntersection( Vector2f rayOrigin, Vector2f rayDirection,
            Vector2f segmentBegin, Vector2f segmentEnd,
            out float tRay, out float tSegment,
            out Vector2f intersectionPoint )
        {
            intersectionPoint = Vector2f.Zero;

	        // first intersect the lines
	        Vector2f p0 = rayOrigin;
	        Vector2f p1 = rayOrigin + rayDirection; // find two points on the ray
	        Vector2f q0 = segmentBegin;
	        Vector2f q1 = segmentEnd;

	        IntersectionResult result = LineLineIntersection( p0, p1, q0, q1, out tRay, out tSegment );
	        if( result == IntersectionResult.INTERESECTING )
	        {
		        if( tSegment >= 0.0f && tSegment <= 1.0f && tRay >= 0.0f )
		        {
			        intersectionPoint.x = q0.x + tSegment * ( q1.x - q0.x );
			        intersectionPoint.y = q0.y + tSegment * ( q1.y - q0.y );
			        return IntersectionResult.INTERESECTING;
		        }
	        }
	        return IntersectionResult.NOT_INTERESECTING;
        }

        public static IntersectionResult RaySegmentIntersection( Vector2f rayOrigin, Vector2f rayDirection,
            Vector2f segmentBegin, Vector2f segmentEnd,            
            out Vector2f intersectionPoint )
        {
            float tRay;
            float tSegment;

            return RaySegmentIntersection( rayOrigin, rayDirection,
                                           segmentBegin, segmentEnd,
                                           out tRay, out tSegment,
                                           out intersectionPoint );
        }

        public static bool LineRectangleIntersection( Vector2f p0, Vector2f p1, Rect2f rect )
        {
            if( rect.Contains( p0 ) || rect.Contains( p1 ) )
            {
                return true;
            }

            if( SegmentSegmentIntersection( p0, p1, rect.Origin, rect.BottomRight ) == IntersectionResult.INTERESECTING )
            {
                return true;
            }
            if( SegmentSegmentIntersection( p0, p1, rect.BottomRight, rect.TopRight ) == IntersectionResult.INTERESECTING )
            {
                return true;
            }
            if( SegmentSegmentIntersection( p0, p1, rect.TopRight, rect.TopLeft ) == IntersectionResult.INTERESECTING )
            {
                return true;
            }
            if( SegmentSegmentIntersection( p0, p1, rect.TopLeft, rect.Origin ) == IntersectionResult.INTERESECTING )
            {
                return true;
            }
            return false;
        }

        // TODO: move this somewhere else
        /// <summary>
        /// Given a directed line given by two points (p0 --> p1) and a test point q
        /// Returns the signed distance from q to the line.
        /// The distance is positive if q is in the positive half-space (to the "left")        
        /// The distance is negative if q is in the negative half-space.
        /// Returns 0 if it's exactly on the line to floating point precision.
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static float DistanceToLine( Vector2f p0, Vector2f p1, Vector2f q )
        {
            var p0p1 = ( p1 - p0 ).Normalized();
            return Vector2f.Cross( p0p1, q - p0 ).z;
        }
    }
}

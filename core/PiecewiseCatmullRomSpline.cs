using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core
{
    [Serializable]
    public class PiecewiseCatmullRomSpline< T > where T : IControlPoint2f
    {
        private List< CatmullRomSpline2f > segments;
        private List< float > splineRanges;

        public PiecewiseCatmullRomSpline( List< T > controlPoints, bool isClosedLoop )
        {
            if( controlPoints.Count < 1 )
            {
                throw new ArgumentException( "Cannot construct Piecewise Catmull-Rom spline with 0 control points" );
            }

            segments = new List< CatmullRomSpline2f >();
            splineRanges = new List< float >();

            // handle closed loops completely separately
            if( isClosedLoop )
            {
                // always returns at least 2 segments
                var controlPointIndicesBySegment = SplitClosedSplineControlPointsAtCorners( controlPoints );

                // handle the 1st segment                
                bool extrapolateLeft;
                bool extrapolateRight;

                var segment0Indices = controlPointIndicesBySegment.First();
                if( controlPoints[ segment0Indices[ 0 ] ].IsCorner )
                {
                    extrapolateLeft = true;
                }
                else
                {
                    segment0Indices.Insert( 0, Arithmetic.Mod( segment0Indices.First() - 1, controlPoints.Count ) );
                    extrapolateLeft = false;
                }
                if( controlPoints[ segment0Indices.Last() ].IsCorner )
                {
                    extrapolateRight = true;
                }
                else
                {
                    segment0Indices.Add( Arithmetic.Mod( segment0Indices.Last() + 1, controlPoints.Count ) );
                    extrapolateRight = false;
                }
                
                segments.Add( CreateCatmullRomSegment( segment0Indices, controlPoints, extrapolateLeft, extrapolateRight ) );

                // handle the middle segments
                for( int i = 1; i < controlPointIndicesBySegment.Count - 1; ++i )
                {
                    segments.Add( CreateCatmullRomSegment( controlPointIndicesBySegment[ i ], controlPoints, true, true ) );
                }

                var segmentKIndices = controlPointIndicesBySegment.Last();
                if( controlPoints[ segmentKIndices[ 0 ] ].IsCorner )
                {
                    extrapolateLeft = true;
                }
                else
                {
                    segmentKIndices.Insert( 0, Arithmetic.Mod( segmentKIndices.First() - 1, controlPoints.Count ) );
                    extrapolateLeft = false;
                }                
                if( controlPoints[ segmentKIndices.Last() ].IsCorner )
                {
                    extrapolateRight = true;
                }
                else
                {
                    segmentKIndices.Add( Arithmetic.Mod( segmentKIndices.Last() + 1, controlPoints.Count ) );
                    extrapolateRight = false;
                }
                
                segments.Add( CreateCatmullRomSegment( segmentKIndices, controlPoints, extrapolateLeft, extrapolateRight ) );
            }
            else // if it's not a closed loop, life is easier
            {
                var controlPointIndicesBySegment = SplitOpenSplineControlPointsAtCorners( controlPoints );
                foreach( var segmentIndices in controlPointIndicesBySegment )
                {
                    segments.Add( CreateCatmullRomSegment( segmentIndices, controlPoints, true, true ) );
                }
            }

#if false

            List< List< int > > controlPointIndicesBySegment = SplitControlPointsAtCorners( controlPoints, isCorners );

            // create the first segment
            if( isClosedLoop )
            {
                // if it's a closed loop, then figure out if we need to extrapolate
                var firstSegmentIndices = controlPointIndicesBySegment.First();
                AddVerticesToFirstSegment( firstSegmentIndices, isCorners );
                if( controlPointIndicesBySegment.Count == 1 )
                {
                    AddVerticesToLastSegment( firstSegmentIndices, isCorners );
                }

                bool extrapolateLeft = isCorners[ 0 ];
                bool extrapolateRight = isCorners[ firstSegmentIndices.Last() ];
                
                segments.Add( CreateCatmullRomSegment( firstSegmentIndices, controlPoints, extrapolateLeft, extrapolateRight ) );
            }
            else
            {
                // otherwise, always extrapolate
                var firstSegmentIndices = controlPointIndicesBySegment.First();
                segments.Add( CreateCatmullRomSegment( firstSegmentIndices, controlPoints, true, true ) );
            }
            
            if( controlPointIndicesBySegment.Count > 1 )
            {
                for( int i = 1; i < controlPointIndicesBySegment.Count - 1; ++i )
                {
                    segments.Add( CreateCatmullRomSegment( controlPointIndicesBySegment[ i ], controlPoints, true, true ) );
                }

                if( isClosedLoop )
                {
                    // create the last segment, extrapolate right only if control point 0 is a corner
                    AddVerticesToLastSegment( controlPointIndicesBySegment.Last(), isCorners );

                    segments.Add( CreateCatmullRomSegment( controlPointIndicesBySegment.Last(), controlPoints, true, isCorners[ 0 ] ) );
                }
                else
                {
                    segments.Add( CreateCatmullRomSegment( controlPointIndicesBySegment.Last(), controlPoints, true, true ) );
                }                
            }
#endif
            for( int i = 0; i < segments.Count; ++i )
            {
                splineRanges.Add( GetSegmentRange( segments[ i ], controlPoints.Count, isClosedLoop ) );
            }
        }

        private List< List< int > > SplitOpenSplineControlPointsAtCorners( List< T > controlPoints )
        {
            int n = controlPoints.Count;
            var controlPointIndicesBySegment = new List< List< int > >();
            
            controlPointIndicesBySegment.Add( new List< int >() );
            for( int i = 0; i < n; ++i )
            {
                // add the control point to the last segment
                controlPointIndicesBySegment.Last().Add( i );

                // if it's a corner, start a new segment
                // and add add the corner to the new segment too
                if( controlPoints[ i ].IsCorner &&
                    ( i != 0 ) &&
                    ( i != n - 1 ) )
                {
                    controlPointIndicesBySegment.Add( new List< int >() );
                    controlPointIndicesBySegment.Last().Add( i );
                }
            }

            return controlPointIndicesBySegment;
        }

        private List< List< int > > SplitClosedSplineControlPointsAtCorners( List< T > controlPoints )
        {
            int n = controlPoints.Count;
            var controlPointIndicesBySegment = new List< List< int > >();
            
            controlPointIndicesBySegment.Add( new List< int >() );
            for( int i = 0; i < n; ++i )
            {
                // add the control point to the last segment
                controlPointIndicesBySegment.Last().Add( i );

                // if it's a corner, start a new segment
                // and add add the corner to the new segment too
                if( controlPoints[ i ].IsCorner && ( i != 0 ) )
                {
                    controlPointIndicesBySegment.Add( new List< int >() );
                    controlPointIndicesBySegment.Last().Add( i );
                }
            }

            // if there are no corners of any kind
            // then force a second segment to cover
            // t from the last control point back to the first
            if( controlPointIndicesBySegment.Count == 1 )
            {
                controlPointIndicesBySegment.Add( new List< int >() );
                controlPointIndicesBySegment.Last().Add( n - 1 );
            }
            controlPointIndicesBySegment.Last().Add( 0 );

            return controlPointIndicesBySegment;
        }

        private void AddVerticesToFirstSegment( List< int > firstSegment, List< bool > isCorners )
        {
            if( !isCorners[ 0 ] )
            {
                firstSegment.Insert( 0, isCorners.Count - 1 );
            }
        }

        private void AddVerticesToLastSegment( List< int > lastSegment, List< bool > isCorners )
        {
            lastSegment.Add( 0 );
            if( !isCorners[ 0 ] )
            {
                lastSegment.Add( 1 );
            }
        }

        private List< Vector2f > GetSegmentControlPoints( List< int > segmentControlPointIndices, List< T > controlPoints )
        {
            int k = segmentControlPointIndices.Count;
            List< Vector2f > segmentControlPoints = new List< Vector2f >( k + 2 );
            foreach( int i in segmentControlPointIndices )
            {
                segmentControlPoints.Add( controlPoints[ i ].Position );
            }

            return segmentControlPoints;
        }

        private float GetSegmentRange( CatmullRomSpline2f segment, int nControlPoints, bool isClosedLoop )
        {
            int k = segment.NumControlPoints;

            float n = isClosedLoop ? nControlPoints : nControlPoints - 1;

            if( k < 4 )
            {
                return ( k - 1 ) / n;
            }
            else
            {
                return ( k - 3 ) / n;
            }
        }

        // TODO: caching evaluator

        public Vector2f EvaluateAt( float t )
        {
            float u;
            var segment = GetSegmentAndParameter( t, out u );
            return segment.EvaluateAt( u );
        }

        public Vector2f TangentAt( float t )
        {
            float u;
            var segment = GetSegmentAndParameter( t, out u );
            return segment.TangentAt( u );
        }

        /// <summary>
        /// Given the global parameter t,
        /// returns the catmull-rom segment to evaluate t
        /// and the local segment parameter u
        /// </summary>
        /// <param name="t"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        private CatmullRomSpline2f GetSegmentAndParameter( float t, out float u )
        {
            u = 0; // initialize
            if( t < 0 )
            {
                u = 0.0f;
                return segments.First();
            }
            else if( t >= 1.0f )
            {
                u = 1.0f;
                return segments.Last();
            }

            // same algorithm as GetSegmentControlPoint actually
            // just need to figure out what the overall spacing is
            // and subtract the right amount of global t from each segment
            // and iterate

            float tt = t;

            for( int s = 0; s < segments.Count; ++s )
            {
                float splineRange = splineRanges[ s ];

                if( tt < splineRange )
                {
                    // rescale the t between 0 and 1
                    u = ( tt / splineRange );
                    return segments[ s ];
                }
                else
                {
                    tt -= splineRange;
                }
            }

            // HACK
            u = ( tt / splineRanges.Last() );
            return segments.Last();
            // throw new ArgumentException( "Cannot get segment parameter: input parameter t is out of range!, t = " + t );
        }

        private CatmullRomSpline2f CreateCatmullRomSegment( List< int > segmentControlPointIndices,
            List< T > controlPoints, bool extrapolateLeft, bool extrapolateRight )
        {
            int k = segmentControlPointIndices.Count;
            var segmentControlPoints = GetSegmentControlPoints( segmentControlPointIndices, controlPoints );

            // if we have exactly 2 control points, then make a linear segment
            if( k == 2 )
            {
                return new CatmullRomSpline2f( segmentControlPoints );
            }
            else
            {
                Vector2f e0 = Vector2f.Zero;
                Vector2f e1 = Vector2f.Zero;
                
                if( extrapolateLeft )
                {
                    e0 = ExtrapolateNextCatmullRomControlPoint( segmentControlPoints[ 2 ], segmentControlPoints[ 1 ], segmentControlPoints[ 0 ] );                    
                }

                if( extrapolateRight )
                {
                    e1 = ExtrapolateNextCatmullRomControlPoint( segmentControlPoints[ k - 3 ], segmentControlPoints[ k - 2 ], segmentControlPoints[ k - 1 ] );
                }

                if( extrapolateLeft )
                {
                    segmentControlPoints.Insert( 0, e0 );
                }

                if( extrapolateRight )
                {
                    segmentControlPoints.Add( e1 );
                }

                return new CatmullRomSpline2f( segmentControlPoints );
            }
        }

        /// <summary>
        /// Given 3 control points, extrapolates the next Catmull-Rom control point position
        /// such that the spline curvature is preserved
        /// </summary>
        /// <param name="cps"></param>
        /// <returns></returns>
        private Vector2f ExtrapolateNextCatmullRomControlPoint( params Vector2f[] cps )
        {
            Vector2f e0 = cps[ 1 ] - cps[ 0 ];
            Vector2f e1 = cps[ 2 ] - cps[ 1 ];

            Vector2f e0n = e0.Normalized();
            Vector2f e1n = e1.Normalized();

            // get the angle between the last two edges e0 and e1
            // and build a rotation matrix rotating e0 into e1
            float cosTheta = Vector2f.Dot( e0n, e1n );
            float sinTheta = Vector2f.Cross( e0n, e1n ).z;

            // rotate e1 by theta
            Matrix2f rotationMatrix = new Matrix2f( cosTheta, -sinTheta, sinTheta, cosTheta );
            Vector2f e2n = rotationMatrix * e1n;

            // scale e2n to be the same length as e1
            Vector2f e2 = e2n * e1.Norm();

            // return the new control point by adding e2 to cps[ 2 ]
            return cps[ 2 ] + e2;
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core
{
    public class Triangulator
    {

#if DEBUG
        //private const string triangleDLL = "triangle-1.6-x64d.dll";
        private const string triangleDLL = "triangle-1.6-x64.dll";
        //private const string triangleDLL = "triangle-1.6.dll";
#else
        //private const string triangleDLL = "triangle-1.6.dll";
        private const string triangleDLL = "triangle-1.6-x64.dll";
#endif

        public static int[] Delaunay( List< Vector2f > positionList )
        {
            double[] pointList = new double[ 2 * positionList.Count ];
            
            for( int i = 0; i < positionList.Count; ++i )
            {
                pointList[ 2 * i ] = positionList[ i ].x;
                pointList[ 2 * i + 1 ] = positionList[ i ].y;
            }

            return Delaunay( pointList );
        }

        public static int[] Delaunay( double[] pointList )
        {
            IntPtr triangleOutput = triangulatePointList( "zcQYY", pointList.Length / 2, pointList );
            
            int nTriangles;
            IntPtr triangleList = getTriangleList( triangleOutput, out nTriangles );

            int[] outputTriangleList = new int[ 3 * nTriangles ];
            Marshal.Copy( triangleList, outputTriangleList, 0, 3 * nTriangles );

            deleteTriangulateIO( triangleOutput );

            return outputTriangleList;
        }

        /*
        public static int[] ConformingDelaunay( List< Vector2f > positionList )
        {
            double[] pointList = new double[ 2 * positionList.Count ];
            
            for( int i = 0; i < positionList.Count; ++i )
            {
                pointList[ 2 * i ] = positionList[ i ].X;
                pointList[ 2 * i + 1 ] = positionList[ i ].Y;
            }

            return ConformingDelaunay( pointList );
        }
        */

        public static int[] ConformingDelaunay( double[] inputPointList, out double[] outputPointList )
        {
            IntPtr triangleOutput = triangulatePointList( "zcqDQ", inputPointList.Length / 2, inputPointList );
            
            int nPoints;
            IntPtr pointList = getPointList( triangleOutput, out nPoints );

            int nTriangles;
            IntPtr triangleList = getTriangleList( triangleOutput, out nTriangles );

            outputPointList = new double[ 2 * nPoints ];
            Marshal.Copy( pointList, outputPointList, 0, 2 * nPoints );

            int[] outputTriangleList = new int[ 3 * nTriangles ];
            Marshal.Copy( triangleList, outputTriangleList, 0, 3 * nTriangles );

            deleteTriangulateIO( triangleOutput );

            return outputTriangleList;
        }

        public static int[] TriangulatePSLG( List< Vector2f > positionList, List< Vector2i > segmentList )
        {
            double[] positionArray = new double[ 2 * positionList.Count ];
            
            for( int i = 0; i < positionList.Count; ++i )
            {
                positionArray[ 2 * i ] = positionList[ i ].x;
                positionArray[ 2 * i + 1 ] = positionList[ i ].y;
            }

            int[] segmentArray = new int[ 2 * segmentList.Count ];
            for( int i = 0; i < segmentList.Count; ++i )
            {
                segmentArray[ 2 * i ] = segmentList[ i ].x;
                segmentArray[ 2 * i + 1 ] = segmentList[ i ].y;
            }

            return TriangulatePSLG( positionArray, segmentArray );
        }

        public static int[] ConstrainedDelaunay( double[] inputPointList, int[] inputSegmentList,
            out double[] outputPointList )
        {
            //IntPtr triangleOutputPtr = triangulatePSLG( "zcpQ", inputPointList.Length / 2, inputPointList, inputSegmentList.Length / 2, inputSegmentList );
            IntPtr triangleOutputPtr = triangulatePSLG( "zpcQYY", inputPointList.Length / 2, inputPointList, inputSegmentList.Length / 2, inputSegmentList );
            
            int nPoints;
            IntPtr outputPointListPtr = getPointList( triangleOutputPtr, out nPoints );

            int nTriangles;
            IntPtr outputTriangleListPtr = getTriangleList( triangleOutputPtr, out nTriangles );

            outputPointList = new double[ 2 * nPoints ];
            Marshal.Copy( outputPointListPtr, outputPointList, 0, 2 * nPoints );

            int[] outputTriangleList = new int[ 3 * nTriangles ];
            Marshal.Copy( outputTriangleListPtr, outputTriangleList, 0, 3 * nTriangles );

            deleteTriangulateIO( triangleOutputPtr );

            return outputTriangleList;
        }

        public static int[] ConstrainedDelaunay( List< Vector2f > inputPointList, List< Vector2i > inputSegmentList,
            out List< Vector2f > outputPointList )
        {
            double[] inputPointArray = new double[ 2 * inputPointList.Count ];
            
            for( int i = 0; i < inputPointList.Count; ++i )
            {
                inputPointArray[ 2 * i ] = inputPointList[ i ].x;
                inputPointArray[ 2 * i + 1 ] = inputPointList[ i ].y;
            }

            int[] inputSegmentArray = new int[ 2 * inputSegmentList.Count ];
            for( int i = 0; i < inputSegmentList.Count; ++i )
            {
                inputSegmentArray[ 2 * i ] = inputSegmentList[ i ].x;
                inputSegmentArray[ 2 * i + 1 ] = inputSegmentList[ i ].y;
            }

            double[] outputPointArray;
            int[] outputTriangleArray = ConstrainedDelaunay( inputPointArray, inputSegmentArray, out outputPointArray );
            outputPointList = new List< Vector2f >( outputPointArray.Length / 2 );
            for( int i = 0; i < outputPointArray.Length / 2; ++i )
            {
                float x = ( float ) ( outputPointArray[ 2 * i ] );
                float y = ( float ) ( outputPointArray[ 2 * i + 1 ] );
                outputPointList.Add( new Vector2f( x, y ) );
            }
            return outputTriangleArray;
        }

        public static int[] TriangulatePSLG( double[] pointList, int[] segmentList )
        {
            // throw new NotImplementedException( "look at this.  used for triangulating holes currently.  not clear this is the right thing.  Y is probably useful for the holes.  YY is probably unnecessary" );
            IntPtr triangleOutput = triangulatePSLG( "zpYY", pointList.Length / 2, pointList, segmentList.Length / 2, segmentList );
            
            int nTriangles;
            IntPtr triangleList = getTriangleList( triangleOutput, out nTriangles );

            int[] outputTriangleList = new int[ 3 * nTriangles ];
            Marshal.Copy( triangleList, outputTriangleList, 0, 3 * nTriangles );

            deleteTriangulateIO( triangleOutput );

            return outputTriangleList;
        }

        public static int[] ConstrainedDelaunayWithAreaConstraint( double[] inputPointList, int[] segmentList,
            double maximumArea,
            out double[] outputPointList )
        {
            var switches = string.Format( "zcpQa{0}", maximumArea );

            IntPtr triangleOutput = triangulatePSLG( switches, inputPointList.Length / 2, inputPointList, segmentList.Length / 2, segmentList );
            
            int nPoints;
            IntPtr pointList = getPointList( triangleOutput, out nPoints );
            
            int nTriangles;
            IntPtr triangleList = getTriangleList( triangleOutput, out nTriangles );

            outputPointList = new double[ 2 * nPoints ];
            Marshal.Copy( pointList, outputPointList, 0, 2 * nPoints );

            int[] outputTriangleList = new int[ 3 * nTriangles ];
            Marshal.Copy( triangleList, outputTriangleList, 0, 3 * nTriangles );

            deleteTriangulateIO( triangleOutput );

            return outputTriangleList;
        }

        public static int[] ConformingConstrainedDelaunay( double[] inputPointList, int[] segmentList,
            out double[] outputPointList )
        {
            IntPtr triangleOutput = triangulatePSLG( "zcpqDQ", inputPointList.Length / 2, inputPointList, segmentList.Length / 2, segmentList );
            
            int nPoints;
            IntPtr pointList = getPointList( triangleOutput, out nPoints );
            
            int nTriangles;
            IntPtr triangleList = getTriangleList( triangleOutput, out nTriangles );

            outputPointList = new double[ 2 * nPoints ];
            Marshal.Copy( pointList, outputPointList, 0, 2 * nPoints );

            int[] outputTriangleList = new int[ 3 * nTriangles ];
            Marshal.Copy( triangleList, outputTriangleList, 0, 3 * nTriangles );

            deleteTriangulateIO( triangleOutput );

            return outputTriangleList;
        }

        public static int[] ConformingConstrainedDelaunayWithAreaConstraint( List< Vector2f > inputPoints, List< Vector2i > inputSegments,
            float maximumArea,
            out List< Vector2f > outputPoints )
        {
            double[] pointList = new double[ 2 * inputPoints.Count ];            
            for( int i = 0; i < inputPoints.Count; ++i )
            {
                pointList[ 2 * i ] = inputPoints[ i ].x;
                pointList[ 2 * i + 1 ] = inputPoints[ i ].y;
            }

            int[] segmentList = new int[ 2 * inputSegments.Count ];
            for( int i = 0; i < inputSegments.Count; ++i )
            {
                segmentList[ 2 * i ] = inputSegments[ i ].x;
                segmentList[ 2 * i + 1 ] = inputSegments[ i ].y;
            }

            double[] outputPointList;
            var outputTriangleList = ConformingConstrainedDelaunayWithAreaConstraint( pointList, segmentList,
                                                                                      maximumArea, out outputPointList );
            outputPoints = new List< Vector2f >( outputPointList.Length / 2 );
            for( int i = 0; i < outputPointList.Length / 2; ++i )
            {
                float x = ( float ) ( outputPointList[ 2 * i ] );
                float y = ( float ) ( outputPointList[ 2 * i + 1 ] );
                outputPoints.Add( new Vector2f( x, y ) );
            }

            return outputTriangleList;
        }

        public static int[] ConformingConstrainedDelaunayWithAreaConstraint( double[] inputPointList, int[] inputSegmentList,
            double maximumArea,
            out double[] outputPointList )
        {
            var switches = string.Format( "zcpqDQa{0}", maximumArea );

            IntPtr triangleOutputPtr = triangulatePSLG( switches, inputPointList.Length / 2, inputPointList, inputSegmentList.Length / 2, inputSegmentList );
            
            int nPoints;
            IntPtr outputPointListPtr = getPointList( triangleOutputPtr, out nPoints );
            
            int nTriangles;
            IntPtr outputTriangleListPtr = getTriangleList( triangleOutputPtr, out nTriangles );

            outputPointList = new double[ 2 * nPoints ];
            Marshal.Copy( outputPointListPtr, outputPointList, 0, 2 * nPoints );

            int[] outputTriangleList = new int[ 3 * nTriangles ];
            Marshal.Copy( outputTriangleListPtr, outputTriangleList, 0, 3 * nTriangles );

            deleteTriangulateIO( triangleOutputPtr );

            return outputTriangleList;
        }

        [DllImport( triangleDLL )]
        private static extern IntPtr triangulatePointList( string triswitches,
            int nPoints, double[] pointList );

        [DllImport( triangleDLL )]
        private static extern IntPtr triangulatePSLG( string triswitches,
            int nPoints, double[] pointList,
            int nSegments, int[] segmentList );

        [DllImport( triangleDLL )]
        private static extern IntPtr getPointList( IntPtr triangulationOutput, out int nPoints );

        [DllImport( triangleDLL )]
        private static extern IntPtr getSegmentList( IntPtr triangulationOutput, out int nSegments );

        [DllImport( triangleDLL )]
        private static extern IntPtr getTriangleList( IntPtr triangulationOutput, out int nTriangles );

        [DllImport( triangleDLL )]
        private static extern void deleteTriangulateIO( IntPtr tio );
    }
}

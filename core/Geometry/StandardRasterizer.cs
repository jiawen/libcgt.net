using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class StandardRasterizer
    {
        // TODO: thie edge test is actually cheaper than the cross product based one used in Predicates.cs

        /// <summary>
        /// Standard rasterization edge test:
        /// Given an edge with normal (in the positive half-space)
        /// and an origin
        /// Determines which side p is on.
        /// Returns a number > 0 if it's on the positive half-space (in the direction of the normal)
        /// < 0 if it's on the negative half-space, and 0 if it's exactly on the edge.
        /// </summary>
        /// <param name="edgeNormal">Normal to the edge</param>
        /// <param name="edgeOrigin">Origin of the edge</param>
        /// <param name="p">Point to be tested</param>
        /// <returns>Vector2f.Dot( edgeNormal, p - edgeOrigin)</returns>
        public static float EdgeTest( Vector2f edgeNormal, Vector2f edgeOrigin, Vector2f p )
        {
            return Vector2f.Dot( edgeNormal, ( p - edgeOrigin ) );
        }

        public static List< Vector2f > PixelsInsideTriangle( Vector2f[] vertices )
        {
            return PixelsInsideTriangle( vertices[ 0 ], vertices[ 1 ], vertices[ 2 ] );
        }

        /// <summary>
        /// Given a BitArray, its corresponding rectangle on the screen
        /// and the vertices of a triangle
        /// Sets the entry of the bit array to true if the center of the pixel is inside the triangle
        /// and false otherwise
        /// </summary>
        /// <param name="vertices"></param>
        public static void Rasterize( BitArray mask, Rect2i maskRect, params Vector2f[] vertices )
        {
            // set up edges
            Vector2f e0 = vertices[ 1 ] - vertices[ 0 ];
            Vector2f e1 = vertices[ 2 ] - vertices[ 1 ];
            Vector2f e2 = vertices[ 0 ] - vertices[ 2 ];

            // get edge normals
            Vector2f n0 = e0.OrthogonalVector();
            Vector2f n1 = e1.OrthogonalVector();
            Vector2f n2 = e2.OrthogonalVector();

            int i = 0;
            int x0 = maskRect.Origin.x;
            int y0 = maskRect.Origin.y;
            int sx = maskRect.Width;
            int sy = maskRect.Height;
            for( int y = 0; y < sy; ++y )
            {
                for( int x = 0; x < sx; ++x )
                {
                    Vector2f p = new Vector2f( x0 + x + 0.5f, y0 + y + 0.5f );

                    float t0 = EdgeTest( n0, vertices[ 0 ], p );
                    float t1 = EdgeTest( n1, vertices[ 1 ], p );
                    float t2 = EdgeTest( n2, vertices[ 2 ], p );

                    if( ( ( t0 > 0 ) && ( t1 > 0 ) && ( t2 > 0 ) ) ||
                        ( ( t0 < 0 ) && ( t1 < 0 ) && ( t2 < 0 ) ) )
                    {
                        mask[ i ] = true;
                    }
                    else
                    {
                        // mask[ i ] = false;
                    }

                    ++i;
                }
            }
        }

        // TODO: refactor into a standard rasterizer...
        // only difference is in the edge test
        public static List< Vector2f > PixelsInsideTriangle( Vector2f v0, Vector2f v1, Vector2f v2 )
        {
            // set up edges
            Vector2f e0 = v1 - v0;
            Vector2f e1 = v2 - v1;
            Vector2f e2 = v0 - v2;

            // get edge normals
            Vector2f n0 = e0.OrthogonalVector();
            Vector2f n1 = e1.OrthogonalVector();
            Vector2f n2 = e2.OrthogonalVector();

            // Get the bounding box
            Rect2f bbox = TriangleUtilities.BoundingBox( v0, v1, v2 );
            int xStart = bbox.Origin.x.FloorToInt();
            int xEnd = bbox.TopRight.x.CeilToInt();
            int yStart = bbox.Origin.y.FloorToInt();
            int yEnd = bbox.TopRight.y.CeilToInt();

            // TODO: parallelize
            // need to aggregate into one giant list

            List< Vector2f > pixelsInside = new List< Vector2f >( ( int )( bbox.Area / 3 ) );
            for( int y = yStart; y <= yEnd; ++y )
            {
                for( int x = xStart; x <= xEnd; ++x )
                {
                    Vector2f p = new Vector2f( x + 0.5f, y + 0.5f );

                    float t0 = EdgeTest( n0, v0, p );
                    float t1 = EdgeTest( n1, v1, p );
                    float t2 = EdgeTest( n2, v2, p );

                    if( ( ( t0 > 0 ) && ( t1 > 0 ) && ( t2 > 0 ) ) ||
                        ( ( t0 < 0 ) && ( t1 < 0 ) && ( t2 < 0 ) ) )
                    {
                        pixelsInside.Add( p );
                    }
                }
            }

            return pixelsInside;
        }
    }
}

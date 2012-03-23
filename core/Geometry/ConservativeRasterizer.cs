using System.Collections;
using System.Collections.Generic;
using System.Linq;

using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class ConservativeRasterizer
    {
        /// <summary>
        /// Conservative edge test as in [Akenine-Moller and Aila 2007]
        /// with tileSize set to 1
        /// </summary>        
        /// <param name="edgeNormal">edge normal</param>
        /// <param name="edgeOrigin">edge origin</param>
        /// <param name="p">point to be tested</param>
        /// <returns></returns>
        public static float EdgeTest( Vector2f edgeNormal, Vector2f edgeOrigin, Vector2f p )
        {
            return EdgeTest( 1.0f, edgeNormal, edgeOrigin, p );
        }

        /// <summary>
        /// Conservative edge test as in [Akenine-Moller and Aila 2007]        
        /// </summary>
        /// <param name="tileSize">size of "pixel"</param>
        /// <param name="edgeNormal">edge normal</param>
        /// <param name="edgeOrigin">edge origin</param>
        /// <param name="p">point to be tested</param>
        /// <returns></returns>
        public static float EdgeTest( float tileSize, Vector2f edgeNormal, Vector2f edgeOrigin, Vector2f p )
        {
            // Modify the triangle setup by shifting an offset depending on the edge normal            
            float tx = -0.5f * tileSize;
            float ty = -0.5f * tileSize;

	        if( edgeNormal.x >= 0 )
	        {
		        tx = 0.5f * tileSize;
	        }

	        if( edgeNormal.y >= 0 )
	        {
		        ty = 0.5f * tileSize;
	        }

            var offset = new Vector2f( tx, ty );
            return Vector2f.Dot( edgeNormal, p ) + StandardRasterizer.EdgeTest( edgeNormal, edgeOrigin, offset );
        }

        public static List< Vector2f > PixelsInsideTriangle( Vector2f[] vertices )
        {
            return PixelsInsideTriangle( vertices[ 0 ], vertices[ 1 ], vertices[ 2 ] );
        }

        // BUGBUG: overly conservative: seems to have the same problem at acute angles as triangle offsets
        // BUGBUG: vertices must be specified in ccw order
        public static void Rasterize( float tileSize, BitArray mask, Rect2i maskRect, params Vector2f[] vertices )
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

                    float t0 = EdgeTest( tileSize, n0, vertices[ 0 ], p );
                    float t1 = EdgeTest( tileSize, n1, vertices[ 1 ], p );
                    float t2 = EdgeTest( tileSize, n2, vertices[ 2 ], p );

                    if( ( ( t0 > 0 ) && ( t1 > 0 ) && ( t2 > 0 ) ) ||
                        ( ( t0 < 0 ) && ( t1 < 0 ) && ( t2 < 0 ) ) )
                    {
                        mask[ i ] = true;
                    }
                    else
                    {
                        mask[ i ] = false;
                    }

                    ++i;
                }
            }
        }

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

            // TODO: this is kinda stupid but oh well
            List< Vector2f > bboxPixels = new List< Vector2f >( ( xEnd - xStart + 1 ) * ( yEnd - yStart + 1 ) );
            for( int y = yStart; y <= yEnd; ++y )
            {
                for( int x = xStart; x <= xEnd; ++x )
                {
                    Vector2f p = new Vector2f( x + 0.5f, y + 0.5f );
                    bboxPixels.Add( p );
                }
            }

            return bboxPixels.Where
            (
                p =>
                {
                    float t0 = EdgeTest( n0, v0, p );
                    float t1 = EdgeTest( n1, v1, p );
                    float t2 = EdgeTest( n2, v2, p );

                    return( ( ( t0 > 0 ) && ( t1 > 0 ) && ( t2 > 0 ) ) ||
                        ( ( t0 < 0 ) && ( t1 < 0 ) && ( t2 < 0 ) ) );
                }
            ).ToList();

            /*
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
             * 
             * */
        }

        public static void RasterizeTriangles( IEnumerable< Vector2f[] > triangles, Image4ub image )
        {            
            foreach( Vector2f[] vertices in triangles )
            {
                RasterizeTriangle( vertices, image );
            }
        }

        public static void RasterizeTriangle( Vector2f[] vertices, Image4ub image )
        {
            // set up edges
            Vector2f e0 = vertices[ 1 ] - vertices[ 0 ];
            Vector2f e1 = vertices[ 2 ] - vertices[ 1 ];
            Vector2f e2 = vertices[ 0 ] - vertices[ 2 ];

            // get edge normals
            Vector2f n0 = e0.OrthogonalVector();
            Vector2f n1 = e1.OrthogonalVector();
            Vector2f n2 = e2.OrthogonalVector();

            // Get the bounding box
            Rect2f bbox = TriangleUtilities.BoundingBox( vertices );
            int xStart = bbox.Origin.x.FloorToInt().Clamp( 0, image.Width );
            int xEnd = bbox.TopRight.x.FloorToInt().Clamp( 0, image.Width );
            int yStart = bbox.Origin.y.FloorToInt().Clamp( 0, image.Height );
            int yEnd = bbox.TopRight.y.FloorToInt().Clamp( 0, image.Height );
            
            for( int y = yStart; y <= yEnd; ++y )            
            {
                for( int x = xStart; x <= xEnd; ++x )
                {
                    Vector2f p = new Vector2f( x + 0.5f, y + 0.5f );

                    float t0 = EdgeTest( n0, vertices[ 0 ], p );
                    float t1 = EdgeTest( n1, vertices[ 1 ], p );
                    float t2 = EdgeTest( n2, vertices[ 2 ], p );

                    if( ( ( t0 > 0 ) && ( t1 > 0 ) && ( t2 > 0 ) ) ||
                        ( ( t0 < 0 ) && ( t1 < 0 ) && ( t2 < 0 ) ) )
                    {
                        image[ x, y ] = new Vector4ub( 255, 255, 255, 255 );
                    }
                }
            }
        }
    }
}

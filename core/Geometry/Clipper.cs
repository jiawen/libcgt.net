using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    /// <summary>
    /// Represents a vertex with its attributes (pass in null for interp if no attributes)
    /// Vertex attributes are correctly adjusted when clipped    
    /// </summary>
    public class ClipVertex
    {
        public Vector4f p;
        public Vector4f[] interp;

        public ClipVertex( Vector4f pt )
        {
            p = pt;
        }

        public ClipVertex( Vector4f pt, Vector4f[] i )
        {
            p = pt;
            interp = i;
        }        
    }

    public class Clipper
    {
        private List< Plane3f > clipPlanes;

        public Clipper( IEnumerable< Plane3f > clipPlanes )
        {
            this.clipPlanes = clipPlanes.ToList();
        }

        public List< Vector3f > ClipPolygon( List< Vector3f > input )
        {
            var cv = input.ConvertAll( v => new ClipVertex( new Vector4f( v, 1 ) ) );
            return ClipPolygon( cv ).Select( v => v.p.XYZ ).ToList();
        }

        public List< Vector4f > ClipPolygon( List< Vector4f > input )
        {
            var cv = input.ConvertAll( v => new ClipVertex( v ) );
            return ClipPolygon( cv ).Select( v => v.p ).ToList();
        }

        public List< ClipVertex > ClipPolygon( List< ClipVertex > input )
        {
            var poly1 = new List< ClipVertex >( input );
            var poly2 = new List< ClipVertex >();

            var l1 = poly1;
            var l2 = poly2;

            for( int i = 0; i < clipPlanes.Count; ++i )
            {
                for( int j = 0; j < l1.Count; ++j )
                {
                    Vector4f p1 = l1[ j ].p;
                    Vector4f p2 = l1[ ( j + 1 ) % l1.Count ].p;

                    float dot1 = Vector4f.Dot( p1, clipPlanes[ i ].ABCD );
                    float dot2 = Vector4f.Dot( p2, clipPlanes[ i ].ABCD );

                    // always add points on the inside
                    if( dot1 >= 0.0f )
                        l2.Add( l1[ j ] );

                    // did we cross sides?
                    if( ( dot1 < 0.0f && dot2 > 0.0f ) ||
                        ( dot1 > 0.0f && dot2 < 0.0f ) )
                    {
                        float t = -dot1 / ( dot2 - dot1 );
                        Vector4f p = ( 1.0f - t ) * p1 + t * p2;

                        // do we have stuff to interpolate?
                        if( l1[ j ].interp != null )
                        {
                            // yes, interpolate the arguments
                            float a = 1.0f - t;
                            float b = t;

                            Vector4f[] interp = new Vector4f[l1[ j ].interp.Length];
                            for( int k = 0; k < l1[ j ].interp.Length; ++k )
                                interp[ k ] = a * l1[ j ].interp[ k ] + b * l1[ ( j + 1 ) % l1.Count ].interp[ k ];

                            l2.Add( new ClipVertex( p, interp ) );
                        }
                        else // no, just copy the new vertex over
                        {
                            l2.Add( new ClipVertex( p, null ) );
                        }
                    }
                }

                // degenerate, return empty
                if( l2.Count < 3 )
                    return new List< ClipVertex >();

                if( l1 == poly1 )
                {
                    l1 = poly2;
                    l2 = poly1;
                }
                else
                {
                    l1 = poly1;
                    l2 = poly2;
                }
                l2.Clear();
            }

            return l1;
        }

#if false
        private static bool ClipLine( ref Vector4 p1, ref Vector4 p2 )
        {
            for (int i = 0; i < ClipPlanes.planes.Length; ++i)
            {
                float d1 = Vector4.Dot( p1, ClipPlanes.planes[ i ] );
                float d2 = Vector4.Dot( p2, ClipPlanes.planes[ i ] );

                // totally behind a plane?
                if (d1 < 0.0f && d2 < 0.0f)
                    return false;

                // totally above
                if (d1 >= 0.0f && d2 >= 0.0f)
                    continue;

                // (p1 + t*(p2-p1)) dot plane = 0
                // <=>  t = p1 dot plane / (p2-p1) dot plane;

                float t = -d1 / (d2 - d1);

                Vector4 p = (1.0f - t) * p1 + t * p2;

                if (d1 < 0.0f)
                    p1 = p;
                else
                    p2 = p;
            }

            return true;
        }
        //-------------------------------------------------------------------        

        public static List<ClipVertex> ClipTriangle( Vector4 pw1, Vector4 pw2, Vector4 pw3 )
        {
            return ClipTriangle( pw1, pw2, pw3, null, null, null );
        }
        //-------------------------------------------------------------------

        public static List<ClipVertex> ClipTriangle( Vector4 pw1, Vector4 pw2, Vector4 pw3, Vector4[] i1, Vector4[] i2, Vector4[] i3 )
        {
            List<ClipVertex> Tri = new List<ClipVertex>();
            Tri.Add( new ClipVertex( pw1, i1 ) );
            Tri.Add( new ClipVertex( pw2, i2 ) );
            Tri.Add( new ClipVertex( pw3, i3 ) );
            return ClipPolygon( Tri );
        }
        //-------------------------------------------------------------------

        
#endif
    }
}

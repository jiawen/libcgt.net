using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public class TriangleTriangleIntersection
    {
        private const float EPSILON = 0.000001f;

        public static bool Intersect( Vector3f v0, Vector3f v1, Vector3f v2,
            Vector3f u0, Vector3f u1, Vector3f u2,
            out bool coplanar,
            out Vector3f isectpt1, out Vector3f isectpt2 )
        {
            coplanar = false;
            isectpt1 = Vector3f.Zero;
            isectpt2 = Vector3f.Zero;

            Vector3f e1;
            Vector3f e2;

            Vector3f n1;
            Vector3f n2;

            float d1;
            float d2;

            float du0;
            float du1;
            float du2;

            float dv0;
            float dv1;
            float dv2;

            Vector3f D;

            float[] isect1 = new float[2];
            float[] isect2 = new float[2];

            Vector3f isectpointA1;
            Vector3f isectpointA2;

            Vector3f isectpointB1;
            Vector3f isectpointB2;

            float du0du1;
            float du0du2;

            float dv0dv1;
            float dv0dv2;

            int index;

            float vp0;
            float vp1;
            float vp2;

            float up0;
            float up1;
            float up2;

            float b;
            float c;
            float max;

            float tmp;
            Vector3f diff;

            int smallest1 = 0;
            int smallest2 = 0;

            // compute plane equation of triangle(V0,V1,V2)
            e1 = v1 - v0;
            e2 = v2 - v0;

            n1 = Vector3f.Cross( e1, e2 );
            d1 = -Vector3f.Dot( n1, v0 );
            // plane equation 1: n1 dot x + d1 = 0

            // put u0, u1, u2 into plane equation 1 to compute signed distances to the plane
            du0 = Vector3f.Dot( n1, u0 ) + d1;
            du1 = Vector3f.Dot( n1, u1 ) + d1;
            du2 = Vector3f.Dot( n1, u2 ) + d1;

            // coplanarity robustness check
// TODO: epsilon
// #if USE_EPSILON_TEST==TRUE
            if( Math.Abs( du0 ) < EPSILON )
            {
                du0 = 0.0f;
            }
            if( Math.Abs( du1 ) < EPSILON )
            {
                du1 = 0.0f;
            }
            if( Math.Abs( du2 ) < EPSILON )
            {
                du2 = 0.0f;
            }
// #endif

            du0du1 = du0 * du1;
            du0du2 = du0 * du2;

            if( du0du1 > 0.0f && du0du2 > 0.0f ) // same sign on all of them && not equal 0?
            {
                return false; // no intersection occurs
            }

            // compute plane of triangle ( u0, u1, u2 )
            e1 = u1 - u0;
            e2 = u2 - u0;

            n2 = Vector3f.Cross( e1, e2 );
            d2 = -Vector3f.Dot( n2, u0 );
            // plane equation 2: n2 dot x + d2 = 0

            // put v0, v1, v2 into plane equation 2
            dv0 = Vector3f.Dot( n2, v0 ) + d2;
            dv1 = Vector3f.Dot( n2, v1 ) + d2;
            dv2 = Vector3f.Dot( n2, v2 ) + d2;

// #if USE_EPSILON_TEST==TRUE
            if( Math.Abs( dv0 ) < EPSILON )
            {
                dv0 = 0.0f;
            }
            if( Math.Abs( dv1 ) < EPSILON )
            {
                dv1 = 0.0f;
            }
            if( Math.Abs( dv2 ) < EPSILON )
            {
                dv2 = 0.0f;
            }
// #endif

            dv0dv1 = dv0 * dv1;
            dv0dv2 = dv0 * dv2;

            if( dv0dv1 > 0.0f && dv0dv2 > 0.0f ) // same sign on all of them && not equal 0?
            {
                return false; // no intersection occurs
            }

            // compute direction of intersection line
            D = Vector3f.Cross( n1, n2 );

            // compute and index to the largest component of D
            max = Math.Abs( D.x );
            index = 0;
            b = Math.Abs( D.y );
            c = Math.Abs( D.z );

            if( b > max )
            {
                max = b;
                index = 1;
            }
            
            if( c > max )
            {
                max = c;
                index = 2;
            }

            // this is the simplified projection onto L
            // TODO: indexing operator is slow in C#
            vp0 = v0[ index ];
            vp1 = v1[ index ];
            vp2 = v2[ index ];
  
            up0 = u0[ index ];
            up1 = u1[ index ];
            up2 = u2[ index ];

            // compute interval for triangle 1
            coplanar = compute_intervals_isectline( v0, v1, v2, vp0, vp1, vp2, dv0, dv1, dv2,
                                                    dv0dv1, dv0dv2,
                                                    out isect1[ 0 ], out isect1[ 1 ],
                                                    out isectpointA1, out isectpointA2 );

            if( coplanar )
            {
                return coplanar_tri_tri( n1, v0, v1, v2, u0, u1, u2 );
            }

            // compute interval for triangle 2
            compute_intervals_isectline( u0, u1, u2, up0, up1, up2, du0, du1, du2,
                                         du0du1, du0du2,
                                         out isect2[ 0 ], out isect2[ 1 ],
                                         out isectpointB1, out isectpointB2 );

            sort2( ref isect1[ 0 ], ref isect1[ 1 ], ref smallest1 );
            sort2( ref isect2[ 0 ], ref isect2[ 1 ], ref smallest2 );

            if( isect1[ 1 ] < isect2[ 0 ] || isect2[ 1 ] < isect1[ 0 ] )
            {
                return false;
            }

            // at this point, we know that the triangles intersect
            if( isect2[ 0 ] < isect1[ 0 ] )
            {
                if( smallest1 == 0 )
                {
                    isectpt1 = isectpointA1;
                }
                else
                {
                    isectpt1 = isectpointA2;
                }

                if( isect2[ 1 ] < isect1[ 1 ] )
                {
                    if( smallest2 == 0 )
                    {
                        isectpt2 = isectpointB2;
                    }
                    else
                    {
                        isectpt2 = isectpointB1;
                    }
                }
                else
                {
                    if( smallest1 == 0 )
                    {
                        isectpt2 = isectpointA2;
                    }
                    else
                    {
                        isectpt2 = isectpointA1;
                    }
                }
            }
            else
            {
                if( smallest2 == 0 )
                {
                    isectpt1 = isectpointB1;
                }
                else
                {
                    isectpt1 = isectpointB2;
                }

                if( isect2[ 1 ] > isect1[ 1 ] )
                {
                    if( smallest1 == 0 )
                    {
                        isectpt2 = isectpointA2;
                    }
                    else
                    {
                        isectpt2 = isectpointA1;
                    }
                }
                else
                {
                    if( smallest2 == 0 )
                    {
                        isectpt2 = isectpointB2;
                    }
                    else
                    {
                        isectpt2 = isectpointB1;
                    }
                }
            }
            return true;
        }

        private static void sort2( ref float a, ref float b, ref int smallest )
        {
            if( a > b )
            {
                float c = a;
                a = b;
                b = c;
                smallest = 1;
            }            
            else
            {
                smallest = 0;
            }
        }

        private static bool coplanar_tri_tri( Vector3f n,
            Vector3f v0, Vector3f v1, Vector3f v2, 
            Vector3f u0, Vector3f u1, Vector3f u2 )
        {
            Vector3f A = new Vector3f();
            int i0;
            int i1;
            // first project onto an axis-aligned plane, that maximizes the area
            // of the triangles, compute indices: i0, i1.

            A.x = Math.Abs( n.x );
            A.y = Math.Abs( n.y );
            A.z = Math.Abs( n.z );
            
            if( A.x > A.y )
            {
                if( A.x > A.z )
                {
                    i0 = 1; // A.X is greatest
                    i1 = 2;
                }
                else
                {
                    i0 = 0; // A.Z is greatest
                    i1 = 1;
                }
            }
            else // A.X <= A.Y
            {
                if( A.z > A.y )
                {
                    i0 = 0; // A.Z is greatest
                    i1 = 1;                      
                }
                else
                {
                    i0 = 0; // A.Y is greatest
                    i1 = 2;
                }
            }               
                
            // test all edges of triangle 1 against the edges of triangle 2
            if( TestEdgeAgainstTriangleEdges( v0, v1, u0, u1, u2, i0, i1 ) )
            {
                return true;
            }
            
            if( TestEdgeAgainstTriangleEdges( v1, v2, u0, u1, u2, i0, i1 ) )
            {
                return true;
            }

            if( TestEdgeAgainstTriangleEdges( v2, v0, u0, u1, u2, i0, i1 ) )
            {
                return true;
            }

            // finally, test if tri1 is totally contained in tri2 or vice versa            
            if( PointInTriangle( v0, u0, u1, u2, i0, i1 ) )
            {
                return true;
            }

            if( PointInTriangle( u0, v0, v1, v2, i0, i1 ) )
            {
                return true;
            }

            return false;
        }

        // this edge to edge test is based on Franlin Antonio's gem:
        // "Faster Line Segment Intersection", in Graphics Gems III,
        // pp. 199-202
        private static bool EdgeEdgeTest( Vector3f v0, Vector3f u0, Vector3f u1,
            float Ax, float Ay, int i0, int i1 )
        {
            float Bx, By, Cx, Cy, e, d, f;
            Bx = u0[ i0 ] - u1[ i0 ];
            By = u0[ i1 ] - u1[ i1 ];
            Cx = v0[ i0 ] - u0[ i0 ];
            Cy = v0[ i1 ] - u0[ i1 ];
            f = Ay * Bx - Ax * By;
            d = By * Cx - Bx * Cy;
            if( ( f > 0 && d >= 0 && d <= f ) || ( f < 0 && d <= 0 && d >= f ) )
            {
                e = Ax * Cy - Ay * Cx;
                if( f > 0 )
                {
                    if( e >= 0 && e <= f )
                    {
                        return true;
                    }
                }
                else
                {
                    if( e <= 0 && e >= f )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TestEdgeAgainstTriangleEdges( Vector3f v0, Vector3f v1,
            Vector3f u0, Vector3f u1, Vector3f u2,
            int i0, int i1 )
        {
            float Ax = v1[ i0 ] - v0[ i0 ];
            float Ay = v1[ i1 ] - v0[ i1 ];

            // test edge U0,U1 against V0,V1
            if( EdgeEdgeTest( v0, u0, u1, Ax, Ay, i0, i1 ) )
            {
                return true;
            }

            // test edge U1,U2 against V0,V1
            if( EdgeEdgeTest( v0, u1, u2, Ax, Ay, i0, i1 ) )
            {
                return true;
            }

            // test edge U2,U1 against V0,V1
            if( EdgeEdgeTest( v0, u2, u0, Ax, Ay, i0, i1 ) )
            {
                return true;
            }

            return false;
        }
        
        private static bool PointInTriangle( Vector3f v0, Vector3f u0, Vector3f u1, Vector3f u2, int i0, int i1 )
        {
            // is T1 completly inside T2?
            // check if v0 is inside tri(u0,u1,u2)
            float a = u1[ i1 ] - u0[ i1 ];
            float b = -( u1[ i0 ] - u0[ i0 ] );
            float c = -a * u0[ i0 ] - b * u0[ i1 ];
            float d0 = a * v0[ i0 ] + b * v0[ i1 ] + c;

            a = u2[ i1 ] - u1[ i1 ];
            b = -( u2[ i0 ] - u1[ i0 ] );
            c = -a * u1[ i0 ] - b * u1[ i1 ];
            float d1 = a * v0[ i0 ] + b * v0[ i1 ] + c;

            a = u0[ i1 ] - u2[ i1 ];
            b = -( u0[ i0 ] - u2[ i0 ] );
            c = -a * u2[ i0 ] - b * u2[ i1 ];
            float d2 = a * v0[ i0 ] + b * v0[ i1 ] + c;
            if( d0 * d1 > 0.0 )
            {
                if( d0 * d2 > 0.0 )
                {
                    return true;
                }
            }

            return false;
        }

        private static void isect2( Vector3f VTX0, Vector3f VTX1, Vector3f VTX2,
            float VV0, float VV1, float VV2,
	        float D0, float D1, float D2,
            out float isect0, out float isect1,
            out Vector3f isectpoint0, out Vector3f isectpoint1 )
        {
            float tmp = D0 / ( D0 - D1 );
            Vector3f diff;

            isect0 = VV0 + ( VV1 - VV0 ) * tmp;

            diff = VTX1 - VTX0;
            diff = diff * tmp;
            isectpoint0 = diff + VTX0;

            tmp = D0 / ( D0 - D2 );
            isect1 = VV0 + ( VV2 - VV0 ) * tmp;

            diff = VTX2 - VTX0;
            diff = diff * tmp;
            isectpoint1 = VTX0 + diff;
        }

        private static bool compute_intervals_isectline( Vector3f vert0, Vector3f vert1, Vector3f vert2,
            float VV0,float VV1,float VV2,float D0,float D1,float D2,
            float D0D1, float D0D2,
            out float isect0, out float isect1,
            out Vector3f isectpoint0, out Vector3f isectpoint1 )
        {
            isect0 = 0;
            isect1 = 0;
            isectpoint0 = Vector3f.Zero;
            isectpoint1 = Vector3f.Zero;

            if( D0D1 > 0.0f )
            {
                // here we know that D0D2 <= 0.0
                // that is D0, D1 are on the same side, D2 on the other or on the plane
                isect2( vert2, vert0, vert1, VV2, VV0, VV1, D2, D0, D1,
                        out isect0, out isect1, out isectpoint0, out isectpoint1 );
            }
            else if( D0D2 > 0.0f )
            {
                // here we know that d0d1<=0.0
                isect2( vert1, vert0, vert2, VV1, VV0, VV2, D1, D0, D2,
                        out isect0, out isect1, out isectpoint0, out isectpoint1 );
            }
            else if( D1 * D2 > 0.0f || D0 != 0.0f )
            {
                // here we know that d0d1<=0.0 or that D0!=0.0
                isect2( vert0, vert1, vert2, VV0, VV1, VV2, D0, D1, D2,
                        out isect0, out isect1, out isectpoint0, out isectpoint1 );
            }
            else if( D1 != 0.0f )
            {
                isect2( vert1, vert0, vert2, VV1, VV0, VV2, D1, D0, D2,
                        out isect0, out isect1, out isectpoint0, out isectpoint1 );
            }
            else if( D2 != 0.0f )
            {
                isect2( vert2, vert0, vert1, VV2, VV0, VV1, D2, D0, D1,
                        out isect0, out isect1, out isectpoint0, out isectpoint1 );
            }
            else
            {
                // triangles are coplanar
                return true;
            }

            // not coplanar
            return false;
        }
    }
}

#if false            

  
  SORT2(isect1[0],isect1[1],smallest1);
  SORT2(isect2[0],isect2[1],smallest2);

  if(isect1[1]<isect2[0] || isect2[1]<isect1[0]) return 0;

  /* at this point, we know that the triangles intersect */

  if(isect2[0]<isect1[0])
  {
    if(smallest1==0) { SET(isectpt1,isectpointA1); }
    else { SET(isectpt1,isectpointA2); }

    if(isect2[1]<isect1[1])
    {
      if(smallest2==0) { SET(isectpt2,isectpointB2); }
      else { SET(isectpt2,isectpointB1); }
    }
    else
    {
      if(smallest1==0) { SET(isectpt2,isectpointA2); }
      else { SET(isectpt2,isectpointA1); }
    }
  }
  else
  {
    if(smallest2==0) { SET(isectpt1,isectpointB1); }
    else { SET(isectpt1,isectpointB2); }

    if(isect2[1]>isect1[1])
    {
      if(smallest1==0) { SET(isectpt2,isectpointA2); }
      else { SET(isectpt2,isectpointA1); }      
    }
    else
    {
      if(smallest2==0) { SET(isectpt2,isectpointB2); }
      else { SET(isectpt2,isectpointB1); } 
    }
  }
  return 1;
}
        
#if false    
        int tri_tri_intersect_with_isectline(float V0[3],float V1[3],float V2[3],
				     float U0[3],float U1[3],float U2[3],int *coplanar,
				     float isectpt1[3],float isectpt2[3])
{
  float E1[3],E2[3];
  float N1[3],N2[3],d1,d2;
  float du0,du1,du2,dv0,dv1,dv2;
  float D[3];
  float isect1[2], isect2[2];
  float isectpointA1[3],isectpointA2[3];
  float isectpointB1[3],isectpointB2[3];
  float du0du1,du0du2,dv0dv1,dv0dv2;
  short index;
  float vp0,vp1,vp2;
  float up0,up1,up2;
  float b,c,max;
  float tmp,diff[3];
  int smallest1,smallest2;
  
  /* compute plane equation of triangle(V0,V1,V2) */
  SUB(E1,V1,V0);
  SUB(E2,V2,V0);
  CROSS(N1,E1,E2);
  d1=-DOT(N1,V0);
  /* plane equation 1: N1.X+d1=0 */

  /* put U0,U1,U2 into plane equation 1 to compute signed distances to the plane*/
  du0=DOT(N1,U0)+d1;
  du1=DOT(N1,U1)+d1;
  du2=DOT(N1,U2)+d1;

  /* coplanarity robustness check */
#if USE_EPSILON_TEST==TRUE
  if(fabs(du0)<EPSILON) du0=0.0;
  if(fabs(du1)<EPSILON) du1=0.0;
  if(fabs(du2)<EPSILON) du2=0.0;
#endif
  du0du1=du0*du1;
  du0du2=du0*du2;

  if(du0du1>0.0f && du0du2>0.0f) /* same sign on all of them + not equal 0 ? */
    return 0;                    /* no intersection occurs */

  /* compute plane of triangle (U0,U1,U2) */
  SUB(E1,U1,U0);
  SUB(E2,U2,U0);
  CROSS(N2,E1,E2);
  d2=-DOT(N2,U0);
  /* plane equation 2: N2.X+d2=0 */

  /* put V0,V1,V2 into plane equation 2 */
  dv0=DOT(N2,V0)+d2;
  dv1=DOT(N2,V1)+d2;
  dv2=DOT(N2,V2)+d2;

#if USE_EPSILON_TEST==TRUE
  if(fabs(dv0)<EPSILON) dv0=0.0;
  if(fabs(dv1)<EPSILON) dv1=0.0;
  if(fabs(dv2)<EPSILON) dv2=0.0;
#endif

  dv0dv1=dv0*dv1;
  dv0dv2=dv0*dv2;
        
  if(dv0dv1>0.0f && dv0dv2>0.0f) /* same sign on all of them + not equal 0 ? */
    return 0;                    /* no intersection occurs */

  /* compute direction of intersection line */
  CROSS(D,N1,N2);

  /* compute and index to the largest component of D */
  max=fabs(D[0]);
  index=0;
  b=fabs(D[1]);
  c=fabs(D[2]);
  if(b>max) max=b,index=1;
  if(c>max) max=c,index=2;

  /* this is the simplified projection onto L*/
  vp0=V0[index];
  vp1=V1[index];
  vp2=V2[index];
  
  up0=U0[index];
  up1=U1[index];
  up2=U2[index];

  /* compute interval for triangle 1 */
  *coplanar=compute_intervals_isectline(V0,V1,V2,vp0,vp1,vp2,dv0,dv1,dv2,
				       dv0dv1,dv0dv2,&isect1[0],&isect1[1],isectpointA1,isectpointA2);
  if(*coplanar) return coplanar_tri_tri(N1,V0,V1,V2,U0,U1,U2);     


  /* compute interval for triangle 2 */
  compute_intervals_isectline(U0,U1,U2,up0,up1,up2,du0,du1,du2,
			      du0du1,du0du2,&isect2[0],&isect2[1],isectpointB1,isectpointB2);

  SORT2(isect1[0],isect1[1],smallest1);
  SORT2(isect2[0],isect2[1],smallest2);

  if(isect1[1]<isect2[0] || isect2[1]<isect1[0]) return 0;

  /* at this point, we know that the triangles intersect */

  if(isect2[0]<isect1[0])
  {
    if(smallest1==0) { SET(isectpt1,isectpointA1); }
    else { SET(isectpt1,isectpointA2); }

    if(isect2[1]<isect1[1])
    {
      if(smallest2==0) { SET(isectpt2,isectpointB2); }
      else { SET(isectpt2,isectpointB1); }
    }
    else
    {
      if(smallest1==0) { SET(isectpt2,isectpointA2); }
      else { SET(isectpt2,isectpointA1); }
    }
  }
  else
  {
    if(smallest2==0) { SET(isectpt1,isectpointB1); }
    else { SET(isectpt1,isectpointB2); }

    if(isect2[1]>isect1[1])
    {
      if(smallest1==0) { SET(isectpt2,isectpointA2); }
      else { SET(isectpt2,isectpointA1); }      
    }
    else
    {
      if(smallest2==0) { SET(isectpt2,isectpointB2); }
      else { SET(isectpt2,isectpointB1); } 
    }
  }
  return 1;
}
#endif
    }
}
#endif
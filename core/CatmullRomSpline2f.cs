using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core
{
    [Serializable]
    public class CatmullRomSpline2f
    {
        private int nControlPoints;
        private List< Vector2f[] > coefficients;

        public CatmullRomSpline2f( List< Vector2f > controlPoints )
        {
            nControlPoints = controlPoints.Count;
            if( nControlPoints < 2 )
            {
                throw new ArgumentException( "Catmull-Rom splines must have at least 2 control points!" );
            }
            coefficients = new List< Vector2f[] >( nControlPoints - 1 );

            // linear
            if( nControlPoints < 3 )
            {
                coefficients.Add( new Vector2f[] { controlPoints[ 0 ], controlPoints[ 1 ] } );
            }
            else if( nControlPoints < 4 )
            {
                coefficients.Add( GetQuadraticCoefficients( controlPoints[ 0 ], controlPoints[ 1 ], controlPoints[ 2 ] ) );
            }
            else
            {
                for( int i = 0; i < nControlPoints - 3; ++i )
                {
                    Vector2f c0 = controlPoints[ i ];
                    Vector2f c1 = controlPoints[ i + 1 ];
                    Vector2f c2 = controlPoints[ i + 2 ];
                    Vector2f c3 = controlPoints[ i + 3 ];
                    coefficients.Add( ComputeCatmullRomCoefficients( c0, c1, c2, c3 ) );
                }
            }
        }

        public int NumControlPoints
        {
            get
            {
                return nControlPoints;
            }
        }

        public Vector2f this[ float t ]
        {
            get
            {
                return EvaluateAt( t );
            }
        }

        public Vector2f EvaluateAt( float t )
        {
            t = t.Clamp( 0.0f, 1.0f );

            int n = NumControlPoints;
            if( n < 3 )
            {
                return Vector2f.Lerp( coefficients[ 0 ][ 0 ], coefficients[ 0 ][ 1 ], t );
            }
            else if( n < 4 )
            {
                Vector2f[] quadraticCoefficients = coefficients[ 0 ];
                float t2 = t * t;
                return quadraticCoefficients[ 0 ] + quadraticCoefficients[ 1 ] * t + quadraticCoefficients[ 2 ] * t2;
            }
            else
            {
                float u;
                Vector2f[] catmullRomCoefficients = GetCatmullRomCoefficientsAndParameter( t, out u );
                float u2 = u * u;
                float u3 = u2 * u;
                return( catmullRomCoefficients[ 0 ] + catmullRomCoefficients[ 1 ] * u + catmullRomCoefficients[ 2 ] * u2 + catmullRomCoefficients[ 3 ] * u3 );
            }
        }

        public Vector2f TangentAt( float t )
        {
            t = t.Clamp( 0.0f, 1.0f );

            int n = NumControlPoints;
            if( n < 3 )
            {
                return ( coefficients[ 0 ][ 1 ] - coefficients[ 0 ][ 0 ] ).Normalized();
            }
            else if( n < 4 )
            {
                Vector2f[] quadraticCoefficients = coefficients[ 0 ];
                return( quadraticCoefficients[ 1 ] + 2 * t * quadraticCoefficients[ 2 ] ).Normalized();
            }
            else
            {
                float u;
                Vector2f[] catmullRomCoefficients = GetCatmullRomCoefficientsAndParameter( t, out u );
                float u2 = u * u;
                return( catmullRomCoefficients[ 1 ] + 2 * catmullRomCoefficients[ 2 ] * u + 3 * catmullRomCoefficients[ 3 ] * u2 ).Normalized();
            }
        }        

        /// <summary>
        /// Given 3 control points
        /// returns c0, c1, c2, the quadratic spline coefficients
        /// q(t) = c0 + c1 * t + c2 * t^2
        /// q(0) = c0 = p0
        /// q(1) = c0 + c1 + c2 = p2
        /// q(1/2) = c0 + c1 / 2 + c2 / 4 = p1
        /// This yields the linear system:
        /// [ 1 0   0   ][ c0 ] = [ p0 ]
        /// [ 1 1/2 1/4 ][ c1 ] = [ p1 ]
        /// [ 1 1   1   ][ c2 ] = [ p2 ]
        /// inv( A ) =
        /// [ 1  0 0  ]
        /// [ -3 4 -1 ]
        /// [ 2 -4 2  ]
        /// So c0 = p0, c1 = -3 p0 + 4 p1 - p2, c2 = 2 p0 - 4 p1 + 2 p2        
        /// </summary>
        /// <param name="cps"></param>
        /// <returns></returns>
        private Vector2f[] GetQuadraticCoefficients( params Vector2f[] cps )
        {
            return new Vector2f[]
            {
                cps[ 0 ],
                -3 * cps[ 0 ] + 4 * cps[ 1 ] - cps[ 2 ],
                2 * cps[ 0 ] - 4 * cps[ 1 ] + 2 * cps[ 2 ]
            };
        }

        private Vector2f[] ComputeCatmullRomCoefficients( params Vector2f[] controlPoints )
        {
            return new Vector2f[]
            {
                controlPoints[ 1 ],
                0.5f * ( controlPoints[ 2 ] - controlPoints[ 0 ] ),
                controlPoints[ 0 ] - 2.5f * controlPoints[ 1 ] + 2 * controlPoints[ 2 ] - 0.5f * controlPoints[ 3 ],
                -0.5f * controlPoints[ 0 ] + 1.5f * controlPoints[ 1 ] - 1.5f * controlPoints[ 2 ] + 0.5f * controlPoints[ 3 ]
            };
        }

        /// <summary>
        /// Given the global parameter t
        /// Find the appropriate catmull-rom segment coefficients
        /// and the local parameter u
        /// </summary>
        /// <param name="t"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        private Vector2f[] GetCatmullRomCoefficientsAndParameter( float t, out float u )
        {
            // TODO: cache this too?            
            int i = ( t * coefficients.Count ).FloorToInt();
            if( i >= coefficients.Count )
            {
                i = coefficients.Count - 1;
            }

            // delta is the size of each interval
            float delta = 1.0f / coefficients.Count;
            u = ( t - i * delta ) / delta;

            return coefficients[ i ];
        }        
    }
}

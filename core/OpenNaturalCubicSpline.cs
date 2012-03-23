using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core
{
    [Serializable]
    public class OpenNaturalCubicSpline
    {
        private List< float > controlPoints;
        private List< Vector4f > coefficients;

        public OpenNaturalCubicSpline()
        {
            controlPoints = new List< float >();
            coefficients = new List< Vector4f >();
        }

        public int NumControlPoints
        {
            get
            {
                return controlPoints.Count;
            }
        }

        public bool IsValid()
        {
            return NumControlPoints >= 4;
        }

        public List< float > ControlPoints
        {
            get
            {
                return controlPoints;
            }
            set
            {
                controlPoints = value;
            }
        }

        public float GetControlPoint( int i )
        {
            return controlPoints[ i ];
        }

        public void SetControlPoint( int i, float p )
        {
            controlPoints[ i ] = p;
            ComputeCoefficients();
        }

        public void InsertControlPoint( int i, float p )
        {
            controlPoints.Insert( i, p );
            ComputeCoefficients();
        }

        public void AddControlPoint( float controlPoint )
        {
            controlPoints.Add( controlPoint );
            ComputeCoefficients();
        }

        public void FlipOrientation()
        {
            controlPoints.Reverse();
            ComputeCoefficients();
        }

        public float this[ float t ]
        {
            get
            {
                return EvaluateAt( t );
            }
        }

        public float EvaluateAt( float t )
        {
            if( !IsValid() )
            {
                throw new ArgumentException( "Cannot evaluate invalid spline." );
            }

            // spacing between control points
            float delta = 1.0f / ( NumControlPoints - 1 );
            // clamp t
            float ct = MathUtils.ClampToRange( t, 0, 1 );

            // find the 4 nearest control points
            int n;
            if( ct == 1.0f )
            {
                n = coefficients.Count - 1;
            }
            else
            {
                n = ( int )( ct / delta );
            }

            float u = ( ct - n * delta ) / delta;
            float u2 = u * u;
            float u3 = u2 * u;

            return ( coefficients[ n ][ 0 ] + u * coefficients[ n ][ 1 ] + u2 * coefficients[ n ][ 2 ] + u3 * coefficients[ n ][ 3 ] );
        }

        public float DerivativeAt( float t )
        {
            if( !IsValid() )
            {
                throw new ArgumentException( "Cannot evaluate invalid spline." );
            }

            // spacing between control points
            float delta = 1.0f / ( NumControlPoints - 1 );
            // clamp t
            float ct = MathUtils.ClampToRange( t, 0, 1 );

            // find the 3 nearest control points
            int n;
            if( ct == 1.0f )
            {
                n = coefficients.Count - 1;
            }
            else
            {
                n = ( int )( ct / delta );
            }

            float u = ( ct - n * delta ) / delta;
            float u2 = u * u;

            float c1 = coefficients[ n ][ 1 ];
            float c2 = coefficients[ n ][ 2 ];
            float c3 = coefficients[ n ][ 3 ];

            if( c1 == c2 && c2 == c3 )
            {
                float f = c1 + 2 * u * c2 + 3 * u2 * c3;
                Console.WriteLine( "f = {0}", f );
            }

            return ( ( c1 + 2 * u * c2 + 3 * u2 * c3 ) / delta );
        }

        public float Inverse( float x, float tGuess, float epsilon, int maxIterations )
        {
            float result = tGuess;
            float xResult = EvaluateAt( result );
            float error = x - xResult;
            float absError = Math.Abs( error );

            int n = 0;
            while( ( absError > epsilon ) && ( n < maxIterations ) )
            {
                float dxdt = DerivativeAt( result );
                result += error / dxdt;
                xResult = EvaluateAt( result );
                error = x - xResult;
                absError = Math.Abs( error );

                ++n;
            }

            return result;
        }

        private void ComputeCoefficients()
        {
            if( !IsValid() )
            {
                return;
            }

            int nPoints = NumControlPoints;

            float[] gamma = new float[ nPoints ];
            float[] delta = new float[ nPoints ];
            float[] D = new float[ nPoints ];

            gamma[ 0 ] = 0.5f;
            for( int i = 1; i < nPoints - 1; ++i )
            {
                gamma[ i ] = 1.0f / ( 4.0f - gamma[ i - 1 ] );
            }
            gamma[ nPoints - 1 ] = 1.0f / ( 2.0f - gamma[ nPoints - 2 ] );

            delta[ 0 ] = 3.0f * ( controlPoints[ 1 ] - controlPoints[ 0 ] ) * gamma[ 0 ];
            for( int i = 1; i < nPoints - 1; ++i )
            {
                delta[ i ] = ( 3.0f * ( controlPoints[ i + 1 ] - controlPoints[ i - 1 ] ) - delta[ i - 1 ] ) * gamma[ i ];
            }
            delta[ nPoints - 1 ] = ( 3.0f * ( controlPoints[ nPoints - 1 ] - controlPoints[ nPoints - 2 ] ) - delta[ nPoints - 2 ] ) * gamma[ nPoints - 1 ];

            D[ nPoints - 1 ] = delta[ nPoints - 1 ];
            for( int i = nPoints - 2; i >= 0; --i )
            {
                D[ i ] = delta[ i ] - gamma[ i ] * D[ i + 1 ];
            }

            coefficients.Clear();
            coefficients.Capacity = nPoints - 1;

            for( int i = 0; i < nPoints - 1; ++i )
            {
                coefficients.Add
                (
                    new Vector4f
                    (
                        controlPoints[ i ],
                        D[ i ],
                        3.0f * ( controlPoints[ i + 1 ] - controlPoints[ i ] ) - 2.0f * D[ i ] - D[ i + 1 ],
                        2.0f * ( controlPoints[ i ] - controlPoints[ i + 1 ] ) + D[ i ] + D[ i + 1 ]
                    )
                );
            }
        }
    }
}

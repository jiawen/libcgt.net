using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core
{
    [Serializable]
    public class Spline2f< T > where T : IControlPoint2f
    {
        public event Action ControlPointsChanged;

        protected List< T > controlPoints;
        private bool isClosedLoop;

        private int nPointsToEvaluateFactor;

        private AbstractCache< int, PiecewiseCatmullRomSpline< T > > piecewiseCache;
        private AbstractCache< int, Vector2f[] > evaluationCache;

        public Spline2f( int nPointsToEvaluateFactor )
        {
            this.controlPoints = new List< T >( 4 );
            this.isClosedLoop = false;

            this.nPointsToEvaluateFactor = nPointsToEvaluateFactor;
            
            piecewiseCache = new Spline2fPiecewiseCache< T >( this );
            evaluationCache = new Spline2fEvaluationCache< T >( this );
        }

        public Spline2f( Spline2f< T > spline )
        {
            this.isClosedLoop = spline.isClosedLoop;

            this.nPointsToEvaluateFactor = spline.nPointsToEvaluateFactor;

            this.controlPoints = new List< T >( spline.controlPoints.Count );
            piecewiseCache = new Spline2fPiecewiseCache< T >( this );
            evaluationCache = new Spline2fEvaluationCache< T >( this );

            for( int c = 0; c < spline.controlPoints.Count; ++c )
            {
                var cp = ( T ) ( spline.controlPoints[ c ].Clone() );
                AddControlPoint( cp );
            }            
        }

        public int NumControlPoints
        {
            get
            {
                return controlPoints.Count;
            }
        }

        public virtual void AddControlPoint( T controlPoint )
        {
            controlPoints.Add( controlPoint );
            controlPoint.PropertyChanged += controlPoint_PropertyChanged;

            ControlPointsChanged();
        }

        public virtual void InsertControlPoint( int i, T controlPoint )
        {
            if( i < 0 || i > controlPoints.Count )
            {
                throw new ArgumentException( "InsertControlPoint: i must be in range [0,NumControlPoints], NumControlPoints appends to end." );
            }
            controlPoints.Insert( i, controlPoint );

            controlPoint.PropertyChanged += controlPoint_PropertyChanged;

            ControlPointsChanged();
        }

        public virtual int InsertControlPoint( T controlPoint )
        {
            float t;
            Vector2f closestPoint = ClosestPointOnSpline( controlPoint.Position, out t );            

            if( t <= 0 )
            {                
                InsertControlPoint( 0, controlPoint );
                return 0;
            }
            
            if( t >= 1 )
            {
                InsertControlPoint( NumControlPoints, controlPoint );
                return NumControlPoints - 1;
            }
            
            // the control point index given t
            int i = GetLowerControlPoint( t );
            InsertControlPoint( i + 1, controlPoint );
            controlPoint.Position = closestPoint;
            return i + 1;
        }

        public void RemoveAllControlPoints()
        {
            this.controlPoints = new List< T >( 4 );
            this.isClosedLoop = false;

            ControlPointsChanged();
        }

        public void RemoveControlPoint( int i )
        {
            controlPoints[ i ].PropertyChanged -= controlPoint_PropertyChanged;
            controlPoints.RemoveAt( i );
            ControlPointsChanged();
        }

        public void RemoveControlPoints( int i, int count )
        {
            for( int j = 0; j < count; ++j )
            {
                controlPoints[ i + j ].PropertyChanged -= controlPoint_PropertyChanged;
            }
            controlPoints.RemoveRange( i, count );
            ControlPointsChanged();
        }

        public T GetControlPoint( int i )
        {
            return controlPoints[ i ];
        }

        public void SetControlPoint( int i, T controlPoint )
        {
            controlPoints[ i ].PropertyChanged -= controlPoint_PropertyChanged;
            controlPoints[ i ] = controlPoint;
            controlPoints[ i ].PropertyChanged += controlPoint_PropertyChanged;
            ControlPointsChanged();
        }

        public List< T > ControlPoints
        {
            get
            {
                return controlPoints;
            }
        }

        public bool IsClosedLoop
        {
            get
            {
                return isClosedLoop;
            }
            set
            {
                isClosedLoop = value;
                ControlPointsChanged();
            }
        }

        /// <summary>
        /// Returns the index of the control point closest to the point p
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int ClosestControlPoint( Vector2f p )
        {
            float f;
            return ClosestControlPoint( p, out f );
        }

        /// <summary>
        /// Returns the index of the control point closest to the point p
        /// along with the distance squared to p
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int ClosestControlPoint( Vector2f p, out float distanceSquared )
        {
            int minIndex = -1;
            float minDistanceSquared = float.PositiveInfinity;

            for( int i = 0; i < NumControlPoints; ++i )
            {
                Vector2f controlPoint = GetControlPoint( i ).Position;
                float currentDistanceSquared = ( p - controlPoint ).NormSquared();
                if( currentDistanceSquared < minDistanceSquared )
                {
                    minDistanceSquared = currentDistanceSquared;
                    minIndex = i;
                }
            }

            distanceSquared = minDistanceSquared;
            return minIndex;
        }

        public void FlipOrientation()
        {
            controlPoints.Reverse();
            ControlPointsChanged();
        }

        public int NumPointsToEvaluateFactor
        {
            get
            {
                return nPointsToEvaluateFactor;
            }
            set
            {
                nPointsToEvaluateFactor = value;
                ControlPointsChanged();
            }
        }

        public int NumPointsToEvaluate
        {
            get
            {
                return NumControlPoints * NumPointsToEvaluateFactor;
            }
        }        

        public Vector2f[] Evaluate()
        {
            return evaluationCache[ 0 ];
        }

        public Vector2f[] Evaluate( int nSamples )
        {
            var pts = new Vector2f[ nSamples ];
            float spacing = EvaluationSpacing( nSamples );
            
            for( int i = 0; i < nSamples; ++i )
            {
                float t = i * spacing;
                pts[ i ] = this[ t ];
            }
            
            return pts;
        }

        public float EvaluationSpacing()
        {
            return EvaluationSpacing( NumPointsToEvaluate );
        }

        public float EvaluationSpacing( int nPointsEvaluated )
        {
            return 1.0f / ( nPointsEvaluated - 1 );
        }

        public float ControlPointSpacing()
        {
            return 1.0f / ( NumControlPoints - 1 );
        }

        public float ParameterForControlPoint( int i )
        {
            return i * ControlPointSpacing();
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
            return piecewiseCache[ 0 ].EvaluateAt( t );
        }

        public Vector2f TangentAt( float t )
        {
            return piecewiseCache[ 0 ].TangentAt( t );
        }

        public Vector2f NormalAt( float t )
        {
            return TangentAt( t ).OrthogonalVector();
        }

        /// <summary>
        /// Returns a positive value if p is in the positive half-space of this spline
        /// and a negative value if p is in the negative half-space
        /// evluated with respect to the spline position and tangent at parameter t
        /// </summary>
        /// <param name="p"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public float HalfSpace( Vector2f p, float t )
        {
            var s = EvaluateAt( t );
            var tangent = TangentAt( t );
            return Vector2f.Cross( tangent, ( p - s ).Normalized() ).z;
        }        

        public Vector2f ClosestPointOnSpline( Vector2f p )
        {
            float t;
            return ClosestPointOnSpline( p, out t );
        }

        public Vector2f ClosestPointOnSpline( Vector2f p, out float closestT )
        {
            float closestDistance;
            return ClosestPointOnSpline( p, out closestT, out closestDistance );
        }

        public Vector2f ClosestPointOnSpline( Vector2f p, out float closestT, out float closestDistance )
        {
            Vector2f[] pts = Evaluate();

            Vector2f minPoint = new Vector2f( 0, 0 );
            float minDistance = float.PositiveInfinity;
            float minT = -1;

            for( int i = 0; i < pts.Length; ++i )
            {
                Vector2f splinePoint = pts[ i ];
                float currentDistanceSquared = ( splinePoint - p ).NormSquared();
                if( currentDistanceSquared < minDistance )
                {
                    minDistance = currentDistanceSquared;
                    minPoint = splinePoint;
                    minT = i * EvaluationSpacing();
                }
            }

		    closestT = minT;
            closestDistance = ( float )( Math.Sqrt( minDistance ) );
            
            return minPoint;
        }

        // HACK
        public int GetLowerControlPoint( float t )
        {
            float f;
            return GetLowerControlPoint( t, out f );
        }

        public int GetLowerControlPoint( float t, out float fraction )
        {
            int n = NumControlPoints;

            if( t <= 0 )
            {
                fraction = 0;
                return 0;
            }
            else if( t >= 1 )
            {
                fraction = 0;
                return n - 1;
            }
            else
            {
                int i0 = ( t * ( n - 1 ) ).FloorToInt();
                fraction = ( t - i0 * ControlPointSpacing() ) / ControlPointSpacing();
                return i0;
            }
        }

        private void controlPoint_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            ControlPointsChanged();
        }
    }
}


/*
namespace libcgt.core
{
    [Serializable]
    public class Spline2f
    {
        public Vector2f TangentAt( float t )
        {
            int i;
            bool nearCorner = NearCorner( t, out i );
            
            if( nearCorner )
            {
                float t0 = t - EvaluationSpacing;
                float t1 = t + EvaluationSpacing;

                float u0;
                var spline0 = GetSegmentAndParameter( t0, out u0 );

                float u1;
                var spline1 = GetSegmentAndParameter( t1, out u1 );

                return 0.5f * ( spline0.TangentAt( u0 ) + spline1.TangentAt( u1 ) );
            }
            else
            {
                float u;
                var spline = GetSegmentAndParameter( t, out u );
                
                return spline.TangentAt( u );
            }
        }        

        /// <summary>
        /// Returns true if parameter t is near a corner (within EvaluationSpacing)
        /// If so, also returns the index of the closest control point.
        /// if t <= EvaluationSpacing or t >= 1 - EvaluationSpacing, returns false and -1, respectively        
        /// </summary>
        /// <param name="t"></param>
        /// <param name="closestControlPoint"></param>
        /// <returns></returns>
        private bool NearCorner( float t, out int closestControlPoint )
        {
            float delta = EvaluationSpacing;
            if( t <= delta || t >= ( 1.0f - delta ) )
            {
                closestControlPoint = -1;
                return false;
            }

            closestControlPoint = GetClosestControlPoint( t );
            if( IsCorner( closestControlPoint ) )
            {
                float it = closestControlPoint * ControlPointSpacing;
                return Math.Abs( t - it ) < EvaluationSpacing;
            }
            else
            {
                closestControlPoint = -1;
                return false;
            }
        }
    }
}
*/
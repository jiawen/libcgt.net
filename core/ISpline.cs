using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public interface ISpline
    {
        int NumControlPoints { get; }
        
        void AddControlPoint( float controlPoint );

        void InsertControlPoint( int i, float p );

        float GetControlPoint( int i );

        void SetControlPoint( int i, float p );
        
        bool IsCorner( int i );

        void SetIsCorner( int i, bool isCorner );

        void FlipOrientation();

        float this[ float t ] { get; }

        float EvaluateAt( float t );
        
        float DerivativeAt( float t );
    }
}

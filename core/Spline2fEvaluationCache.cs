using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public class Spline2fEvaluationCache< T > : AbstractCache< int, Vector2f[] > where T : IControlPoint2f
    {
        private Spline2f< T > spline;

        public Spline2fEvaluationCache( Spline2f< T > spline )
        {
            this.spline = spline;

            InsertEntry( 0, new Vector2f[ spline.NumPointsToEvaluate ] );

            spline.ControlPointsChanged += MarkAllEntriesDirty;
        }

        protected override void UpdateEntry( int key, ref Vector2f[] entry )
        {
            int nSamples = spline.NumPointsToEvaluate;
            if( entry.Length != nSamples )
            {
                entry = new Vector2f[ nSamples ];
            }

            float spacing = spline.EvaluationSpacing( nSamples );
            for( int i = 0; i < nSamples; ++i )
            {
                float t = i * spacing;
                entry[ i ] = spline[ t ];
            }
        }
    }
}

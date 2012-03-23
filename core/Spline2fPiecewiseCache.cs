using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public class Spline2fPiecewiseCache< T > : AbstractCache< int, PiecewiseCatmullRomSpline< T > >
        where T : IControlPoint2f
    {
        private Spline2f< T > spline;

        public Spline2fPiecewiseCache( Spline2f< T > spline )
        {
            this.spline = spline;

            InsertEntry( 0, null );

            spline.ControlPointsChanged += MarkAllEntriesDirty;
        }

        protected override void UpdateEntry( int key, ref PiecewiseCatmullRomSpline< T > entry )
        {
            entry = new PiecewiseCatmullRomSpline< T >( spline.ControlPoints, spline.IsClosedLoop );
        }
    }
}

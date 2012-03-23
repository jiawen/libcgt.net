using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class MeanValueCoordinates2D
    {
        /// <summary>
        /// Computes the two-dimensional mean value coordinates at v0 with respect to polygon.
        /// 
        /// polygon specifies a list of polygon vertices in 2D in counterclockwise order.
        /// It does *not* need to be convex.
        ///
        /// Returns an array of mean value coordinates, one per vertex in the polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static float[] Compute( Vector2f v0, List< Vector2f > polygon )
        {
            var weights = new float[ polygon.Count ];
            Compute( v0, polygon, weights );
            return weights;
        }

        /// <summary>
        /// Computes the two-dimensional mean value coordinates at v0 with respect to polygon.
        /// 
        /// polygon specifies a list of polygon vertices in 2D in counterclockwise order.
        /// It does *not* need to be convex.
        ///
        /// outputWeights must be an array of length equal to polygon.Count
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static void Compute( Vector2f v0, List< Vector2f > polygon, float[] outputWeights )
        {
            int nVertices = polygon.Count;
            
            float sumWeights = 0;

            for( int i = 0; i < nVertices; ++i )
            {
                var vi = polygon[ i ];
                var vip1 = polygon[ Arithmetic.Mod( i + 1, nVertices ) ];
                var vim1 = polygon[ Arithmetic.Mod( i - 1, nVertices ) ];

                var ei = vi - v0;
                var eip1 = vip1 - v0;
                var eim1 = vim1 - v0;

                var alpha_i = Vector2f.AngleBetween( ei, eip1 );
                var alpha_im1 = Vector2f.AngleBetween( eim1, ei );

                var weight = ( MathUtils.Tan( 0.5f * alpha_im1 ) + MathUtils.Tan( 0.5f * alpha_i ) ) / ( vi - v0 ).Norm();
                outputWeights[ i ] = weight;
                sumWeights += weight;
            }

            for( int i = 0; i < nVertices; ++i )
            {
                outputWeights[ i ] /= sumWeights;
            }
        }
    }
}

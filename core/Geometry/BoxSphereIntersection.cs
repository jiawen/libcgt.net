using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class BoxSphereIntersection
    {
        public static bool SolidSphereHollowBox( Vector2f sphereCenter, float sphereRadius,
            Vector2f boxMin, Vector2f boxSize )
        {
            float dmin = 0;
            var boxMax = boxMin + boxSize;
            bool face = false;
            for( int i = 0; i < 2; ++i )
            {
                if( sphereCenter[ i ] < boxMin[ i ] )
                {
                    face = true;
                    dmin += ( sphereCenter[ i ] - boxMin[ i ] ) * ( sphereCenter[ i ] - boxMin[ i ] );
                }
                else if( sphereCenter[ i ] > boxMax[ i ] )
                {
                    face = true;
                    dmin += ( sphereCenter[ i ] - boxMax[ i ] ) * ( sphereCenter[ i ] - boxMax[ i ] );
                }
                else if( sphereCenter[ i ] - boxMin[ i ] <= sphereRadius )
                {
                    face = true;
                }
                else if( boxMax[ i ] - sphereCenter[ i ] <= sphereRadius )
                {
                    face = true;
                }
            }
            return( face && ( dmin <= sphereRadius * sphereRadius ) );
        }

        public static bool SolidSphereHollowBox( Vector3f sphereCenter, float sphereRadius,
            Vector3f boxCorner, Vector3f boxSize )
        {
            float dmin = 0;
            var boxMax = boxCorner + boxSize;
            bool face = false;
            for( int i = 0; i < 3; ++i )
            {
                if( sphereCenter[ i ] < boxCorner[ i ] )
                {
                    face = true;
                    dmin += ( sphereCenter[ i ] - boxCorner[ i ] ) * ( sphereCenter[ i ] - boxCorner[ i ] );
                }
                else if( sphereCenter[ i ] > boxMax[ i ] )
                {
                    face = true;
                    dmin += ( sphereCenter[ i ] - boxMax[ i ] ) * ( sphereCenter[ i ] - boxMax[ i ] );
                }
                else if( sphereCenter[ i ] - boxCorner[ i ] <= sphereRadius )
                {
                    face = true;
                }
                else if( boxMax[ i ] - sphereCenter[ i ] <= sphereRadius )
                {
                    face = true;
                }
            }
            return ( face && ( dmin <= sphereRadius * sphereRadius ) );
        }

        /// <summary>
        /// Solid sphere intersect solid box.        
        /// </summary>
        /// <param name="sphereCenter"></param>
        /// <param name="sphereRadius"></param>
        /// <param name="boxCorner"></param>
        /// <param name="boxSize"></param>
        /// <returns></returns>
        public static bool SolidSphereSolidBox( Vector2f sphereCenter, float sphereRadius,
            Vector2f boxMin, Vector2f boxSize )
        {
            float dMin = 0;
            var boxMax = boxMin + boxSize;

            for( int i = 0; i < 2; ++i )
            {
                if( sphereCenter[ i ] < boxMin[ i ] )
                {
                    dMin += Arithmetic.Square( sphereCenter[ i ] - boxMin[ i ] );
                }
                else if( sphereCenter[ i ] > boxMax[ i ] )
                {
                    dMin += Arithmetic.Square( sphereCenter[ i ] - boxMax[ i ] );
                }
            }
            return( dMin < sphereRadius * sphereRadius );
        }

        /// <summary>
        /// Solid sphere intersect solid box.        
        /// </summary>
        /// <param name="sphereCenter"></param>
        /// <param name="sphereRadius"></param>
        /// <param name="boxCorner"></param>
        /// <param name="boxSize"></param>
        /// <returns></returns>
        public static bool SolidSphereSolidBox( Vector3f sphereCenter, float sphereRadius,
            Vector3f boxMin, Vector3f boxSize )
        {
            float dMin = 0;
            var boxMax = boxMin + boxSize;

            for( int i = 0; i < 3; ++i )
            {
                if( sphereCenter[ i ] < boxMin[ i ] )
                {
                    dMin += Arithmetic.Square( sphereCenter[ i ] - boxMin[ i ] );
                }
                else if( sphereCenter[ i ] > boxMax[ i ] )
                {
                    dMin += Arithmetic.Square( sphereCenter[ i ] - boxMax[ i ] );
                }
            }
            return ( dMin < sphereRadius * sphereRadius );
        }

        /// TODO: look at vectorized version
        /// Not solid-solid!
        /// By Larsson, Akenine-Moller and Lengyel, JGT
        /// From: http://www.idt.mdh.se/personal/tla/publ/sb.pdf
    }
}

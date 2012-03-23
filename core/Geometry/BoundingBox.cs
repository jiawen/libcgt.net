using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public static class BoundingBox
    {
        public static Rect2f FindBoundingBox( params Vector2f[] vertices )
        {
            var v0 = vertices[ 0 ];
            float xMin = v0.x;
            float xMax = v0.x;
            float yMin = v0.y;
            float yMax = v0.y;

            for( int i = 1; i < vertices.Length; ++i )
            {
                var v = vertices[ i ];
                if( v.x < xMin )
                {
                    xMin = v.x;
                }
                if( v.x > xMax )
                {
                    xMax = v.x;
                }

                if( v.y < yMin )
                {
                    yMin = v.y;
                }
                if( v.y > yMax )
                {
                    yMax = v.y;
                }
            }

            return new Rect2f( xMin, yMin, xMax - xMin, yMax - yMin );
        }

        public static Box3f FindBoundingBox( IEnumerable< Vector3f > vertices )
        {
            float xMin = float.PositiveInfinity;
            float xMax = float.NegativeInfinity;
            float yMin = float.PositiveInfinity;
            float yMax = float.NegativeInfinity;
            float zMin = float.PositiveInfinity;
            float zMax = float.NegativeInfinity;

            foreach( var v in vertices )
            {                
                if( v.x < xMin )
                {
                    xMin = v.x;
                }
                if( v.x > xMax )
                {
                    xMax = v.x;
                }

                if( v.y < yMin )
                {
                    yMin = v.y;
                }
                if( v.y > yMax )
                {
                    yMax = v.y;
                }

                if( v.z < zMin )
                {
                    zMin = v.z;
                }
                if( v.z > zMax )
                {
                    zMax = v.z;
                }
            }

            var min = new Vector3f( xMin, yMin, zMin );
            var max =  new Vector3f( xMax, yMax, zMax );

            return Box3f.AxisAligned( min, max );
        }
    }
}

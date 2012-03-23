using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    [Serializable]
    public class Box3f
    {
        public Vector3f V000 { get; set; }
        public Vector3f V100 { get; set; }
        public Vector3f V010 { get; set; }
        public Vector3f V110 { get; set; }
        public Vector3f V001 { get; set; }
        public Vector3f V101 { get; set; }
        public Vector3f V011 { get; set; }
        public Vector3f V111 { get; set; }

        public static Box3f AxisAligned( Vector3f min, Vector3f max )
        {
            var d = max - min;
            var dx = new Vector3f( d.x, 0, 0 );
            var dy = new Vector3f( 0, d.y, 0 );
            var dz = new Vector3f( 0, 0, d.z );
            return new Box3f( min, dx, dy, dz );
        }

        public Box3f( Vector3f origin, Vector3f dx, Vector3f dy, Vector3f dz )
        {
            V000 = origin;
            V100 = origin + dx;
            V010 = origin + dy;
            V110 = origin + dx + dy;
            V001 = origin + dz;
            V101 = origin + dx + dz;
            V011 = origin + dy + dz;
            V111 = origin + dx + dy + dz;
        }

        public Vector3f Center
        {
            get
            {
                return 0.5f * ( V000 + V111 );
            }
        }

        public Vector3f DX
        {
            get
            {
                return new Vector3f( V100 - V000 );
            }
        }

        public Vector3f DY
        {
            get
            {
                return new Vector3f( V010 - V000 );
            }
        }

        public Vector3f DZ
        {
            get
            {
                return new Vector3f( V001 - V000 );
            }
        }

        public Vector3f SideLengths
        {
            get
            {
                return new Vector3f( DX.Norm(), DY.Norm(), DZ.Norm() );
            }
        }

        public Plane3f[] ClippingPlanes( bool normalsPointOutward )
        {
            var planes = new Plane3f[ 6 ];

            // back
            planes[ 0 ] = new Plane3f( V000, V010, V100 );

            // front
            planes[ 1 ] = new Plane3f( V001, V101, V111 );

            // left
            planes[ 2 ] = new Plane3f( V011, V010, V000 );

            // right
            planes[ 3 ] = new Plane3f( V101, V100, V111 );

            // top
            planes[ 4 ] = new Plane3f( V011, V111, V010 );

            // bottom
            planes[ 5 ] = new Plane3f( V000, V100, V001 );

            if( !normalsPointOutward )
            {
                for( int i = 0; i < 6; ++i )
                {
                    planes[ i ] = planes[ i ].Flipped();
                }
            }

            return planes;
        }

        /*
         * public Vector3f V000 { get; set; }
        public Vector3f V100 { get; set; }
        public Vector3f V010 { get; set; }
        public Vector3f V110 { get; set; }
        public Vector3f V001 { get; set; }
        public Vector3f V101 { get; set; }
        public Vector3f V011 { get; set; }
        public Vector3f V111 { get; set; }
         * */
    }
}

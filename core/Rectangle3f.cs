using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    // TODO: quadrilateral is the general version, make this one actually be a rect
    // (store as origin, dx, dy, ... or something)
    public class Rectangle3f
    {
        public Vector3f V00 { get; set; }
        public Vector3f V10 { get; set; }
        public Vector3f V01 { get; set; }
        public Vector3f V11 { get; set; }

        /// <summary>
        /// dx needs to be orthogonal to dy
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public Rectangle3f( Vector3f origin, Vector3f dx, Vector3f dy )
        {
            V00 = origin;
            V10 = origin + dx;
            V01 = origin + dy;
            V11 = origin + dx + dy;
        }

        /// <summary>
        /// Returns the unit normal vector
        /// </summary>
        public Vector3f Normal
        {
            get
            {
                return Vector3f.Cross( V10 - V00, V01 - V00 ).Normalized();
            }
        }

        public Vector3f Center
        {
            get
            {
                return 0.25f * ( V00 + V10 + V01 + V11 );
            }
        }

        public Vector3f DX
        {
            get
            {
                return V10 - V00;
            }
        }

        public Vector3f DY
        {
            get
            {
                return V01 - V00;
            }
        }

        public float Width
        {
            get
            {
                return DX.Norm();
            }
        }

        public float Height
        {
            get
            {
                return DY.Norm();
            }
        }

        /// <summary>
        /// Moves this rectangle along its coordinate system by dx, dy, dz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        public Rectangle3f Offset( float dx, float dy, float dz )
        {
            var unitX = DX.Normalized();
            var unitY = DY.Normalized();
            return new Rectangle3f( V00 + dx * unitX + dy * unitY + dz * Normal, DX, DY );
        }

        public Rectangle3f Dilated( float dx, float dy )
        {
            var sx = DX.Norm();
            var sy = DY.Norm();
            var unitX = DX.Normalized();
            var unitY = DY.Normalized();

            return new Rectangle3f( V00 - dx * unitX - dy * unitY, ( sx + 2 * dx ) * unitX, ( sy + 2 * dy ) * unitY );            
        }

        /// <summary>
        /// Extrude this rectangle into a box by z along its normal
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public Box3f Extruded( float z )
        {
            return new Box3f( V00, DX, DY, z * Normal );
        }
    }
}

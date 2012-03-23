using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    /// <summary>
    /// A 4D array of data stored where the first index varies the fastest.    
    /// int k = w * ( nx * ny * nz )
    ///       + z * ( nx * ny )
    ///       + y * nx
    ///       + x;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Array4D< T > : IEnumerable< T >
    {
        private int nx;
        private int ny;
        private int nz;
        private int nw;

        private T[] data;        

        public Array4D( int nx, int ny, int nz, int nw )
        {
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.nw = nw;

            int nElements = nx * ny * nz * nw;

            data = new T[ nElements ];
        }

        public int NX
        {
            get
            {
                return nx;
            }
        }

        public int NY
        {
            get
            {
                return ny;
            }
        }
        
        public int NZ
        {
            get
            {
                return nz;
            }
        }

        public int NW
        {
            get
            {
                return nw;
            }
        }

        public T[] Data
        {
            get
            {
                return data;
            }
        }

        public int NumElements
        {
            get
            {
                return data.Length;
            }
        }

        /// <summary>
        /// Access the "k-th" element of the array as it's stored        
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public T this[ int k ]
        {
            get
            {
                return data[ k ];
            }
            set
            {
                data[ k ] = value;
            }
        }

        public T this[ int x, int y, int z, int w ]
        {
            get
            {
                return data[ SubscriptsToIndex( x, y, z, w ) ];
            }
            set
            {
                data[ SubscriptsToIndex( x, y, z, w ) ] = value;
            }
        }

        public T this[ Vector4i xyzw ]
        {
            get
            {
                return this[ xyzw.x, xyzw.y, xyzw.z, xyzw.w ];
            }
            set
            {
                this[ xyzw.x, xyzw.y, xyzw.z, xyzw.w ] = value;
            }
        }

        public Vector4i IndexToSubscripts( int k )
        {
            int w = k / ( nx * ny * nz );
            k -= w * ( nx * ny * nz );

            int z = k / ( nx * ny );
            k -= z * ( nx * ny );

            int y = k / nx;
            k -= y * nx;

            int x = k;

            return new Vector4i( x, y, z, w );
        }

        public int SubscriptsToIndex( int x, int y, int z, int w )
        {
            /*
            int k = w * ( nx * ny * nz )
                + z * ( nx * ny )
                + y * nx
                + x;
            */

            int k = x;
            
            int d = nx;
            k += y * d;

            d *= ny;            
            k += z * d;

            d *= nz;            
            k += w * d;

            return k;
        }

        public IEnumerator< T > GetEnumerator()
        {
            return data.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

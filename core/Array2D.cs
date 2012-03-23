using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    /// <summary>
    /// A 2D array of data stored row major in a flattened 1D array.
    /// sub2ind: x, y --> y * width + x
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Array2D< T > : IEnumerable< T >
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public T[] Data { get; private set; }

        public Array2D( int width, int height )
        {
            Width = width;
            Height = height;
            Data = new T[ width * height ];
        }

        public T this[ int k ]
        {
            get
            {
                return Data[ k ];
            }
            set
            {
                Data[ k ] = value;
            }
        }

        public T this[ int x, int y ]
        {
            get
            {
                return Data[ y * Width + x ];
            }
            set
            {
                Data[ y * Width + x ] = value;
            }
        }

        public T this[ Vector2i xy ]
        {
            get
            {
                return Data[ xy.y * Width + xy.x ];
            }
            set
            {
                Data[ xy.y * Width + xy.x ] = value;
            }
        }

        public int NumElements
        {
            get
            {
                return Width * Height;
            }
        }

        public Vector2i IndexToSubscripts( int k )
        {
            int y = k / Width;
            int x = k % Width;
            return new Vector2i( x, y );
        }

        public IEnumerator< T > GetEnumerator()
        {
            return Data.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public class Box4f
    {
        public Vector4f origin;
        public Vector4f delta;

        public Box4f( Vector4f origin, Vector4f delta )
        {
            this.origin = origin;
            this.delta = delta;
        }

        public Vector4f[] Vertices
        {
            get
            {
                var output = new Vector4f[ 16 ];

                for ( int i = 0; i < 16; ++i )
                {
                    var vertex = origin;

                    for ( int j = 0; j < 4; ++j )
                    {
                        if ( ( i & ( 1 << j ) ) != 0 )
                        {
                            vertex[ j ] += delta[ j ];
                        }
                    }

                    output[ i ] = vertex;
                }

                return output;
            }
        }

        public Vector4f Center
        {
            get
            {
                return origin + 0.5f * delta;
            }
        }

        /// <summary>
        /// Subdivides this box in half along each dimension.
        /// If the d-th dimension is <= minDeltas[ d ],
        /// then that side is not divided, but the rest might be.
        /// </summary>
        /// <param name="minDeltas"></param>
        /// <returns></returns>
        public List< Box4f > SubdivideHalf( Vector4f minDeltas )
        {
            var delta2 = delta / 2;            
            var outputs = new List< Box4f >( 16 );

            for( int i = 0; i < 16; ++i )
            {
                bool addToOutput = true;
                var origin2 = origin;

                for( int j = 0; j < 4; ++j )
                {
                    if( ( i & ( 1 << j ) ) != 0 )
                    {
                        if( delta2[ j ] > minDeltas[ j ] )
                        {
                            origin2[ j ] += delta2[ j ];
                        }
                        else
                        {
                            addToOutput = false;
                        }
                    }
                }

                if( addToOutput )
                {
                    outputs.Add( new Box4f( origin2, delta2 ) );
                }
            }

            return outputs;
        }        

        public override string ToString()
        {
            return string.Format( "Box4f: origin = {0}, delta = {1}", origin, delta );
        }
    }
}

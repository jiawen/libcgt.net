using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Geometry
{
    public class Box4i
    {
        public Vector4i origin;
        public Vector4i delta;

        public Box4i( Vector4i origin, Vector4i delta )
        {
            this.origin = origin;
            this.delta = delta;
        }

        public Vector4i[] Vertices
        {
            get
            {
                var output = new Vector4i[16];

                for( int i = 0; i < 16; ++i )
                {
                    var vertex = origin;

                    for( int j = 0; j < 4; ++j )
                    {
                        if( ( i & ( 1 << j ) ) != 0 )
                        {
                            vertex[ j ] += delta[ j ];
                        }
                    }

                    output[ i ] = vertex;
                }

                return output;
            }
        }

        /// <summary>
        /// Subdivides this box in half along each dimension.
        /// If the d-th dimension is 1,
        /// then that side is not divided, but the rest might be.
        /// </summary>
        /// <returns></returns>
        public List< Box4i > SubdivideHalf()
        {
            var delta2 = delta / 2;            
            var outputs = new List< Box4i >( 16 );

            for( int i = 0; i < 16; ++i )
            {
                bool addToOutput = true;
                var origin2 = origin;

                for( int j = 0; j < 4; ++j )
                {
                    if( ( i & ( 1 << j ) ) != 0 )
                    {
                        if( delta2[ j ] > 0 )
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
                    outputs.Add( new Box4i( origin2, delta2 ) );
                }
            }

            return outputs;
        }        

        /// <summary>
        /// Inclusive on left, exclusive on right.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ContainsPoint( Vector4i p )
        {
            return
                (
                    origin.x <= p.x &&
                    origin.y <= p.y &&
                    origin.z <= p.z &&
                    origin.w <= p.w &&

                    p.x < origin.x + delta.x &&
                    p.y < origin.y + delta.y &&
                    p.z < origin.z + delta.z &&
                    p.z < origin.w + delta.w
                );
        }

        public override string ToString()
        {
            return string.Format( "Box4i: origin = {0}, delta = {1}", origin, delta );
        }
    }
}

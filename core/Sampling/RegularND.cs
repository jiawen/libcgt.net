using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Sampling
{
    public class RegularND : IPatternND
    {
        private int nDimensions;
        private int[] nSamples;

        public RegularND( params int[] nSamples )
        {
            nDimensions = nSamples.Length;

            this.nSamples = new int[ nDimensions ];
            Array.Copy( nSamples, this.nSamples, nSamples.Length );
        }

        public int NumDimensions
        {
            get
            {
                return nDimensions;
            }
        }

        public void GetNumSamples( int[] output )
        {
            Array.Copy( nSamples, output, nSamples.Length );
        }

        public void GetSample( float[] output, params int[] indices )
        {
            for( int i = 0; i < nDimensions; ++i )
            {
                output[ i ] = 0.5f;
            }
        }
    }
}

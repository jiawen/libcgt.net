using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Sampling
{
    public class StratifiedND : IPatternND
    {
        private int[] nSamples;
        private int nDimensions;
        private float[] values;

        public StratifiedND( Random random, params int[] nSamples )
        {
            nDimensions = nSamples.Length;

            this.nSamples = new int[ nDimensions ];
            Array.Copy( nSamples, this.nSamples, nSamples.Length );

            int nSamplesTotal = NumSamplesTotal;
            int nFloatsTotal = nSamplesTotal * nDimensions;
            values = new float[ nFloatsTotal ];
            for( int i = 0; i < nFloatsTotal; ++i )
            {
                values[ i ] = random.NextFloat();
            }
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

        public int NumSamplesTotal
        {
            get
            {
                return nSamples.Product();
            }
        }

        public void GetSample( float[] output, params int[] subscripts )
        {
            int k = SubscriptsToIndex( subscripts ) * nDimensions;
            Array.Copy( values, k, output, 0, nDimensions );
        }

        private int SubscriptsToIndex( params int[] subscripts )
        {
            int idx = subscripts[ 0 ];
            for( int d = 1; d < nSamples.Length; ++d )
            {
                idx += subscripts[ d ] * nSamples[ d - 1 ];
            }
            return idx;
        }
    }
}

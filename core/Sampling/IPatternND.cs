using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Sampling
{
    public interface IPatternND
    {
        int NumDimensions { get; }

        void GetNumSamples( int[] output );

        void GetSample( float[] output, params int[] indices );
    }
}

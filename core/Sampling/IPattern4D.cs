using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Sampling
{
    public interface IPattern4D
    {
        int Width { get; }
        int Height { get; }

        int NumSamplesPerPixel { get; }

        Vector4f this[ int x, int y, int s ] { get; }
    }
}

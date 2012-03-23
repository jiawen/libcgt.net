using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.ImageProcessing
{
    public interface IImage< T >
    {
        int Width { get; }
        int Height { get; }
        int NumPixels { get; }

        T this[ int k ] { get; set; }
        T this[ int x, int y ] { get; set; }
    }
}

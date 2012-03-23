using System.ComponentModel;
using libcgt.core.Vecmath;

using libcgt.core.ImageProcessing;

namespace libcgt.core
{
    public interface IVideo
    {
        int Width
        {
            get;
        }

        int Height
        {
            get;
        }

        Vector2i Size
        {
            get;
        }        

        int NumFrames
        {
            get;
        }

        float MillisecondsPerFrame
        {
            get;
        }

        Image4ub GetFrame( int t );
    }
}

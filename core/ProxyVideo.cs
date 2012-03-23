using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public class ProxyVideo : IVideo
    {
        private Vector2i size;
        private int nFrames;
        private float millisecondsPerFrame;

        public ProxyVideo( Vector2i size, int nFrames, float millisecondsPerFrame )
        {
            this.size = size;
            this.nFrames = nFrames;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }

        public int Width
        {
            get
            {
                return size.x;
            }
        }

        public int Height
        {
            get
            {
                return size.y;
            }
        }

        public Vector2i Size
        {
            get
            {
                return size;
            }
        }

        public int NumFrames
        {
            get
            {
                return nFrames;
            }
        }

        public float MillisecondsPerFrame
        {
            get
            {
                return millisecondsPerFrame;
            }
        }

        public Image4ub GetFrame( int t )
        {
            throw new System.NotImplementedException();
        }
    }
}

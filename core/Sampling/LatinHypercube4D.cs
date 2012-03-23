using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Sampling
{
    public class LatinHypercube4D : IPattern4D
    {
        private int width;
        private int height;
        private int nx;
        private int ny;

        private float[] samples;

        /// <summary>
        /// Creates a width x height sampling pattern,
        /// where for each pixel, there is an nx x ny 4D latin hypercube pattern.
        /// 
        /// TODO: does nx x ny make *any* sense?        
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="?"></param>
        public LatinHypercube4D( int width, int height,
            int nx, int ny, Random random )
        {
            this.width = width;
            this.height = height;
            this.nx = nx;
            this.ny = ny;

            samples = new float[ width * height * 4 * nx * ny ];

            for( int p = 0; p < width * height; ++p )
            {
                int offset4D = 4 * p * nx * ny;

                SamplingUtils.LatinHypercube( samples, offset4D, nx * ny, 4, random );
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public int NumSamplesPerPixel
        {
            get
            {
                return nx * ny;
            }
        }

        public int SamplesPerPixelX
        {
            get
            {
                return nx;
            }
        }

        public int SamplesPerPixelY
        {
            get
            {
                return ny;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public Vector4f this[ int x, int y, int s ]
        {
            get
            {
                int p = y * width + x;
                int offset4D = 4 * p * nx * ny;

                float xs = samples[ offset4D + 4 * s ];
                float ys = samples[ offset4D + 4 * s + 1 ];
                float us = samples[ offset4D + 4 * s + 2 ];
                float vs = samples[ offset4D + 4 * s + 3 ];

                return new Vector4f( xs, ys, us, vs );
            }
        }
    }
}

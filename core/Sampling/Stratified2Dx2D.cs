using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Sampling
{
    public class Stratified2Dx2D : IPattern4D
    {
        private int width;
        private int height;
        private int nx;
        private int ny;

        private float[] imageSamples;
        private float[] lensSamples;

        /// <summary>
        /// Creates a width x height sampling pattern,
        /// where for each pixel, there is a:
        /// stratified nx x ny pattern (typically over the image),
        /// and a stratified nx x ny pattern (typically over the lens).
        /// 
        /// These two 2D patterns are decorrelated to produce a 4D pattern of
        /// (nx x ny samples per pixel, each of which is 4D).
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="?"></param>
        public Stratified2Dx2D( int width, int height,
            int nx, int ny, bool jitter, Random random )
        {
            this.width = width;
            this.height = height;
            this.nx = nx;
            this.ny = ny;

            imageSamples = new float[ width * height * 2 * nx * ny ];
            lensSamples = new float[ width * height * 2 * nx * ny ];

            for( int p = 0; p < width * height; ++p )
            {
                int offset2D = 2 * p * nx * ny;

                SamplingUtils.StratifiedSample2D( imageSamples, offset2D, nx, ny, jitter, random );
                SamplingUtils.StratifiedSample2D( lensSamples, offset2D, nx, ny, jitter, random );

                SamplingUtils.Shuffle( lensSamples, offset2D, nx * ny, 2, random );
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
                int offset2D = 2 * p * nx * ny;

                float xs = imageSamples[ offset2D + 2 * s ];
                float ys = imageSamples[ offset2D + 2 * s + 1 ];
                float us = lensSamples[ offset2D + 2 * s ];
                float vs = lensSamples[ offset2D + 2 * s + 1 ];

                return new Vector4f( xs, ys, us, vs );
            }
        }
    }
}

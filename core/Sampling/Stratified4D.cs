using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Sampling
{
    public class Stratified4D : IPattern4D
    {
        private int width;
        private int height;

        private int nx;
        private int ny;
        private int nu;
        private int nv;
        
        private float[] samples;

        public Stratified4D( int width, int height,
            int nx, int ny, int nu, int nv,
            bool jitter, Random random )
        {
            this.width = width;
            this.height = height;

            this.nx = nx;
            this.ny = ny;
            this.nu = nu;
            this.nv = nv;

            int np = width * height;
            int spp = NumSamplesPerPixel;
            int nSamplesTotal = np * spp;

            int nFloatsTotal = 4 * nSamplesTotal;
            samples = new float[ nFloatsTotal ];
            
            for( int p = 0; p < np; ++p )
            {
                int offset = 4 * p * spp;
                SamplingUtils.StratifiedSample4D( samples, offset,
                    nx, ny, nu, nv, jitter, random );
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
                return nx * ny * nu * nv;
            }
        }

        public Vector4i SamplesPerPixel
        {
            get
            {
                return new Vector4i( nx, ny, nu, nv );
            }
        }

        public Vector4f this[ int x, int y, int s ]
        {
            get
            {
                int p = y * width + x;
                int offset = 4 * p * NumSamplesPerPixel;

                return new Vector4f
                    (
                    samples[ offset ],
                    samples[ offset + 1 ],
                    samples[ offset + 2 ],
                    samples[ offset + 3 ]
                    );
            }
        }
    }
}

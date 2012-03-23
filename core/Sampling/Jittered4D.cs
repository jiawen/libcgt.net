using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Sampling
{
    public class Jittered4D : IPattern4D
    {
        private int width;
        private int height;
        private int nu;
        private int nv;

        private float[] samples;

        public Jittered4D( int width, int height, int nu, int nv,
            bool jitter, Random random )
        {
            this.width = width;
            this.height = height;
            this.nu = nu;
            this.nv = nv;

            samples = new float[ 4 * width * height * nu * nv ];

            int k = 0;
            for( int y = 0; y < height; ++y )
            {
                for( int x = 0; x < width; ++x )
                {
                    for( int v = 0; v < nv; ++v )
                    {
                        for( int u = 0; u < nu; ++u )
                        {
                            float r0 = jitter ? random.NextFloat() : 0.5f;
                            float r1 = jitter ? random.NextFloat() : 0.5f;
                            float r2 = jitter ? random.NextFloat() : 0.5f;
                            float r3 = jitter ? random.NextFloat() : 0.5f;

                            samples[ k ] = r0;
                            samples[ k + 1 ] = r1;
                            samples[ k + 2 ] = r2;
                            samples[ k + 3 ] = r3;

                            k += 4;
                        }
                    }
                }
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

        public int NU
        {
            get
            {
                return nu;
            }
        }

        public int NV
        {
            get
            {
                return nv;
            }
        }

        public int NumSamplesPerPixel
        {
            get
            {
                return nu * nv;
            }
        }

        public Vector4f this[ int x, int y, int s ]
        {
            get
            {
                // int offset = 4 * ( ( y * width + x ) * nu * nv + s );
                int offset = 4 * ( y * width * nu * nv + x * nu * nv + s );

                return new Vector4f
                (
                    samples[ offset ],
                    samples[ offset + 1 ],
                    samples[ offset + 2 ],
                    samples[ offset + 3 ]
                );
            }
        }

        public Vector4f this[ int x, int y, int u, int v ]
        {
            get
            {
                //int offset = 4 * ( ( y * width + x ) * nu * nv + v * nu + u );
                int offset = 4 * ( y * width * nu * nv + x * nu * nv + v * nu + u );

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

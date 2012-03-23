using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.Vecmath;

namespace libcgt.core.Sampling
{
    public static class SamplingUtils
    {
        /// <summary>
        /// Given nx strata in [0,1],
        /// Place a sample in each one,
        /// starting from samples[ offset ]
        /// and ending at samples[ offset + nx - 1 ]
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="jitter"></param>
        public static void StratifiedSample1D( float[] samples,
            int offset, int nx,
            bool jitter, Random random )
        {
            float invTot = 1.0f / nx;
            for( int i = 0; i < nx; ++i )
            {
                float j = jitter ? random.NextFloat() : 0.5f;
                samples[ offset + i ] = ( i + j ) * invTot;
            }
        }

        /// <summary>
        /// Given nx x ny strata in [0,1]^2
        /// Place a sample in each one,
        /// starting at samples[ offset ],
        /// and ending at samples[ offset + nx * ny - 1 ]
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="jitter"></param>
        public static void StratifiedSample2D( float[] samples,
            int offset, int nx, int ny,
            bool jitter, Random random )
        {
            float dx = 1.0f / nx;
            float dy = 1.0f / ny;

            int k = offset;
            for( int y = 0; y < ny; ++y )
            {
                for( int x = 0; x < nx; ++x )
                {
                    float jx = jitter ? random.NextFloat() : 0.5f;
                    float jy = jitter ? random.NextFloat() : 0.5f;

                    samples[ k ] = ( x + jx ) * dx;
                    samples[ k + 1 ] = ( y + jy ) * dy;

                    k += 2;
                }
            }
        }

        public static void StratifiedSample4D( float[] samples,
            int offset, int nx, int ny, int nu, int nv,
            bool jitter, Random random )
        {
            float dx = 1.0f / nx;
            float dy = 1.0f / ny;
            float du = 1.0f / nu;
            float dv = 1.0f / nv;

            int k = offset;
            for( int v = 0; v < nv; ++v )
            {
                for( int u = 0; u < nu; ++u )
                {
                    for( int y = 0; y < ny; ++y )
                    {
                        for( int x = 0; x < nx; ++x )
                        {
                            float jx = jitter ? random.NextFloat() : 0.5f;
                            float jy = jitter ? random.NextFloat() : 0.5f;
                            float ju = jitter ? random.NextFloat() : 0.5f;
                            float jv = jitter ? random.NextFloat() : 0.5f;

                            samples[ k ] = ( x + jx ) * dx;
                            samples[ k + 1 ] = ( y + jy ) * dy;
                            samples[ k + 2 ] = ( u + ju ) * du;
                            samples[ k + 3 ] = ( v + jv ) * dv;

                            k += 4;
                        }
                    }
                }
            }
        }

        public static void LatinHypercube( float[] samples, int offset, int nSamples, int nDim,
            Random random )
        {
            // generate samples along the diagonal
            float delta = 1.0f / nSamples;
            for( int i = 0; i < nSamples; ++i )
            {
                for( int j = 0; j < nDim; ++j )
                {
                    samples[ offset + nDim * i + j ] = ( i + random.NextFloat() ) * delta;
                }
            }

            // permute samples in each dimension
            for( int i = 0; i < nDim; ++i )
            {
                for( int j = 0; j < nSamples; ++j )
                {
                    int other = random.Next( nSamples );
                    float tmp = samples[ offset + nDim * j + i ];
                    samples[ offset + nDim * j + i ] = samples[ offset + nDim * other + i ];
                    samples[ offset + nDim * other + i ] = tmp;
                }
            }
        }

        /// <summary>
        /// Uniformly samples the unit disc with probability proportional to area
        /// </summary>
        /// <param name="u0"></param>
        /// <param name="u1"></param>
        /// <returns></returns>
        public static Vector2f UniformSampleDisc( float u0, float u1 )
        {
	        double r = Math.Sqrt( u0 );
	        double theta = 2 * Math.PI * u1;

            float x = ( float )( r * Math.Cos( theta ) );
            float y = ( float )( r * Math.Sin( theta ) );

            return new Vector2f( x, y );
        }

        /// <summary>
        /// *Concentrically* samples the unit disc in a concentric stratified pattern
        /// </summary>
        /// <param name="u0"></param>
        /// <param name="u1"></param>
        /// <returns></returns>
        public static Vector2f ConcentricSampleDisc( float u0, float u1 )
        {
            float r;
            float theta;

	        // Map uniform random numbers to [-1, 1]^2
	        float sx = 2 * u0 - 1;
	        float sy = 2 * u1 - 1;

	        // Map square to (r, theta)
	        // Handle degeneracy at the origin
	        if( sx == 0.0 && sy == 0.0 )
	        {
	            return Vector2f.Zero;
	        }
        	
	        if( sx >= -sy )
	        {
		        if( sx > sy )
		        {
			        // Handle first region of disk
			        r = sx;
			        if( sy > 0.0 )
			        {
				        theta = sy / r;
			        }
			        else
			        {
				        theta = 8.0f + sy / r;
			        }
		        }
		        else
		        {
			        // Handle second region of disk
			        r = sy;
			        theta = 2.0f - sx / r;
		        }
	        }
	        else
	        {
		        if( sx <= sy )
		        {
			        // Handle third region of disk
			        r = -sx;
			        theta = 4.0f - sy / r;
		        }
		        else
		        {
			        // Handle fourth region of disk
			        r = -sy;
			        theta = 6.0f + sx / r;
		        }
	        }

            theta *= ( float )( 0.25 * Math.PI );
            float x = ( float )( r * Math.Cos( theta ) );
            float y = ( float )( r * Math.Sin( theta ) );
	        return new Vector2f( x, y );
        }

        /// <summary>
        /// Shuffles samplse[ offset ] through samples[ offset + count  - 1 ]
        /// in dims dimensions.
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="dims"></param>
        /// <param name="random"></param>
        public static void Shuffle( float[] samples, int offset, int count, int dims, Random random )
        {
            for( int i = 0; i < count; ++i )
            {
                int other = random.Next( count );
                for( int j = 0; j < dims; ++j )
                {
                    float tmp = samples[ offset + i * dims + j ];
                    samples[ offset + i * dims + j ] = samples[ offset + dims * other + j ];
                    samples[ offset + dims * other + j ] = tmp;
                }
            }
        }
    }
}

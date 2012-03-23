using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.Sampling
{
    /// <summary>
    /// Tiled in the first two dimensions, stratified in the others.
    /// </summary>
    public class RegularTiled2DStratified4D : IPattern4D
    {
        private int tileSizeX;
        private int tileSizeY;

        private int width;
        private int height;

        private IPattern4D tilePattern;

        public RegularTiled2DStratified4D( int tileSizeX, int tileSizeY,
            int width, int height,
            IPattern4D pattern )
        {
            if ( width % tileSizeX != 0 ||
                height % tileSizeY != 0 )
            {
                throw new ArgumentException( "Width and height must divide tile size." );
            }

            this.tileSizeX = tileSizeX;
            this.tileSizeY = tileSizeY;

            this.width = width;
            this.height = height;

            this.tilePattern = pattern;
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
                return tilePattern.NumSamplesPerPixel;
            }
        }

        public Vector4f this[ int x, int y, int s ]
        {
            get
            {
                int tox = x % tileSizeX;
                int toy = y % tileSizeY;

                return tilePattern[ tox, toy, s ];
            }
        }
    }
}

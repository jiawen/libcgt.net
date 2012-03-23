using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Sampling
{
    /// <summary>
    /// TODO: rename this to ShuffledTiled
    /// Tiled in the first two dimensions, stratified in the others.
    /// </summary>
    public class Tiled2DStratifiedND : IPatternND        
    {
        private int nDimensions;
        private int[] nSamples;

        private int tileSizeX;
        private int tileSizeY;

        private int nUniqueTilesX;
        private int nUniqueTilesY;

        private int regionSizeX;
        private int regionSizeY;

        private List< IPatternND > uniqueTiles;

        // tileIndicesForRegion[ rx, ry ]
        // for region (rx,ry), returns a tile index
        private List< int >[,] tileIndicesForRegion;

        public Tiled2DStratifiedND( Random random,
            int tileSizeX, int tileSizeY,
            int nUniqueTilesX, int nUniqueTilesY,            
            params int[] nSamples )
        {
            this.nDimensions = nSamples.Length;
            this.nSamples = new int[ nDimensions ];
            Array.Copy( nSamples, this.nSamples, nSamples.Length );

            this.tileSizeX = tileSizeX;
            this.tileSizeY = tileSizeY;
            this.nUniqueTilesX = nUniqueTilesX;
            this.nUniqueTilesY = nUniqueTilesY;

            if( nSamples.Length < 2 )
            {
                throw new ArgumentException( "Tiled patterns make no sense in less than 3D" );
            }

            int nx = nSamples[ 0 ];
            int ny = nSamples[ 1 ];

            if( nx % tileSizeX != 0 ||
                ny % tileSizeY != 0 )
            {
                throw new ArgumentException( "Width and height must divide tile size." );
            }

            // figure out the number of samples in each tile
            var tileNSamples = new int[ nSamples.Length ];
            Array.Copy( nSamples, tileNSamples, nSamples.Length );
            tileNSamples[ 0 ] = tileSizeX;
            tileNSamples[ 1 ] = tileSizeY;

            int nUniqueTiles = nUniqueTilesX * nUniqueTilesY;
            uniqueTiles = new List< IPatternND >( nUniqueTiles );
            for( int t = 0; t < nUniqueTiles; ++t )
            {
                var tile = new StratifiedND( random, tileNSamples );
                uniqueTiles.Add( tile );
            }

            // now spread the tiles over the screen
            regionSizeX = tileSizeX * nUniqueTilesX;
            regionSizeY = tileSizeY * nUniqueTilesY;

            if( nx % regionSizeX != 0 ||
                ny % regionSizeY != 0 )
            {
                var msg =
                    string.Format(
                        "Width and height must divide region size.  width = {0}, height = {1}, regionSizeX = {2}, regionSizeY = {3}",
                        nx, ny, regionSizeX, regionSizeY );
                throw new ArgumentException( msg );
            }

            int nRegionsX = nSamples[ 0 ] / regionSizeX;
            int nRegionsY = nSamples[ 1 ] / regionSizeY;

            tileIndicesForRegion = new List< int >[ nRegionsX, nRegionsY ];

            for( int ry = 0; ry < nRegionsY; ++ry )
            {
                for( int rx = 0; rx < nRegionsX; ++rx )
                {
                    var tileIndices = Arithmetic.IntegerSlice( nUniqueTiles ).ToList();
                    tileIndices.Shuffle( random );
                    tileIndicesForRegion[ rx, ry ] = tileIndices;
                }
            }
        }

        public int NumDimensions
        {
            get
            {
                return nDimensions;
            }
        }

        public void GetNumSamples( int[] output )
        {
            Array.Copy( nSamples, output, nSamples.Length );
        }

        public void GetSample( float[] output, params int[] indices )
        {
            int gx = indices[ 0 ];
            int gy = indices[ 1 ];

            // figure out which region I'm in
            int rx = gx / regionSizeX;
            int ry = gy / regionSizeY;

            // figure out which tile I'm in within the region
            int rox = ( gx % regionSizeX );
            int roy = ( gy % regionSizeY );
 
            int tx = rox / tileSizeX;
            int ty = roy / tileSizeY;

            // get the tile
            var tile = GetTile( rx, ry, tx, ty );
            
            // figure out which sample I am within the tile
            int tox = rox % tileSizeX;
            int toy = roy % tileSizeY;

            int[] tileIndices = new int[ indices.Length ];
            Array.Copy( indices, tileIndices, indices.Length );
            tileIndices[ 0 ] = tox;
            tileIndices[ 1 ] = toy;

            tile.GetSample( output, tileIndices );
        }

        private IPatternND GetTile( int rx, int ry, int tx, int ty )
        {
            int t = ty * nUniqueTilesX + tx;
            return uniqueTiles[ tileIndicesForRegion[ rx, ry ][ t ] ];
        }
    }
}

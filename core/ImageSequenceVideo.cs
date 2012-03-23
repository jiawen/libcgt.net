using System;
using System.ComponentModel;
using System.IO;

using libcgt.core.Vecmath;
using libcgt.core.ImageProcessing;

namespace libcgt.core
{
    public class ImageSequenceVideo : IVideo
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int NumFrames { get; private set; }

        public float MillisecondsPerFrame { get; private set; }

        private string[] filenames;
        private LRUCache< int, Image4ub > frameCache;

        public ImageSequenceVideo( string directory, string pattern )
            : this( directory, pattern, 24 )
        {

        }

        public ImageSequenceVideo( string directory, string pattern,            
            float millisecondsPerFrame )
            : this( directory, pattern, millisecondsPerFrame, -1, -1 )
        {
            
        }

        // TODO: -1, -1 is a bit of a hack
        public ImageSequenceVideo( string directory, string pattern,
            float millisecondsPerFrame, int tStart, int nFrames )
        {
            if( millisecondsPerFrame < 0 )
            {
                throw new ArgumentException( "millisecondsPerFrame must be greater than 0" );
            }
            MillisecondsPerFrame = millisecondsPerFrame;

            // throw exceptions if:
            // directory not found
            // files not found in directory

            if( !( Directory.Exists( directory ) ) )
            {
                throw new DirectoryNotFoundException( directory );
            }            

            filenames = Directory.GetFiles( directory, pattern, SearchOption.TopDirectoryOnly );
            if( filenames.Length < 1 )
            {
                throw new FileNotFoundException( "directory must contain at least one file" );
            }
            Array.Sort( filenames );            

            if( tStart >= 0 && nFrames > 0 )
            {
                filenames = filenames.Slice( tStart, tStart + nFrames );
            }
            NumFrames = filenames.Length;

            // load the first file just to get the size
            var img = Image4ub.FromFile( filenames[ 0 ] );
            Width = img.Width;
            Height = img.Height;

            frameCache = new LRUCache< int, Image4ub >( NumFrames );
            frameCache.ReadMissEvent += FrameReadMissEvent;            
        }

        private void FrameReadMissEvent( int tag, ref Image4ub data )
        {
            var filename = filenames[ tag ];
            data = Image4ub.FromFile( filename );
        }

#if false
        public ImageSequenceVideo( Image4ub[] images,
            float millisecondsPerFrame )
        {
            Width = images[ 0 ].Width;
            Height = images[ 0 ].Height;
            NumFrames = images.Length;

            MillisecondsPerFrame = millisecondsPerFrame;

            frameCache = new ImageSequenceCache( Width, Height, null ); // HACK
            for( int t = 0; t < NumFrames; ++t )
            {
                frameCache.InsertEntry( t, images[ t ] );
                frameCache.MarkClean( t );
            }
        }
#endif

        public Vector2i Size
        {
            get
            {
                return new Vector2i( Width, Height );
            }
        }
       
        public int LastFrame
        {
            get
            {
                return NumFrames - 1;
            }
        }

        public Image4ub GetFrame( int t )
        {
            return frameCache[ t ];
        }
    }
}

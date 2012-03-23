using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt.core.ImageProcessing;

namespace libcgt.core
{
    public class ImageSequenceCache : AbstractCache< int, Image4ub >
    {
        // for performance, store a buffer of size 4 * width * height
        private byte[] loadingBuffer;
        private string[] filenames;

        public ImageSequenceCache( int width, int height, string[] filenames )
            : base()
        {
            loadingBuffer = new byte[ 4 * width * height ];
            this.filenames = filenames;
        }

        protected override void UpdateEntry( int key, ref Image4ub entry )
        {
            string filename = filenames[ key ];
            entry = Image4ub.FromFile( filename );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.IO
{
    /// <summary>
    /// Utility class for building sequences of numbered filenames
    /// </summary>
    public class FilenameBuilder
    {
        private string prefix;
        private string suffix;
        private int nDigits;

        // TODO: make it an Enumerable?
        public FilenameBuilder( string prefix, string suffix, int nDigits )
        {
            this.prefix = prefix;
            this.suffix = suffix;
            this.nDigits = nDigits;
        }

        public string GetFilename( int i )
        {
            string formatString = prefix + "{0:D" + nDigits + "}" + suffix;
            return string.Format( formatString, i );
        }
    }
}

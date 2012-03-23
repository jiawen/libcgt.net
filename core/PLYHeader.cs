using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.core
{
    public enum PLYScalarType
    {
        CHAR,
        BYTE,
        SHORT,
        USHORT,
        INT,
        UINT,
        FLOAT,
        DOUBLE
    }

    public enum PLYFileMode
    {
        ASCII,
        BINARY_LITTLE_ENDIAN,
        BINARY_BIG_ENDIAN
    }

    public class PLYHeader
    {
        private List< PLYElementDescription > elementDescriptions;

        public PLYHeader()
        {
            elementDescriptions = new List< PLYElementDescription >();
        }

        public void AddElementDescription( PLYElementDescription elementDescription )
        {
            elementDescriptions.Add( elementDescription );
        }

        public List< PLYElementDescription > ElementDescriptions
        {
            get
            {
                return elementDescriptions;
            }
        }
    }
}

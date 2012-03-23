using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public class PLYElementDescription
    {
        public string ElementName { get; set; }
        public int NumInstances { get; set; }

        private List< string > propertyNames; // ordered list of property names

        // whether each property is:
        // a list
        // its list length type (if it's a list)
        // its data type
        private Dictionary< string, bool > propertyIsList;
        private Dictionary< string, PLYScalarType > propertyLengthTypes;
        private Dictionary< string, PLYScalarType > propertyDataTypes;

        public PLYElementDescription( string elementName, int nInstances )
        {
            ElementName = elementName;
            NumInstances = nInstances;

            propertyNames = new List< string >();
            propertyIsList = new Dictionary< string, bool >();
            propertyLengthTypes = new Dictionary< string, PLYScalarType >();
            propertyDataTypes = new Dictionary< string, PLYScalarType >();
        }

        public void AddScalarProperty( string propertyName, PLYScalarType dataType )
        {
            propertyIsList[ propertyName ] = false;
            propertyNames.Add( propertyName );
            propertyDataTypes[ propertyName ] = dataType;
        }

        public void AddListProperty( string propertyName,
            PLYScalarType lengthType, PLYScalarType dataType )
        {
            propertyIsList[ propertyName ] = true;
            propertyNames.Add( propertyName );
            propertyLengthTypes[ propertyName ] = lengthType;
            propertyDataTypes[ propertyName ] = dataType;
        }

        public List< string > PropertyNames
        {
            get
            {
                return propertyNames;
            }
        }

        public bool IsList( string propertyName )
        {
            return propertyIsList[ propertyName ];
        }

        public PLYScalarType PropertyLengthType( string propertyName )
        {
            return propertyLengthTypes[ propertyName ];
        }

        public PLYScalarType PropertyDataType( string propertyName )
        {
            return propertyDataTypes[ propertyName ];
        }
    }
}

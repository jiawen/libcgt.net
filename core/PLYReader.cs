using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public class PLYReader
    {
        public delegate void HeaderReadHandler( PLYHeader plyHeader );

        public delegate void ElementHandler( string elementName, int instanceId );
        public delegate void PropertyHandler( string elementName, string propertyName, int listLength );        
        public delegate void DataHandler< T >( string elementName, string propertyName, int listIndex, T value );

        private string filename;
        private PLYFileMode fileMode;        
        private StreamReader streamReader;
        private BinaryReader binaryReader;
        private int dataBegin;

        private PLYHeader plyHeader;        

        public event HeaderReadHandler HeaderRead;

        public event ElementHandler ElementBegin;
        public event ElementHandler ElementEnd;

        public event PropertyHandler PropertyBegin;
        public event PropertyHandler PropertyEnd;

        public event DataHandler< byte > ByteRead;
        public event DataHandler< int > IntRead;
        public event DataHandler< float > FloatRead;
        public event DataHandler< double > DoubleRead;

        public PLYReader( string filename )
        {
            this.filename = filename;
            this.dataBegin = 0;
        }

        public void Load()
        {
            streamReader = new StreamReader( filename );

            // read header
            ReadHeader();

            // read element declaractions
            ReadElementDeclarations();

            OnHeaderRead( plyHeader );

            if( fileMode == PLYFileMode.BINARY_LITTLE_ENDIAN )
            {
                streamReader.Dispose();
                FileStream fs = new FileStream( filename, FileMode.Open, FileAccess.Read );
                fs.Seek( dataBegin, SeekOrigin.Begin );
                binaryReader = new BinaryReader( fs );

                // read elements in sequence
                foreach( PLYElementDescription elementDescription in plyHeader.ElementDescriptions )
                {
                    string elementName = elementDescription.ElementName;
                    int nInstances = elementDescription.NumInstances;
                    for( int i = 0; i < nInstances; ++i )
                    {
                        OnElementBegin( elementName, i );
                        ReadElementInstanceBinary( elementDescription );
                        OnElementEnd( elementName, i );
                    }
                }
            }
            else
            {
                // read the whole thing into memory and start splitting
                string rest = streamReader.ReadToEnd();

                streamReader.Dispose();
            }
        }

        private void ReadHeader()
        {
            plyHeader = new PLYHeader();
            string magic = streamReader.ReadLine();
            if( magic != "ply" )
            {
                throw new InvalidDataException( "PLY files must have \"ply\" as the first line." );
            }
            dataBegin += 1 + magic.Length;

            string formatLine = streamReader.ReadLine();
            string[] delimiters = new string[] { " " };
            string[] formatTokens = formatLine.Split( delimiters, StringSplitOptions.RemoveEmptyEntries );

            dataBegin += 1 + formatLine.Length;

            if( formatTokens.Length != 3 )
            {
                throw new InvalidDataException( "Invalid format line: " + formatLine );
            }

            if( formatTokens[ 0 ] != "format" )
            {
                throw new InvalidDataException( "Invalid format line: " + formatLine );
            }

            switch( formatTokens[ 1 ] )
            {
                case "ascii":
                    fileMode = PLYFileMode.ASCII;
                    break;

                case "binary_little_endian":
                    fileMode = PLYFileMode.BINARY_LITTLE_ENDIAN;
                    break;

                case "binary_big_endian":
                    fileMode = PLYFileMode.BINARY_BIG_ENDIAN;
                    throw new InvalidDataException( "Reading binary_big_endian is not supported." );

                default:
                    throw new InvalidDataException( "Invalid format line: " + formatLine );
            }
        }

        private void ReadElementDeclarations()
        {
            PLYElementDescription currentElement = null;

            string line = streamReader.ReadLine();
            dataBegin += 1 + line.Length;
            while( line != null && line != "end_header" )
            {
                string[] delimiters = new string[] { " " };
                string[] tokens = line.Split( delimiters, StringSplitOptions.RemoveEmptyEntries );
                switch( tokens[ 0 ] )
                {
                    // on a comment, do nothing
                    case "comment":
                        break;

                    // on an element declaration
                    // append the previous one (unless it's the first)
                    // and create a new one
                    case "element":

                        if( currentElement != null )
                        {
                            plyHeader.AddElementDescription( currentElement );
                        }

                        // and parse the new one
                        if( tokens.Length != 3 )
                        {
                            throw new InvalidDataException( "Invalid element declaration: " + line );
                        }

                        string elementName = tokens[ 1 ];
                        int nInstances = int.Parse( tokens[ 2 ] );
                        currentElement = new PLYElementDescription( elementName, nInstances );

                        break;

                    // on a property declaration
                    // append the property to the current element
                    case "property":

                        if( tokens.Length < 3 )
                        {
                            throw new InvalidDataException( "Invalid property declaration: " + line );
                        }

                        // read the property type
                        string propertyType = tokens[ 1 ];
                        if( propertyType == "list" )
                        {
                            if( tokens.Length < 5 )
                            {
                                throw new InvalidDataException( "Invalid property declaration: " + line );
                            }

                            PLYScalarType lengthType = scalarTypeFromString( tokens[ 2 ] );
                            if( lengthType == PLYScalarType.FLOAT ||
                                lengthType == PLYScalarType.DOUBLE )
                            {
                                throw new InvalidDataException( "List length must be an integer type.  Found: " + tokens[ 2 ] );
                            }

                            PLYScalarType dataType = scalarTypeFromString( tokens[ 3 ] );
                            string propertyName = tokens[ 4 ];

                            currentElement.AddListProperty( propertyName, lengthType, dataType );
                        }
                        else
                        {
                            PLYScalarType dataType = scalarTypeFromString( propertyType );
                            string propertyName = tokens[ 2 ];

                            currentElement.AddScalarProperty( propertyName, dataType );
                        }

                        break;
                }

                line = streamReader.ReadLine();
                dataBegin += 1 + line.Length;
            }

            if( currentElement != null )
            {
                plyHeader.AddElementDescription( currentElement );
            }
        }

        private PLYScalarType scalarTypeFromString( string s )
        {
            switch( s )
            {
                case "char":
                    return PLYScalarType.CHAR;

                case "uchar":
                    return PLYScalarType.BYTE;

                case "short":
                    return PLYScalarType.SHORT;

                case "ushort":
                    return PLYScalarType.USHORT;

                case "int":
                    return PLYScalarType.INT;

                case "uint":
                    return PLYScalarType.UINT;

                case "float":
                    return PLYScalarType.FLOAT;

                case "double":
                    return PLYScalarType.DOUBLE;

                default:
                    throw new InvalidDataException( "Invalid PLY type: " + s );
            }
        }

        private void ReadElementInstanceBinary( PLYElementDescription elementDescription )
        {
            string elementName = elementDescription.ElementName;
            foreach( string propertyName in elementDescription.PropertyNames )
            {
                int listLength = ReadListLength( elementDescription, propertyName );
                OnPropertyBegin( elementName, propertyName, listLength );

                PLYScalarType dataType = elementDescription.PropertyDataType( propertyName );
                for( int i = 0; i < listLength; ++i )
                {
                    switch( dataType )
                    {
                        case PLYScalarType.BYTE:
                        {
                            byte val = ReadByteBinary();
                            OnByteRead( elementName, propertyName, i, val );
                            break;
                        }

                        case PLYScalarType.INT:
                        {
                            int val = ReadIntBinary();
                            OnIntRead( elementName, propertyName, i, val );
                            break;
                        }

                        case PLYScalarType.FLOAT:
                        {
                            float val = ReadFloatBinary();
                            OnFloatRead( elementName, propertyName, i, val );
                            break;
                        }

                        case PLYScalarType.DOUBLE:
                        {
                            double val = ReadDoubleBinary();
                            OnDoubleRead( elementName, propertyName, i, val );
                            break;
                        }

                        default:
                            throw new NotImplementedException( "Sorry, I need to implement reading other types" );
                    }
                }

                OnPropertyEnd( elementName, propertyName, listLength );
            }
        }

        private int ReadListLength( PLYElementDescription elementDescription, string propertyName )
        {
            int listLength = 1;
            if( elementDescription.IsList( propertyName ) )
            {
                PLYScalarType lengthType = elementDescription.PropertyLengthType( propertyName );
                switch( lengthType )
                {
                    case PLYScalarType.CHAR:
                        listLength = ( int )( ReadCharBinary() );
                        break;
                    
                    case PLYScalarType.BYTE:
                        listLength = ( int )( ReadByteBinary() );
                        break;
                        
                    case PLYScalarType.SHORT:
                        listLength = ( int )( ReadShortBinary() );
                        break;
                    
                    case PLYScalarType.USHORT:
                        listLength = ( int )( ReadUShortBinary() );
                        break;

                    case PLYScalarType.INT:
                        listLength = ReadIntBinary();
                        break;

                    case PLYScalarType.UINT:
                        listLength = ( int )( ReadUIntBinary() );
                        break;

                    default:
                        throw new InvalidDataException( "List length must be an integer type." );
                }
            }

            return listLength;
        }

        private char ReadCharBinary()
        {
            return ( char )( binaryReader.ReadSByte() );
        }

        private byte ReadByteBinary()
        {
            return binaryReader.ReadByte();
        }

        private short ReadShortBinary()
        {
            return binaryReader.ReadInt16();
        }

        private ushort ReadUShortBinary()
        {
            return binaryReader.ReadUInt16();
        }

        private int ReadIntBinary()
        {
            return binaryReader.ReadInt32();
        }

        private uint ReadUIntBinary()
        {
            return binaryReader.ReadUInt32();
        }

        private float ReadFloatBinary()
        {
            return binaryReader.ReadSingle();
        }

        private double ReadDoubleBinary()
        {
            return binaryReader.ReadDouble();
        }

        // event triggers
        private void OnHeaderRead( PLYHeader plyHeader )
        {
            if( HeaderRead != null )
            {
                HeaderRead( plyHeader );
            }
        }

        private void OnElementBegin( string elementName, int instanceId )
        {
            if( ElementBegin != null )
            {
                ElementBegin( elementName, instanceId );
            }
        }

        private void OnElementEnd( string elementName, int instanceId )
        {
            if( ElementEnd != null )
            {
                ElementEnd( elementName, instanceId );
            }
        }

        private void OnPropertyBegin( string elementName, string propertyName, int listLength )
        {
            if( PropertyBegin != null )
            {
                PropertyBegin( elementName, propertyName, listLength );
            }
        }
        
        private void OnPropertyEnd( string elementName, string propertyName, int listLength )
        {
            if( PropertyEnd != null )
            {
                PropertyEnd( elementName, propertyName, listLength );
            }
        }

        private void OnByteRead( string elementName, string propertyName, int listIndex, byte value )
        {
            if( ByteRead != null )
            {
                ByteRead( elementName, propertyName, listIndex, value );
            }
        }

        private void OnIntRead( string elementName, string propertyName, int listIndex, int value )
        {
            if( IntRead != null )
            {
                IntRead( elementName, propertyName, listIndex, value );
            }
        }

        private void OnFloatRead( string elementName, string propertyName, int listIndex, float value )
        {
            if( FloatRead != null )
            {
                FloatRead( elementName, propertyName, listIndex, value );
            }
        }

        private void OnDoubleRead( string elementName, string propertyName, int listIndex, double value )
        {
            if( DoubleRead != null )
            {
                DoubleRead( elementName, propertyName, listIndex, value );
            }
        }
    }
}

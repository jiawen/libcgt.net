using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public class PLYWriter
    {
        private string filename;
        private PLYFileMode plyFileMode;

        private List< PLYElementDescription > elements;
        private FileStream stream;
        private StreamWriter asciiWriter;
        private BinaryWriter binaryWriter;

        public PLYWriter( string filename, PLYFileMode plyFileMode )
        {
            this.filename = filename;
            this.plyFileMode = plyFileMode;

            this.elements = new List< PLYElementDescription >();
        }

        public void AddElement( PLYElementDescription element )
        {
            elements.Add( element );
        }

        public void Open()
        {
            // TODO: open a local file, throw an exception if called twice, etc

            // open the file
            stream = new FileStream( filename, FileMode.Create, FileAccess.Write );

            // write the header
            using( StreamWriter headerWriter = new StreamWriter( stream, Encoding.ASCII ) )
            {
                headerWriter.NewLine = "\n";
                headerWriter.WriteLine( "ply" );
                switch( plyFileMode )
                {
                    case PLYFileMode.ASCII:
                        headerWriter.WriteLine( "format ascii 1.0" );
                        break;

                    case PLYFileMode.BINARY_LITTLE_ENDIAN:
                        headerWriter.WriteLine( "format binary_little_endian 1.0" );
                        break;

                    case PLYFileMode.BINARY_BIG_ENDIAN:
                        headerWriter.WriteLine( "format binary_big_endian 1.0" );
                        break;

                    default:
                        throw new ArgumentException( "Cannot reach here" );
                }

                foreach( var ed in elements )
                {
                    headerWriter.WriteLine( "element {0} {1}", ed.ElementName, ed.NumInstances );
                    foreach( var property in ed.PropertyNames )
                    {
                        if( ed.IsList( property ) )
                        {
                            headerWriter.WriteLine( "property list {0} {1} {2}",
                                PLYScalarTypeToString( ed.PropertyLengthType( property ) ),
                                PLYScalarTypeToString( ed.PropertyDataType( property ) ),
                                property );
                        }
                        else
                        {
                            headerWriter.WriteLine( "property {0} {1}",
                                PLYScalarTypeToString( ed.PropertyDataType( property ) ),
                                property );
                        }
                    }
                }
                headerWriter.WriteLine( "end_header" );

                headerWriter.Close(); // close the stream as well
            }

            // re-open a new stream based on the type
            stream = new FileStream( filename, FileMode.Append, FileAccess.Write );
            switch( plyFileMode )
                {
                    case PLYFileMode.ASCII:
                        asciiWriter = new StreamWriter( stream, Encoding.ASCII );
                        asciiWriter.NewLine = "\n";
                        break;

                    case PLYFileMode.BINARY_LITTLE_ENDIAN:
                        binaryWriter = new BinaryWriter( stream );
                        break;

                    default:
                        throw new NotImplementedException( "Cannot reach here" );
                }
        }

        public void Write( char val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void Write( byte val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void Write( short val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void Write( ushort val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void Write( int val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void Write( uint val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void Write( float val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void Write( double val )
        {
            switch( plyFileMode )
            {
                case PLYFileMode.ASCII:
                    asciiWriter.Write( "{0} ", val );
                    break;

                case PLYFileMode.BINARY_LITTLE_ENDIAN:
                    binaryWriter.Write( val );
                    break;

                case PLYFileMode.BINARY_BIG_ENDIAN:
                    throw new NotImplementedException( "Big endian binary not implemented." );
            }
        }

        public void WriteLine()
        {
            if( plyFileMode != PLYFileMode.ASCII )
            {
                throw new ArgumentException( "Cannot call WriteLine() in binary mode." );
            }

            asciiWriter.WriteLine();
        }

        public void Close()
        {
            switch( plyFileMode )
                {
                    case PLYFileMode.ASCII:
                        asciiWriter = new StreamWriter( stream, Encoding.ASCII );
                        break;

                    default:
                        binaryWriter.Close();
                        break;
                }
        }

        private static string PLYScalarTypeToString( PLYScalarType scalarType )
        {
            switch( scalarType )
            {
                case PLYScalarType.CHAR:
                    return "char";
                
                case PLYScalarType.BYTE:
                    return "uchar";
                
                case PLYScalarType.SHORT:
                    return "short";
                
                case PLYScalarType.USHORT:
                    return "ushort";
                
                case PLYScalarType.INT:
                    return "int";
                
                case PLYScalarType.UINT:
                    return "uint";
                
                case PLYScalarType.FLOAT:
                    return "float";
                
                case PLYScalarType.DOUBLE:
                    return "double";
                    
                default:
                    throw new ArgumentException( "Cannot reach here." );
            }
        }
    }
}

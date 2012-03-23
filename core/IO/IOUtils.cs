using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace libcgt.core.IO
{
    public static class IOUtils
    {
        /// <summary>
        /// If directory ends with either a '/' or '\', then it's simply returned.
        /// Otherwise, a '/' is appended and returned.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string AppendSlashIfDirectoryDoesNotEndWithSlash( string directory )
        {
            if( !directory.EndsWith( "/" ) && !directory.EndsWith( "\\" ) )
            {
                directory = directory + "/";
            }
            return directory;
        }

        /// <summary>
        /// Creates a directory "directory" if it doesn't exist.
        /// Returns true if a directory was created,
        /// and returns false if the directory already exists.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool CreateDirectoryIfDoesNotExist( string directory )
        {
            if( !( Directory.Exists( directory ) ) )
            {
                Directory.CreateDirectory( directory );
                return true;
            }
            return false;
        }

        public static object LoadObjectBinary( string filename )
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read );
            object obj = formatter.Deserialize( stream );
            stream.Close();

            return obj;
        }

        public static void SaveObjectBinary( string filename, object obj )
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream( filename, FileMode.Create, FileAccess.Write, FileShare.None );
            formatter.Serialize( stream, obj );
            stream.Close();
        }

        // TODO: make it an extension method?
        public static string FloatToHexString( float f )
        {
            var bytes = BitConverter.GetBytes( f );
            var sb = new StringBuilder( 8 );
            foreach( byte b in bytes )
            {
                sb.AppendFormat( "{0:x2}", b );
            }
            return sb.ToString();
        }

        public static float HexStringToFloat( string hex )
        {            
            return BitConverter.ToSingle( StringToByteArray( hex ), 0 );
        }

        // TODO: make it an extension method?
        public static string DoubleToHexString( double d )
        {
            var bytes = BitConverter.GetBytes( d );
            var sb = new StringBuilder( 16 );
            foreach( byte b in bytes )
            {
                sb.AppendFormat( "{0:x2}", b );
            }
            return sb.ToString();
        }

        public static double HexStringToDouble( string hex )
        {            
            return BitConverter.ToDouble( StringToByteArray( hex ), 0 );
        }

        public static byte[] StringToByteArray( String hex )
        {
            int nChars = hex.Length;
            byte[] bytes = new byte[ nChars / 2 ];
            for (int i = 0; i < nChars; i += 2)
            {
                bytes[ i / 2 ] = Convert.ToByte( hex.Substring( i, 2 ), 16 );
            }
            return bytes;
        }
    }
}

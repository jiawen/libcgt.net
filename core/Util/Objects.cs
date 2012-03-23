using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Util
{
    // Blatantly ripping off Java 7's java.util.Objects class
    public static class Objects< T > where T : class
    {
        public static T NonNull( T obj )
        {
            return NonNull( obj, "" );
        }

        public static T NonNull( T obj, string message )
        {
            if( obj != null )
            {
                return obj;
            }
            throw new NullReferenceException( message );
        }

        public static string ToString( object obj )
        {
            return ToString( obj, "null" );
        }

        public static string ToString( object obj, string nullDefault )
        {
            if( obj != null )
            {
                return obj.ToString();
            }
            return nullDefault;
        }

        public static int Hash( params object[] values )
        {
            int result = 17;
            foreach( var obj in values )
            {
                result = 31 * result + GetHashCode( obj );
            }
            return result;
        }

        public static int GetHashCode( object obj )
        {
            if( obj == null )
            {
                return 0;
            }
            return obj.GetHashCode();
        }

        public new static bool Equals( object a, object b )
        {
            // if a is null and so is b, then they're equal
            // else, a is null and b is not, so they're not equal
            if( a == null )
            {
                return ( b == null );
            }
            // a is not null
            if( b == null )
            {
                return false;
            }
            return a.Equals( b );
        }

        // boolean deepEquals( Object a, Object b)
        // Almost the same as the first method except that if both a and b are arrays, the equality is evaluated using Arrays.deepEquals method.


    }
}

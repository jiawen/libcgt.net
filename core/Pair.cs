using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public static class Pair
    {
        public static Pair< T1, T2 > Create< T1, T2 >( T1 first, T2 second )
        {
            return new Pair< T1, T2 >( first, second );
        }

        public static Pair< T1, T2 > Copy< T1, T2 >( Pair< T1, T2 > p )
        {
            return new Pair< T1, T2 >( p );
        }
    }

    public class Pair< T1, T2 >
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        
        public Pair( T1 first, T2 second )
        {
            First = first;
            Second = second;
        }

        public Pair( Pair< T1, T2 > p )
        {
            First = p.First;
            Second = p.Second;
        }        

        public Pair< T2, T1 > Flipped()
        {
            return new Pair< T2, T1 >( Second, First );
        }        

        public override int GetHashCode()
        {
            int h1 = First.GetHashCode();
            int h2 = Second.GetHashCode();
            return ( ( h1 << 16 ) | ( h1 >> 16 ) ) ^ h2;
        }

        public override bool Equals( object obj )
        {
            if( obj is Pair< T1, T2 > )
            {
                var o = ( Pair< T1, T2 > )obj;
                return First.Equals( o.First ) && Second.Equals( o.Second );
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format( "Pair( {0}, {1} )", First.ToString(), Second.ToString() );
        }
    }
}

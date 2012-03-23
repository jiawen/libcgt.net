using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public static class ListExtensions
    {
        public static bool IsEmpty< T >( this List< T > list )
        {
            return( list.Count == 0 );
        }

        public static void RemoveLastIfNotEmpty< T >( this List< T > list )
        {
            if( list.IsEmpty() )
            {
                return;
            }
            else
            {
                list.RemoveLast();
            }
        }

        public static void RemoveLast< T >( this List< T > list )
        {
            var i = list.Count - 1;
            list.RemoveAt( i );
        }
        
        public static void Shuffle< T >( this List< T > list, Random random )
        {            
            int n = list.Count;
            for( int i = 0; i < n; ++i )
            {
                int r = i + random.Next( n - i );
         
                var tmp = list[ r ];
                list[ r ] = list[ i ];
                list[ i ] = tmp;
            }
        }

        public static float Select( this List< float > list, int k )
        {
            for( int i = 0; i < k + 1; ++i )
            {
                int minIndex = i;
                float minValue = list[ i ];
                for( int j = i + 1; j < list.Count; ++j )
                {
                    if( list[ j ] < minValue )
                    {
                        minIndex = j;
                        minValue = list[ j ];
                    }
                    
                    // swap list[ i ] and list[ minIndex ];
                    var tmp = list[ i ];
                    list[ i ] = list[ minIndex ];
                    list[ minIndex ] = tmp;
                }
            }

            return list[ k ];
        }

        public static T Select< T >( List< T > list, int k ) where T : IComparable< T >
        {
            for( int i = 0; i < k + 1; ++i )
            {
                int minIndex = i;
                var minValue = list[ i ];
                for( int j = i + 1; j < list.Count; ++j )
                {
                    if( list[ j ].CompareTo( minValue ) < 0 )
                    {
                        minIndex = j;
                        minValue = list[ j ];
                    }
                    
                    // swap list[ i ] and list[ minIndex ];
                    var tmp = list[ i ];
                    list[ i ] = list[ minIndex ];
                    list[ minIndex ] = tmp;
                }
            }

            return list[ k ];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public static class SimpleAlgorithms
    {
        public static void Swap< T >( ref T x, ref T y )
        {
            T z = y;
            y = x;
            x = z;
        }

        public static int MaxIndex< TElement >( this IList< TElement > elements,
            Func< TElement, float > selector )
        {
            float f;
            return MaxIndex( elements, selector, out f );
        }
        
        public static int MaxIndex< TElement, TValue >( this IList< TElement > elements,
            Func< TElement, TValue > selector, out TValue maxValue ) where TValue : IComparable< TValue >
        {
            if( elements.IsEmpty() )
            {
                maxValue = default( TValue );
                return -1;
            }

            int maxIndex = 0;
            maxValue = selector( elements[ 0 ] );

            for( int i = 1; i < elements.Count; ++i )
            {
                TElement currentElement = elements[ i ];
                var currentValue = selector( currentElement );
                if( currentValue.CompareTo( maxValue ) > 0 )
                {
                    maxIndex = i;
                    maxValue = currentValue;
                }
            }
            
            return maxIndex;
        }

        public static int MaxIndex< TElement >( this IList< TElement > elements,
            Func< TElement, float > selector, out float maxValue )
        {
            int maxIndex = -1;
            maxValue = float.NegativeInfinity;

            for( int i = 0; i < elements.Count; ++i )
            {
                TElement currentElement = elements[ i ];
                float currentValue = selector( currentElement );
                if( currentValue > maxValue )
                {
                    maxIndex = i;
                    maxValue = currentValue;
                }
            }            
            
            return maxIndex;
        }

        public static int MinIndex< TElement >( this IList< TElement > elements,
            Func< TElement, float > selector )
        {
            float f;
            return MinIndex( elements, selector, out f );
        }
        
        public static int MinIndex< TElement, TValue >( this IList< TElement > elements,
            Func< TElement, TValue > selector, out TValue minValue ) where TValue : IComparable< TValue >
        {
            if( elements.IsEmpty() )
            {
                minValue = default( TValue );
                return -1;
            }

            int minIndex = 0;
            minValue = selector( elements[ 0 ] );

            for( int i = 1; i < elements.Count; ++i )
            {
                TElement currentElement = elements[ i ];
                var currentValue = selector( currentElement );
                if( currentValue.CompareTo( minValue ) < 0 )
                {
                    minIndex = i;
                    minValue = currentValue;
                }
            }
            
            return minIndex;
        }

        public static int MinIndex< TElement >( this IList< TElement > elements,
            Func< TElement, float > selector, out float minValue )
        {
            int minIndex = -1;
            minValue = float.PositiveInfinity;

            for( int i = 0; i < elements.Count; ++i )
            {
                TElement currentElement = elements[ i ];
                float currentValue = selector( currentElement );
                if( currentValue < minValue )
                {
                    minIndex = i;
                    minValue = currentValue;
                }
            }            
            
            return minIndex;
        }

        public static int MinValidIndex< TElement >( this IList< TElement > elements,
            Func< TElement, bool > validSelector, Func< TElement, float > valueSelector, out float minValue )
        {
            int minIndex = -1;
            minValue = float.PositiveInfinity;

            for( int i = 0; i < elements.Count; ++i )
            {
                TElement currentElement = elements[ i ];
                if( validSelector( currentElement ) )
                {
                    float currentValue = valueSelector( currentElement );
                    if( currentValue < minValue )
                    {
                        minIndex = i;
                        minValue = currentValue;
                    }
                }
            }            
            
            return minIndex;
        }

        public static bool IsEmpty< T >( this IEnumerable< T > e )
        {
            return( e.Count() == 0 );
        }

        public static TSource ArgMin< TSource, TResult >( this IEnumerable< TSource > source,
            Func< TSource, TResult > selector ) where TResult : IComparable< TResult >
        {
            bool initialized = false;
            TResult min = default( TResult );
            TSource argMin = default( TSource );

            foreach( TSource e in source )
            {
                TResult v = selector( e );
                if( !initialized )
                {
                    min = v;
                    argMin = e;

                    initialized = true;
                }
                else
                {
                    if( v.CompareTo( min ) < 0 )
                    {
                        min = v;
                        argMin = e;
                    }
                }
            }

            return argMin;
        }

        public static TSource ArgMax< TSource >( this IEnumerable< TSource > source,
            Func< TSource, float > selector )
        {
            TSource argMax = source.First();
            float max = selector( argMax );

            foreach( TSource e in source )
            {
                float v = selector( e );
                if( v > max )
                {
                    max = v;
                    argMax = e;
                }
            }

            return argMax;
        }

        public static void ArgMinMax< TSource, TResult >( this IEnumerable< TSource > source,
            Func< TSource, TResult > selector, out TSource argMin, out TSource argMax ) where TResult : IComparable< TResult >
        {
            bool initialized = false;
            TResult min = default( TResult );
            TResult max = default( TResult );
            argMin = default( TSource );
            argMax = default( TSource );

            foreach( TSource e in source )
            {
                TResult v = selector( e );
                if( !initialized )
                {
                    min = v;
                    argMin = e;

                    max = v;
                    argMax = e;
                    initialized = true;
                }
                else
                {
                    if( v.CompareTo( min ) < 0 )
                    {
                        min = v;
                        argMin = e;
                    }
                    if( v.CompareTo( max ) > 0 )
                    {
                        max = v;
                        argMax = e;
                    }
                }
            }
        }

        public static void MinMax< TSource, TResult >( this IEnumerable< TSource > source,
            Func< TSource, TResult > selector, out TResult min, out TResult max ) where TResult : IComparable< TResult >
        {
            bool initialized = false;
            min = default( TResult );
            max = default( TResult );

            foreach( TSource e in source )
            {
                TResult v = selector( e );
                if( !initialized )
                {
                    min = v;
                    max = v;
                    initialized = true;
                }
                else
                {
                    if( v.CompareTo( min ) < 0 )
                    {
                        min = v;
                    }
                    if( v.CompareTo( max ) > 0 )
                    {
                        max = v;
                    }
                }
            }
        }

        // TODO: selector

        public static float Product( this IEnumerable< float > source )
        {
            float p = 1;
            foreach( var f in source )
            {
                p *= f;
            }
            return p;
        }

        public static int Product( this IEnumerable<int> source )
        {
            int p = 1;
            foreach( var i in source )
            {
                p *= i;
            }
            return p;
        }

        public static void Apply< T1, T2 >( this IEnumerable< T1 > source, Func< T1, T2 > func )
        {
            foreach( T1 e in source )
            {
                func( e );
            }
        }

        public static void Apply< T >( this IEnumerable< T > source, Action< T > func )
        {
            foreach( T e in source )
            {
                func( e );
            }
        }

        /// <summary>
        /// Same as Slice( source, loInclusive, source.Length )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="loInclusive"></param>
        /// <returns></returns>
        public static T[] Slice< T >( this T[] source, int loInclusive )
        {
            return Slice( source, loInclusive, source.Length );
        }

        // TODO: support negative numbers (figure out how Python works)
        /// <summary>
        /// Extends C# arrays to allow Python-style slicing
        /// </summary>
        /// <typeparam name="T">array element type</typeparam>
        /// <param name="source">source array</param>
        /// <param name="loInclusive">index of the first element (inclusive)</param>
        /// <param name="hiExclusive">index of the last element (exclusive)</param>
        /// <returns></returns>
        public static T[] Slice< T >( this T[] source, int loInclusive, int hiExclusive )
        {
            if( loInclusive < 0 || loInclusive >= source.Length ||
                hiExclusive <= loInclusive || hiExclusive > source.Length )
            {
                throw new ArgumentException( "Invalid array slice parameters" );
            }

            int count = hiExclusive - loInclusive;
            T[] result = new T[ count ];
            Array.Copy( source, loInclusive, result, 0, count );
            return result;
        }

        /// <summary>
        /// Returns an array of pairs of indices indexed circularly.  For example:
        /// Input: i0 = 0, count = 4
        /// Output: [ <0,1>, <1,2>, <2,3>, <3,0> ]
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Vector2i[] CircularIndexPairs( int i0, int count )
        {
            if( count < 2 )
            {
                throw new ArgumentException( "count must be at least 2" );
            }

            var output = new Vector2i[ count ];

            for( int i = 0; i < count - 1; ++i )
            {
                int j = i0 + i;
                output[ i ] = new Vector2i( j, j + 1 );
            }
            output[ count - 1 ] = new Vector2i( i0 + count - 1, i0 );

            return output;
        }

        /// <summary>
        /// Returns an array of triplets of indices indexed circularly.  For example:
        /// Input: i0 = 0, count = 4
        /// Output: [ <0,1,2>, <1,2,3>, <2,3,0> ]
        /// </summary>
        /// <param name="i0"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Vector3i[] CircularIndexTriplets( int i0, int count )
        {
            if( count < 3 )
            {
                throw new ArgumentException( "count must be at least 3" );
            }

            var output = new Vector3i[ count - 1 ];

            for( int i = 0; i < count - 2; ++i )
            {
                int j = i0 + i;
                int k = i0 + i + 1;
                int l = i0 + i + 2;

                output[ i ] = new Vector3i( j, k, l );
            }
            output[ count - 2 ] = new Vector3i( i0 + count - 2, i0 + count - 1, i0 );

            return output;
        }

        /// <summary>
        /// If x < y, then nothing happens.  Otherwise, swaps them.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Sort2( ref float x, ref float y )
        {
            if( y < x )
            {
                Swap( ref x, ref y );
            }
        }

        // TODO: sort3, sort3-into-key-value-pairs: (0,z), (1,y), ...

        public static float Min( params float[] vals )
        {
            return vals.Min();
        }

        public static float Max( params float[] vals )
        {
            return vals.Max();
        }
    }
}

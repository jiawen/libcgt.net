using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Parallel
{
    // Stolen from:
    // http://coding-time.blogspot.com/2008/03/implement-your-own-parallelfor-in-c.html
    public static class ParallelUtils
    {
        public delegate void ForDelegate( int i );
        public delegate void ThreadDelegate();

        /// <summary>
        /// Parallel for loop. Invokes given action, passing arguments
        /// fromInclusive - toExclusive on multiple threads.
        /// Returns when loop finished.
        /// </summary>
        public static void For( int fromInclusive, int toExclusive, ForDelegate action )
        {
            // ChunkSize = 1 makes items to be processed in order.
            // Bigger chunk size should reduce lock waiting time and thus
            // increase paralelism.
            int chunkSize = 4;

            // number of process() threads
            int threadCount = Environment.ProcessorCount;
            int cnt = fromInclusive - chunkSize;

            // processing function
            // takes next chunk and processes it using action
            ThreadDelegate process = delegate()
            {
                while( true )
                {
                    int cntMem = 0;
                    lock( typeof( ParallelUtils ) )
                    {
                        // take next chunk
                        cnt += chunkSize;
                        cntMem = cnt;
                    }
                    // process chunk
                    // here items can come out of order if chunkSize > 1
                    for( int i = cntMem; i < cntMem + chunkSize; ++i )
                    {
                        if( i >= toExclusive ) return;
                        action( i );
                    }
                }
            };

            // launch process() threads
            IAsyncResult[] asyncResults = new IAsyncResult[ threadCount ];
            for( int i = 0; i < threadCount; ++i )
            {
                asyncResults[ i ] = process.BeginInvoke( null, null );
            }
            // wait for all threads to complete
            for( int i = 0; i < threadCount; ++i )
            {
                process.EndInvoke( asyncResults[ i ] );
            }
        }
    }
}

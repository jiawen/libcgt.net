using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace libcgt.core
{
    public class PrefetchingLRUCache< TTag, TData > : LRUCache< TTag, TData >
    {
        private readonly Func< TTag, IEnumerable< TTag > > AdjacentTagsCallback;
        private object entryLock;
        
        private object prefetchQueueLock;
        private IEnumerable< TTag > prefetchQueue;
        private Thread prefetchingThread;

        public PrefetchingLRUCache( int capacity,
            Func< TTag, IEnumerable< TTag > > adjacentTagsCallback )
            : base( capacity )
        {
            AdjacentTagsCallback = adjacentTagsCallback;

            entryLock = new object();
            
            prefetchingThread = new Thread( new ThreadStart( PrefetchMain ) );
        }

        public override TData Read( TTag tag )
        {
            //lock( readLock )
            {
                var data = base.Read( tag );

                Prefetch( tag );

                return data;
            }
        }

        private void PrefetchMain()
        {
            while( true )
            {
                try
                {
                   Monitor.Wait( this );
                }
                catch (SynchronizationLockException e)
                {
                   Console.WriteLine(e);
                }
                catch (ThreadInterruptedException e)
                {
                   Console.WriteLine(e);
                }
            }
        }

        private void Prefetch( TTag tag )
        {
            var adjacentTags = AdjacentTagsCallback( tag );
            foreach( var adjacentTag in adjacentTags )
            {
                base.Read( adjacentTag );
            }
        }
    }
}

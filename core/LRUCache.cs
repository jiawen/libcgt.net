using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    // TODO: unify this with AbstractCache
    public class LRUCache< TTag, TData >
    {
        public delegate void CacheEvent( TTag tag, ref TData data );
        public event CacheEvent ReadMissEvent;
        public event CacheEvent EvictEvent;

        public int Capacity { get; private set; }

        protected class LRUCacheEntry
        {
            public bool dirty;
            public LinkedListNode< TTag > lru;
            public TData data;
        }

        // recently used list
        // the front of the list is the most recently used
        protected LinkedList< TTag > lruList;
        protected Dictionary< TTag, LRUCacheEntry > entries;

        public LRUCache( int capacity )
        {
            if( capacity < 1 )
            {
                throw new ArgumentException( "capacity must be a positive integer" );
            }

            Capacity = capacity;
            entries = new Dictionary< TTag, LRUCacheEntry >( capacity );
            lruList = new LinkedList< TTag >();
        }

        public TData this[ TTag tag ]
        {
            get
            {
                return Read( tag );
            }
        }

        /// <summary>
        /// Performs a cache read
        /// On a hit: tag is in entries and not dirty, returns the entry
        /// Othewise, miss: fetch with OnReadMiss()
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public virtual TData Read( TTag tag )
        {
            if( IsCached( tag ) ) // hit: entry is in the cache
            {
                var entry = entries[ tag ];

                // cache miss: entry is dirty, fetch using OnReadMiss()                
                if( entry.dirty )
                {
                    OnReadMiss( tag, ref entry.data );
                    entry.dirty = false;
                }

                // else, it's a hit
                // mark entry as most recently used
                lruList.Remove( entry.lru );
                lruList.AddFirst( entry.lru );
            }            
            else // miss: entry is not in the cache
            {
                TData evictedData = default( TData );
                // evict the least recently used if full
                if( IsFull() )
                {
                    evictedData = EvictLeastRecentlyUsed();
                }

                // create a new entry and mark it most recently used
                var entry = new LRUCacheEntry
                {
                    dirty = true,
                    lru = new LinkedListNode< TTag >( tag ),
                    data = evictedData
                };
                entries[ tag ] = entry;

                OnReadMiss( tag, ref entry.data );
                entry.dirty = false;

                lruList.AddFirst( entry.lru );
            }
            // return the data
            return entries[ tag ].data;
        }

        public bool IsCached( TTag tag )
        {
            return entries.ContainsKey( tag );
        }

        public bool IsDirty( TTag tag )
        {
            if( !( entries.ContainsKey( tag ) ) )
            {
                throw new ArgumentException( "cache does not contain tag: " + tag );
            }
            return entries[ tag ].dirty;
        }

        public void MarkDirty( TTag tag )
        {
            if( entries.ContainsKey( tag ) )
            {
                entries[ tag ].dirty = true;
            }            
        }

        // TODO: IsFull isn't quite right
        // should count the number of entries that are not dirty?
        private bool IsFull()
        {
            return( entries.Count == Capacity );
        }

        private TData EvictLeastRecentlyUsed()
        {
            var lru = lruList.Last;
            var entry = entries[ lru.Value ];
            
            OnEvict( entry.lru.Value, ref entry.data );

            lruList.RemoveLast();
            entries.Remove( lru.Value );
            return entry.data;
        }

        protected void OnReadMiss( TTag tag, ref TData data )
        {
            if( ReadMissEvent != null )
            {
                ReadMissEvent( tag, ref data );
            }
            else
            {
                throw new DataException( "LRU cache miss without handler." );
            }
        }
        
        protected void OnEvict( TTag tag, ref TData data )
        {
            if( EvictEvent != null )
            {
                EvictEvent( tag, ref data );
            }
        }
    }
}

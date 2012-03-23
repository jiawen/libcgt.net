using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    // TODO: LRU, cost, infinite cost to keep stuff forever
    // TODO: simple integer cost cache
    // TODO: weak references

    // TODO: AbstractCache with no key
    public abstract class AbstractCache< TKey, TData >
    {
        // a dictionary mapping the cache lookup type to pair
        // consisting of a dirty bit and the data itself
        protected Dictionary< TKey, Pair< bool, TData > > entries;

        protected AbstractCache()
        {
            entries = new Dictionary< TKey, Pair< bool, TData > >();
        }

        protected abstract void UpdateEntry( TKey key, ref TData entry );

        /// <summary>
        /// Creates a new entry in the cache, initialized to dirty.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public virtual void InsertEntry( TKey key, TData data )
        {
            entries[ key ] = Pair.Create( true, data );
        }

        public TData this[ TKey key ]
        {
            get
            {
                return GetEntry( key );
            }
        }

        public TData GetEntry( TKey key )
        {
            if( IsDirty( key ) )
            {
                var entry = entries[ key ].Second;
                UpdateEntry( key, ref entry );
                entries[ key ].Second = entry;
                MarkClean( key );
            }
            return entries[ key ].Second;
        }

        public void RemoveEntry( TKey key )
        {
            entries.Remove( key );
        }

        public void MarkClean( TKey key )
        {
            MarkEntry( key, false );
        }

        public void MarkDirty( TKey key )
        {
            MarkEntry( key, true );
        }

        public void MarkAllEntriesClean()
        {
            MarkAllEntries( false );
        }

        public void MarkAllEntriesDirty()
        {
            MarkAllEntries( true );
        }

        public void MarkAllEntries( bool isDirty )
        {
            foreach( var key in entries.Keys )
            {
                entries[ key ].First = isDirty;
            }
        }

        public void MarkEntry( TKey key, bool isDirty )
        {
            entries[ key ].First = isDirty;
        }

        public bool IsClean( TKey key )
        {
            return !IsDirty( key );
        }

        public bool IsDirty( TKey key )
        {
            return entries[ key ].First;
        }
    }
}

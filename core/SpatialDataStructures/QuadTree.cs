using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core.SpatialDataStructures
{
    public class QuadTreeNode< T >
    {
        public bool hasData;
        public Vector2f dataPosition;
        public T data;

        // rectangle for self
        public Rect2f bounds;

        // children
        public QuadTreeNode< T > southWest;
        public QuadTreeNode< T > southEast;
        public QuadTreeNode< T > northWest;
        public QuadTreeNode< T > northEast;
        public QuadTreeNode< T >[] children;

        public QuadTreeNode( Rect2f bounds )
        {
            this.bounds = bounds;
        }

        public void Add( Vector2f incomingPosition, T incomingData )
        {
            // no data
            if( !hasData )
            {
                // and no children
                // then add to self
                if( children == null )
                {
                    hasData = true;
                    dataPosition = incomingPosition;
                    data = incomingData;
                }
                // no data, but has children
                // (which means one of the children has data in its subtree)
                // add it to one of this children
                else
                {
                    AddDataToChildren( incomingPosition, incomingData );
                }
            }
            // has data
            else
            {
                // but no children
                if( children == null )
                {
                    
                    AllocateChildren();

                    // 1. add incoming data to children
                    AddDataToChildren( incomingPosition, incomingData );
                    
                    // 2. move this node's data to children
                    AddDataToChildren( dataPosition, data );
                    hasData = false;
                    data = default( T );
                }
                else
                {
                    throw new DataException( "Inconsistent quad tree detected: cannot have a node with both data and children" );
                }
            }
        }

        private void AllocateChildren()
        {
            float x0 = bounds.Origin.x;
            float y0 = bounds.Origin.y;
            float w = bounds.Width;
            float h = bounds.Height;
            float hw = 0.5f * w;
            float hh = 0.5f * h;

            // 0. allocate 4 children
            southWest = new QuadTreeNode< T >( new Rect2f( x0, y0, hw, hh ) );
            southEast = new QuadTreeNode< T >( new Rect2f( x0 + hw, y0, hw, hh ) );
            northWest = new QuadTreeNode< T >( new Rect2f( x0, y0 + hh, hw, hh ) );
            northEast = new QuadTreeNode< T >( new Rect2f( x0 + hw, y0 + hh, hw, hh ) );
            children = new[] { southWest, southEast, northWest, northEast };
        }

        private void AddDataToChildren( Vector2f position, T data )
        {
            foreach( var child in children )
            {
                if( child.bounds.Contains( position ) )
                {
                    child.Add( position, data );
                }
            }
        }
    }

    public class QuadTree< T >
    {
        // TODO: self = average of children or what?

        // TODO: public int cellsPerNode
        public QuadTreeNode< T > root;        

        public QuadTree( Rect2f boundingBox )
        {
            root = new QuadTreeNode< T >( boundingBox );
        }

        public void Add( Vector2f position, T data )
        {
            if( !( root.bounds.Contains( position ) ) )
            {
                throw new DataException( "data is outside bounding box of quadtree" );
            }
            root.Add( position, data );
        }

        // TODO: public FindNeighbor( QuadTreeNode< T > node )
        // TODO: add field: parent
        // TODO: add field: tree
    }
}

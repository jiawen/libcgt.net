using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.Visualizer
{
    public class ItemTree
    {
        public bool Active { get; set; }
        public string Name { get; set; }

        public Dictionary< string, ItemTree > Subtrees { get; set; }

        private List< IVisualizerItem > items;

        // TODO: add a transform...

        public ItemTree( string name )
        {
            Active = true;
            Name = name;
            Subtrees = new Dictionary< string, ItemTree >();

            items = new List< IVisualizerItem >();
        }

        public void Add( IVisualizerItem item )
        {
            items.Add( item );
        }

        public void Add( IEnumerable< IVisualizerItem > items )
        {
            this.items.AddRange( items );
        }

        public void Clear()
        {
            items.Clear();
        }        

        public void RecursiveClear()
        {
            items.Clear();
            foreach( var tree in Subtrees )
            {
                tree.Value.RecursiveClear();
            }
        }

        public void CreateSubtree( string name, bool active )
        {
            var tree = new ItemTree( name );
            tree.Active = active;
            Subtrees[ name ] = tree;
        }

        public ItemTree GetSubtreeByName( string name )
        {
            return Subtrees[ name ];
        }
        
        public IList< IVisualizerItem > ItemsInTreeNode()
        {
            return items.AsReadOnly();
        }

        public IList< IVisualizerItem > ItemsInSubTree()
        {
            var output = new List< IVisualizerItem >();
            
            if( Active )
            {
                output.AddRange( items );
            }

            foreach( var tree in Subtrees )
            {
                output.AddRange( tree.Value.ItemsInSubTree() );
            }
            return output;
        }
    }
}

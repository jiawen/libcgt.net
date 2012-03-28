using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;
using SlimDX;

namespace libcgt.Visualizer
{
    [Serializable]
    public class PointItem : IVisualizerItem
    {
        public Vector4f Position { get; set; }
        public Vector4f Color { get; set; }

        public PointItem( Vector2f p0, Vector4f color )
            : this( new Vector4f( p0, 0, 1 ), color )
        {
            
        }

        public PointItem( Vector4f p0, Vector4f color )
        {
            Position = p0;
            Color = color;
        }

        public void Write( DataStream stream )
        {
            stream.Write( Position );
            stream.Write( Color );
        }

        public int NumVertices
        {
            get
            {
                return 1;
            }
        }

        public ItemType ItemType
        {
            get
            {
                return ItemType.Point;
            }
        }

        public BlendType BlendType
        {
            get
            {
                return BlendType.Opaque;
            }
        }
    }
}

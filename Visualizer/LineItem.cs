using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;
using SlimDX;

namespace libcgt.Visualizer
{
    [Serializable]
    public class LineItem : IVisualizerItem
    {
        public Vector4f P0 { get; set; }
        public Vector4f C0 { get; set; }
        public Vector4f P1 { get; set; }
        public Vector4f C1 { get; set; }

        public BlendType BlendType { get; set; }

        public LineItem( Vector2f p0, Vector2f p1, Vector4f color )
            : this( new Vector4f( p0, 0, 1 ), new Vector4f( p1, 0, 1 ), color )
        {
            
        }

        public LineItem( Vector2f p0, Vector4f c0,
            Vector2f p1, Vector4f c1 )
            : this( new Vector4f( p0, 0, 1 ), c0, new Vector4f( p1, 0, 1 ), c1 )
        {
            
        }

        public LineItem( Vector4f p0, Vector4f p1, Vector4f color )
            : this( p0, color, p1, color )
        {
            
        }

        public LineItem( Vector4f p0, Vector4f c0,
            Vector4f p1, Vector4f c1 )
        {
            P0 = p0;
            C0 = c0;
            P1 = p1;
            C1 = c1;

            BlendType = BlendType.Opaque;
        }

        public void Write( DataStream stream )
        {
            stream.Write( P0 );
            stream.Write( C0 );

            stream.Write( P1 );
            stream.Write( C1 );
        }

        public int NumVertices
        {
            get
            {
                return 2;
            }
        }

        public ItemType ItemType
        {
            get
            {
                return ItemType.Line;
            }
        }

        public static LineItem[] MakeRectangle( Rect2f rect, Vector4f color )
        {
            return new[]
            {
                new LineItem( rect.Origin, rect.BottomRight, color ), 
                new LineItem( rect.BottomRight, rect.TopRight, color ), 
                new LineItem( rect.TopRight, rect.TopLeft, color ), 
                new LineItem( rect.TopLeft, rect.Origin, color )
            };
        }
    }
}

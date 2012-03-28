using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;
using SlimDX;

namespace libcgt.Visualizer
{
    [Serializable]
    public class TriangleItem : IVisualizerItem
    {
        public Vector4f P0 { get; set; }
        public Vector4f P1 { get; set; }
        public Vector4f P2 { get; set; }
        public Vector4f C0 { get; set; }
        public Vector4f C1 { get; set; }
        public Vector4f C2 { get; set; }
        public BlendType BlendType { get; set; }

        public TriangleItem( Vector2f p0, Vector2f p1, Vector2f p2, Vector4f color, BlendType blendType )
            : this( new Vector4f( p0, 0, 1 ), new Vector4f( p1, 0, 1 ), new Vector4f( p2, 0, 1 ),
            color, color, color, blendType )
        {
            
        }

        public TriangleItem
            (
                Vector2f p0, Vector2f p1, Vector2f p2,
                Vector4f c0, Vector4f c1, Vector4f c2,
                BlendType blendType
            )
            : this
            (
                new Vector4f( p0, 0, 1 ), new Vector4f( p1, 0, 1 ), new Vector4f( p2, 0, 1 ),
                c0, c1, c2,
                blendType
            )
        {
            
        }

        public TriangleItem( Vector4f p0, Vector4f p1, Vector4f p2,
            Vector4f c0, Vector4f c1, Vector4f c2, BlendType blendType )
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;

            C0 = c0;
            C1 = c1;
            C2 = c2;

            BlendType = blendType;
        }

        public void Write( DataStream stream )
        {
            stream.Write( P0 );
            stream.Write( C0 );

            stream.Write( P1 );
            stream.Write( C1 );

            stream.Write( P2 );
            stream.Write( C2 );
        }

        public int NumVertices
        {
            get
            {
                return 3;
            }
        }

        public ItemType ItemType
        {
            get
            {
                return ItemType.Triangle;
            }
        }

        public static TriangleItem[] MakeSquare( Vector2f center, float radius, Vector4f color, BlendType blendType )
        {
            return new[]
            {
                new TriangleItem
                (
                    new Vector2f( center.X - radius, center.Y - radius ),
                    new Vector2f( center.X + radius, center.Y - radius ),
                    new Vector2f( center.X - radius, center.Y + radius ),
                    color,
                    blendType
                ),

                new TriangleItem
                (
                    new Vector2f( center.X - radius, center.Y + radius ),
                    new Vector2f( center.X + radius, center.Y - radius ),
                    new Vector2f( center.X + radius, center.Y + radius ),
                    color,
                    blendType
                )
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace libcgt.Visualizer
{
    public interface IVisualizerItem
    {
        void Write( DataStream stream );

        int NumVertices { get; }
        ItemType ItemType { get; }
        BlendType BlendType { get; }
    }
}

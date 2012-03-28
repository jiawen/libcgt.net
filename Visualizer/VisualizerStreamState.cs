using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.DirectX.D3D10;
using libcgt.DirectX.D3D10.VertexFormats;
using SlimDX;

namespace libcgt.Visualizer
{
    public class VisualizerStreamState
    {
        private const int DefaultNumVertexBuffers = 3;
        private const int DefaultBufferSizeVertices = 16384;

        // Permanent data
        private DynamicVertexBuffer[] vertexBuffers;
        private int bufferSizeVertices;
        
        // Transient data
        private int currentVertexBufferIndex;
        private DataStream currentStream;
        public int NumVerticesInCurrentStream { get; private set; }

        public VisualizerStreamState()
            : this( DefaultNumVertexBuffers, DefaultBufferSizeVertices )
        {
            
        }

        public VisualizerStreamState( int nBuffers, int bufferSizeVertices )
        {
            vertexBuffers = new DynamicVertexBuffer[ nBuffers ];
            for( int i = 0; i < nBuffers; ++i )
            {
                vertexBuffers[ i ] = new DynamicVertexBuffer( VertexPosition4fColor4f.SizeInBytes, bufferSizeVertices );
            }

            this.bufferSizeVertices = bufferSizeVertices;
        }
        
        public void Begin()
        {
            currentVertexBufferIndex = 0;
            currentStream = CurrentVertexBuffer.MapForWriteDiscard();
            NumVerticesInCurrentStream = 0;
        }

        public void End()
        {
            CurrentVertexBuffer.Unmap();

            currentVertexBufferIndex = -1;
            currentStream = null;
            NumVerticesInCurrentStream = -1;
        }

        public void BeginRender()
        {
            CurrentVertexBuffer.Unmap();
        }

        public void EndRender()
        {
            // switch buffers
            currentVertexBufferIndex = ( currentVertexBufferIndex + 1 ) % vertexBuffers.Length;            
            currentStream = CurrentVertexBuffer.MapForWriteDiscard();
            NumVerticesInCurrentStream = 0;
        }

        public DynamicVertexBuffer CurrentVertexBuffer
        {
            get
            {
                return vertexBuffers[ currentVertexBufferIndex ];
            }
        }

        public bool CanAppendItem( IVisualizerItem item )
        {
            return ( NumVerticesInCurrentStream + item.NumVertices <= bufferSizeVertices );
        }

        public void AppendItem( IVisualizerItem item )
        {
            item.Write( currentStream );
            NumVerticesInCurrentStream += item.NumVertices;
        }
    }
}

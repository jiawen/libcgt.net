using System;
using libcgt.core;
using SlimDX;
using SlimDX.Direct3D10;
using Buffer=SlimDX.Direct3D10.Buffer;

namespace libcgt.DirectX.D3D10
{
    // TODO: make these VertexBuffer, or even D3DBuffer?    
    public class DynamicVertexBuffer
    {
        // current number of useful vertices
        public int Count { get; set; }

        // maximum capacity of the buffer (in vertices)
        public int Capacity { get; private set; }

        // size of each vertex
        public int VertexSizeBytes { get; private set; }

        // the underlying buffer itself
        public Buffer VertexBuffer { get; private set; }

        // the default binding starting from vertex 0
        public VertexBufferBinding DefaultBinding { get; private set; }

        private DataStream mappedStream;

        public DynamicVertexBuffer( int vertexSizeBytes, int capacity )
        {
            // TODO: check on capacity
            VertexSizeBytes = vertexSizeBytes;
            Resize( capacity );
        }

        /// <summary>
        /// Resizes the buffer to the nearest power of two >= nVertices
        /// and sets Count to nVertices
        /// </summary>
        /// <param name="nVertices"></param>
        public void Resize( int nVertices )
        {
            if( nVertices > Capacity || // if we need more than we currently have allocated
                nVertices < Capacity / 2 ) // or if the current one is more than twice as big
            {
                Capacity = nVertices.RoundUpToNearestPowerOfTwo();

                if( VertexBuffer != null )
                {
                    VertexBuffer.Dispose();
                }
                VertexBuffer = BufferFactory.CreateDynamicVertexBuffer( Capacity, VertexSizeBytes );
                DefaultBinding = new VertexBufferBinding( VertexBuffer, VertexSizeBytes, 0 );
            }
            Count = nVertices;
        }
        
        public DataStream MapForWriteDiscard()
        {
            if( mappedStream != null )
            {
                throw new InvalidOperationException( "This vertex buffer is already mapped" );
            }            
            mappedStream = VertexBuffer.Map( MapMode.WriteDiscard, MapFlags.None );
            return mappedStream;
        }

        public void Unmap()
        {
            if( mappedStream == null )
            {
                throw new InvalidOperationException( "This vertex buffer is not mapped" );
            }

            // TODO: why do we need to keep the mapped stream around, just to close it?
            mappedStream.Close();
            VertexBuffer.Unmap();
            mappedStream = null;
        }
    }
}

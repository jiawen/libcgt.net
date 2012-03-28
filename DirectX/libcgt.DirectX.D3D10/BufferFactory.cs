using System;
using libcgt.DirectX.D3D10;
using SlimDX.Direct3D10;
using Buffer=SlimDX.Direct3D10.Buffer;

namespace libcgt.DirectX.D3D10
{
    /// <summary>
    /// Convenience methods for creating various types of Buffers
    /// </summary>
    public static class BufferFactory
    {
        public static Buffer CreateDynamicVertexBuffer( int nVertices, int vertexSizeBytes )
        {
            var bd = new BufferDescription
            (
                nVertices * vertexSizeBytes,
                ResourceUsage.Dynamic,
                BindFlags.VertexBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None
            );
            return new Buffer( D3D10Wrapper.Instance.Device, bd );
        }

        public static Buffer CreateStaticVertexBuffer( int nVertices, int vertexSizeBytes )
        {
            var bd = new BufferDescription
            (
                nVertices * vertexSizeBytes,
                ResourceUsage.Default,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None
            );
            return new Buffer( D3D10Wrapper.Instance.Device, bd );
        }

        public static Buffer CreateStagingBuffer( int nVertices, int vertexSizeBytes )
        {
            return CreateStagingBuffer( nVertices * vertexSizeBytes );
        }

        public static Buffer CreateStagingBuffer( int nBytes )
        {
            var bd = new BufferDescription
            (
                nBytes,
                ResourceUsage.Staging,
                BindFlags.None,
                CpuAccessFlags.Read | CpuAccessFlags.Write,
                ResourceOptionFlags.None
            );
            return new Buffer( D3D10Wrapper.Instance.Device, bd );
        }

        public static Buffer CreateDynamicDataBuffer( int nElements, int elementSizeBytes )
        {
            throw new NotImplementedException();
            var bd = new BufferDescription
            (
                nElements * elementSizeBytes,
                //ResourceUsage.Dynamic,
                ResourceUsage.Default,
                BindFlags.StreamOutput | BindFlags.ShaderResource,
                //CpuAccessFlags.None,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None
            );
            return new Buffer( D3D10Wrapper.Instance.Device, bd );
        }
    }
}

using libcgt.core;
using SlimDX;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10
{
    // TODO: allow a couple of formats?
    // int, int2, float 2, 3, 4?
    public class DynamicDataBuffer
    {
        // current number of useful vertices
        public int Count { get; set; }

        // maximum capacity of the buffer (in vertices)
        public int Capacity { get; private set; }

        // size of each vertex
        public int ElementSizeBytes { get; private set; }

        // the underlying buffer itself
        public Buffer Buffer { get; private set; }
        
        public ShaderResourceView View { get; private set; }

        public DynamicDataBuffer( int elementSizeBytes, int capacity )
        {
            // TODO: check on capacity
            ElementSizeBytes = elementSizeBytes;            
            Resize( capacity );
        }

        /// <summary>
        /// Resizes the buffer to the nearest power of two >= nElements
        /// and sets Count to nElements
        /// </summary>
        /// <param name="nElements"></param>
        public void Resize( int nElements )
        {
            if( nElements > Capacity || // if we need more than we currently have allocated
                nElements < Capacity / 2 ) // or if the current one is more than twice as big
            {
                Capacity = nElements.RoundUpToNearestPowerOfTwo();

                if( Buffer != null )
                {
                    View.Dispose();
                    Buffer.Dispose();
                }
                Buffer = BufferFactory.CreateDynamicDataBuffer( Capacity, ElementSizeBytes );

                var srv = new ShaderResourceViewDescription()
                {
                    ArraySize = 1,
                    Format = SlimDX.DXGI.Format.R32G32B32A32_Float,
                    Dimension = ShaderResourceViewDimension.Buffer,
                    ElementOffset = 0,
                    ElementWidth = ElementSizeBytes
                };

                View = new ShaderResourceView( D3D10Wrapper.Instance.Device, Buffer, srv );
            }
            Count = nElements;
        }

        // TODO: make this just like DynamicVertexBuffer, subclass?
        public DataStream MapForWriteDiscard()
        {
            return Buffer.Map( MapMode.WriteDiscard, MapFlags.None );
        }

        public void Unmap()
        {
            Buffer.Unmap();
        }
    }
}

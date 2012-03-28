using System.Runtime.InteropServices;
using libcgt.core.Vecmath;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition4f
    {
        public Vector4f position;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 )
        };

        public static int SizeInBytes
        {
            get
            {
                return 4 * sizeof( float );
            }
        }

        public VertexPosition4f( float x, float y, float z, float w )
        {
            position = new Vector4f( x, y, z, w );
        }

        public VertexPosition4f( Vector4f position )
        {
            this.position = position;
        }
    }
}

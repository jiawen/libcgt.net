using System.Runtime.InteropServices;
using libcgt.core.Vecmath;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition4fNormal3f
    {
        public Vector4f position;
        public Vector3f normal;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
            new InputElement( "NORMAL", 0, Format.R32G32B32_Float, 4 * sizeof( float ), 0 )
        };

        public static int SizeInBytes
        {
            get
            {
                return 7 * sizeof( float );
            }
        }

        public VertexPosition4fNormal3f( float x, float y, float z, float w,
            float nx, float ny, float nz )
        {
            position = new Vector4f( x, y, z, w );
            normal = new Vector3f( nx, ny, nz );
        }

        public VertexPosition4fNormal3f( Vector3f position, Vector3f normal )
        {
            this.position = new Vector4f( position, 1.0f );
            this.normal = normal;
        }

        public VertexPosition4fNormal3f( Vector4f position, Vector3f normal )
        {
            this.position = position;
            this.normal = normal;
        }
    }
}

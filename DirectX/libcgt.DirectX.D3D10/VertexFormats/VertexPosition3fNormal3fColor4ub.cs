using System.Runtime.InteropServices;
using libcgt.core.Vecmath;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition3fNormal3fColor4ub
    {
        public Vector3f position;
        public Vector3f normal;
        public Vector4ub color;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32_Float, 0, 0 ),
            new InputElement( "NORMAL", 0, Format.R32G32B32_Float, 3 * sizeof( float ), 0 ),
            new InputElement( "COLOR", 0, Format.R8G8B8A8_UNorm, 6 * sizeof( float ), 0 )
        };

        public static int SizeInBytes
        {
            get
            {
                return 4 + 6 * sizeof( float );
            }
        }

        public VertexPosition3fNormal3fColor4ub( float x, float y, float z,
            float nx, float ny, float nz,
            byte r, byte g, byte b, byte a )
        {
            position = new Vector3f( x, y, z );
            normal = new Vector3f( nx, ny, nz );
            color = new Vector4ub( r, g, b, a );
        }

        public VertexPosition3fNormal3fColor4ub( Vector3f position, Vector3f normal, Vector4ub color )
        {
            this.position = position;
            this.normal = normal;
            this.color = color;
        }
    }
}

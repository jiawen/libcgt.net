using System.Runtime.InteropServices;
using libcgt.core.Vecmath;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition3fColor4ub
    {
        public Vector3f position;
        public Vector4ub color;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32_Float, 0, 0 ),
            new InputElement( "COLOR", 0, Format.R8G8B8A8_UNorm, 3 * sizeof( float ), 0 )
        };

        public static int SizeInBytes
        {
            get
            {
                return 4 + 3 * sizeof( float );
            }
        }

        public VertexPosition3fColor4ub( float x, float y, float z, byte r, byte g, byte b, byte a )
        {
            position = new Vector3f( x, y, z );
            color = new Vector4ub( r, g, b, a );
        }

        public VertexPosition3fColor4ub( Vector3f position, byte r, byte g, byte b, byte a )
        {
            this.position = position;
            color = new Vector4ub( r, g, b, a );
        }

        public VertexPosition3fColor4ub( float x, float y, float z, Vector4ub color )
        {
            position = new Vector3f( x, y, z );
            this.color = color;
        }
        
        public VertexPosition3fColor4ub( Vector3f position, Vector4ub color )
        {
            this.position = position;
            this.color = color;
        }
    }
}

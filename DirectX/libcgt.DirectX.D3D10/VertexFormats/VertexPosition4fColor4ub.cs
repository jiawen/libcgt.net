using System.Runtime.InteropServices;
using libcgt.core.Vecmath;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition4fColor4ub
    {
        public Vector4f position;
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
            new InputElement( "COLOR", 0, Format.R8G8B8A8_UNorm, 4 * sizeof( float ), 0 )
        };

        public static int SizeInBytes
        {
            get
            {
                return 4 + 4 * sizeof( float );
            }
        }

        public VertexPosition4fColor4ub( Vector4f position, byte r, byte g, byte b, byte a )
        {
            this.position = position;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }        
    }
}

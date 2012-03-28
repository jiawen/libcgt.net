using System.Runtime.InteropServices;
using libcgt.core.Vecmath;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition3fTexture2f
    {
        public Vector3f position;
        public Vector2f texcoord;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32_Float, 0, 0 ),
            new InputElement( "TEXCOORD", 0, Format.R32G32_Float, 3 * sizeof( float ), 0 )
        };

        public static int SizeInBytes
        {
            get
            {
                return 5 * sizeof( float );
            }
        }

        public VertexPosition3fTexture2f( float x, float y, float z, float u, float v )
        {
            position = new Vector3f( x, y, z );
            texcoord = new Vector2f( u, v );
        }   

        public VertexPosition3fTexture2f( Vector3f position, Vector2f texcoord )
        {
            this.position = position;
            this.texcoord = texcoord;
        }        
    }
}

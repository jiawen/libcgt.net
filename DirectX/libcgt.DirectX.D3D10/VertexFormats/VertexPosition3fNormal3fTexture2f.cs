using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using libcgt.core.Vecmath;
using SlimDX.Direct3D10;
using SlimDX.DXGI;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition3fNormal3fTexture2f
    {
        public Vector3f position;
        public Vector3f normal;
        public Vector2f textureCoordinates;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32_Float, 0, 0 ),
            new InputElement( "NORMAL", 0, Format.R32G32B32_Float, 3 * sizeof( float ), 0 ),
            new InputElement( "TEXCOORD", 0, Format.R32G32B32_Float, 6 * sizeof( float ), 0 ),
        };

        public static int SizeInBytes
        {
            get
            {
                return 8 * sizeof( float );
            }
        }

        public VertexPosition3fNormal3fTexture2f( float x, float y, float z,
            float nx, float ny, float nz,
            float u, float v )            
        {
            position = new Vector3f( x, y, z );
            normal = new Vector3f( nx, ny, nz );
            textureCoordinates = new Vector2f( u, v );
        }

        public VertexPosition3fNormal3fTexture2f( Vector3f position, Vector3f normal, Vector2f uv )
        {
            this.position = position;
            this.normal = normal;
            this.textureCoordinates = uv;
        }
    }
}

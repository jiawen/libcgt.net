﻿using System.Runtime.InteropServices;
using libcgt.core.Vecmath;
using SlimDX.DXGI;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.VertexFormats
{
    [StructLayout( LayoutKind.Sequential )]
    public struct VertexPosition4fTexture4f
    {
        public Vector4f position;
        public Vector4f texture;

        public static readonly InputElement[] InputElements = new[]
        {
            new InputElement( "POSITION", 0, Format.R32G32B32A32_Float, 0, 0 ),
            new InputElement( "TEXCOORD", 0, Format.R32G32B32A32_Float, 4 * sizeof( float ), 0 )
        };

        public static int SizeInBytes
        {
            get
            {
                return 8 * sizeof( float );
            }
        }

        public VertexPosition4fTexture4f( Vector4f position, Vector4f texture )
        {
            this.position = position;
            this.texture = texture;
        }
    }
}

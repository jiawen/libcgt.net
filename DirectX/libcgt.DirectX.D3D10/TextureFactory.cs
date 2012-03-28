using libcgt.core.Vecmath;
using SlimDX.Direct3D10;
using SlimDX.DXGI;

namespace libcgt.DirectX.D3D10
{
    public static class TextureFactory
    {
        public static Texture2D CreateDepth( Vector2i size,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateDepth( size.x, size.y, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateDepth( int width, int height,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, 1, 1, Format.D24_UNorm_S8_UInt, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateFloat1( Vector2i size,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateFloat1( size.x, size.y, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateFloat1( int width, int height,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, 1, 1, Format.R32_Float, usage, bindFlags, cpuAccessFlags );            
        }

        public static Texture2D CreateFloat2( Vector2i size,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateFloat1( size.x, size.y, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateFloat2( int width, int height,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, 1, 1, Format.R32G32_Float, usage, bindFlags, cpuAccessFlags );            
        }

        public static Texture2D CreateFloat2Array( int width, int height, int arraySize,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, arraySize, 1, Format.R32G32_Float, usage, bindFlags, cpuAccessFlags );            
        }

        public static Texture2D CreateFloat4( Vector2i size,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateFloat4( size.x, size.y, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateFloat4( int width, int height,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, 1, 1, Format.R32G32B32A32_Float, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateUnsignedByte4( Vector2i size,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateUnsignedByte4( size.x, size.y, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateUnsignedByte4( int width, int height,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, 1, 1, Format.R8G8B8A8_UNorm, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateUnsignedByte4Array( int width, int height, int arraySize,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, arraySize, 1, Format.R8G8B8A8_UNorm, usage, bindFlags, cpuAccessFlags );
        }

        public static Texture2D CreateUnsignedByte4MipMapped( int width, int height, int mipLevels,
            ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            return CreateTexture( width, height, 1, mipLevels, Format.R8G8B8A8_UNorm, usage, bindFlags, cpuAccessFlags );
        }

        // HACK: figure out a good way...maybe the D3D way really is the right way
        private static Texture2D CreateTexture( int width, int height, int arraySize, int mipLevels,
            Format format, ResourceUsage usage, BindFlags bindFlags, CpuAccessFlags cpuAccessFlags )
        {
            var wrapper = D3D10Wrapper.Instance;
            var desc = new Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = mipLevels,
                ArraySize = arraySize,
                Format = format,
                SampleDescription = new SampleDescription( 1, 0 ),
                Usage = usage,
                BindFlags = bindFlags,
                CpuAccessFlags = cpuAccessFlags,
                OptionFlags = ResourceOptionFlags.None
            };

            if( mipLevels != 1 )
            {
                desc.OptionFlags = ResourceOptionFlags.GenerateMipMaps;
            }

            return new Texture2D( wrapper.Device, desc );
        }
    }
}

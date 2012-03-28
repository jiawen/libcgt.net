using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;
using SlimDX;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using MapFlags=SlimDX.Direct3D10.MapFlags;

namespace libcgt.DirectX.D3D10
{
    public class DynamicTexture1D
    {
        public int Length { get; private set; }
        public Texture1D Texture { get; private set; }
        public ShaderResourceView View { get; private set; }

        // TODO: float4
        public DynamicTexture1D( int length )
        {            
            Resize( length );
        }

        public void Resize( int length )
        {
            if( Length == length || length == 0 )
            {
                return;
            }

            Length = length;

            if( Texture != null )
            {
                View.Dispose();
                Texture.Dispose();
            }

            var desc = new Texture1DDescription
            {
                Width = length,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R32G32B32A32_Float,                
                Usage = ResourceUsage.Dynamic,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None
            };

            Texture = new Texture1D( D3D10Wrapper.Instance.Device, desc );
            View = new ShaderResourceView( D3D10Wrapper.Instance.Device, Texture );
        }

        public DataStream MapForWrite()
        {
            return Texture.Map( 0, MapMode.WriteDiscard, MapFlags.None );
        }

        public void Unmap()
        {
            Texture.Unmap( 0 );
        }

        public void SaveTXT( string filename )
        {
            var device = D3D10Wrapper.Instance.Device;

            // copy to CPU
            var ctd = Texture.Description;
            ctd.Usage = ResourceUsage.Staging;
            ctd.BindFlags = BindFlags.None;
            ctd.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;

            using( var cpuTexture = new Texture1D( device, ctd ) )
            {
                device.CopyResource( Texture, cpuTexture );
                var stream = cpuTexture.Map( 0, MapMode.Read, MapFlags.None );

                using( var sw = new StreamWriter( filename ) )
                for( int i = 0; i < Length; ++i )
                {
                    var x = stream.Read< float >();
                    var y = stream.Read< float >();
                    var z = stream.Read< float >();
                    var w = stream.Read< float >();
                    sw.WriteLine( "{0}: {1}, {2}, {3}, {4}", i, x, y, z, w );
                }

                stream.Close();
                cpuTexture.Unmap( 0 );
            }
        }
    }
}

using System;
using System.Collections.Generic;
using SlimDX.Direct3D10;
using SlimDX.D3DCompiler;

namespace libcgt.DirectX.D3D10
{
    public class EffectManager
    {
        private Device device;
        private string pathPrefix;
        private Dictionary< string, Effect > effects;

        // TODO: singleton effect manager a bad idea since it depends on a prefix
        public EffectManager( Device device, string pathPrefix )
        {
            this.pathPrefix = pathPrefix;
            
            this.device = device;
            effects = new Dictionary< string, Effect >();
        }

        public Effect this[ string filename ]
        {
            get
            {
                if( !( effects.ContainsKey( filename ) ) )
                {
                    var shaderFlags = ShaderFlags.None;

                    if( ( device.CreationFlags & DeviceCreationFlags.Debug ) != 0 )
                    {
                        shaderFlags |= ShaderFlags.Debug | ShaderFlags.SkipOptimization;
                        effects[ filename ] = Effect.FromFile( device, pathPrefix + filename, "fx_4_1", shaderFlags, EffectFlags.None );
                    }
                    else
                    {                        
                        effects[ filename ] = Effect.FromFile( device, pathPrefix + filename, "fx_4_1", shaderFlags, EffectFlags.None );
                    }                    
                }
                return effects[ filename ];
            }
        }

        public EffectPass this[ string filename, string technique, string pass ]
        {
            get
            {
                return this[ filename ].GetTechniqueByName( technique ).GetPassByName( pass );
            }
        }
    }
}

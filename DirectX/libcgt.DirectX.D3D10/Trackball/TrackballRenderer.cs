using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.DirectX.D3D10.VertexFormats;
using libcgt.core;
using libcgt.core.Trackball;
using libcgt.core.Vecmath;
using SlimDX.Direct3D10;

namespace libcgt.DirectX.D3D10.Trackball
{
    // TODO: pass in device?
    public class TrackballRenderer
    {
        private int nLat;
        private int nLong;

        private Effect effect;
        private EffectPass trackballPass;
        private InputLayout trackballLayout;
        private DynamicVertexBuffer trackballBuffer;

        public TrackballRenderer( EffectManager effectManager )
            : this( effectManager, 64, 32 )
        {
            
        }

        public TrackballRenderer( EffectManager effectManager, int nLat, int nLong )
        {
            this.nLat = nLat;
            this.nLong = nLong;

            effect = effectManager[ "trackball.fx" ];
            trackballPass = effect.GetTechniqueByName( "trackball" ).GetPassByName( "p0" );
            trackballLayout = new InputLayout( D3D10Wrapper.Instance.Device, trackballPass.Description.Signature, VertexPosition3fColor4ub.InputElements );

            InitializeVertexBuffer();
        }

        private void InitializeVertexBuffer()
        {
            int nVertices = 6 * nLat * nLong;

            float dt = MathUtils.TWO_PI / nLat;
            float dp = MathUtils.PI / nLong;

            trackballBuffer = new DynamicVertexBuffer( VertexPosition3fColor4ub.SizeInBytes, nVertices );
            var stream = trackballBuffer.MapForWriteDiscard();

            var color = Color4ub.CornflowerBlue;
            color.w = 64;

            for( int i = 0; i < nLat; ++i )
            {
                float t0 = i * dt;
                float t1 = t0 + dt;

                for( int j = 0; j < nLong; ++j )
                {
                    float p0 = j * dp;
                    float p1 = p0 + dp;

                    var v00 = PointOnSphere( t0, p0, Vector3f.Zero, 1 );
                    var v10 = PointOnSphere( t1, p0, Vector3f.Zero, 1 );
                    var v01 = PointOnSphere( t0, p1, Vector3f.Zero, 1 );
                    var v11 = PointOnSphere( t1, p1, Vector3f.Zero, 1 );

                    stream.Write( v00 );
                    stream.Write( color );
                    stream.Write( v10 );
                    stream.Write( color );
                    stream.Write( v01 );
                    stream.Write( color );

                    stream.Write( v01 );
                    stream.Write( color );
                    stream.Write( v10 );
                    stream.Write( color );
                    stream.Write( v11 );
                    stream.Write( color );
                }
            }

            trackballBuffer.Unmap();
        }

        public void Render( TrackballControls controls )
        {
            var d3d = D3D10Wrapper.Instance;

            d3d.Device.InputAssembler.SetInputLayout( trackballLayout );
            d3d.Device.InputAssembler.SetPrimitiveTopology( PrimitiveTopology.TriangleList );
            d3d.Device.InputAssembler.SetVertexBuffers( 0, trackballBuffer.DefaultBinding );

            var pvw = controls.Camera.ProjectionViewMatrix;

            effect.GetVariableByName( "pvw" ).AsMatrix().SetMatrix( pvw );
            effect.GetVariableByName( "sceneCenterAndRadius" ).AsVector().Set( new Vector4f( controls.SceneCenter, controls.SceneRadius ) );
            effect.GetVariableByName( "eye" ).AsVector().Set( new Vector4f( controls.Camera.Position, 1 ) );
            trackballPass.Apply();

            d3d.Device.Draw( trackballBuffer.Count, 0 );
        }

        private static Vector3f PointOnSphere( float t, float p, Vector3f center, float radius )
        {
            float x = radius * MathUtils.Cos( t ) * MathUtils.Sin( p );
            float y = radius * MathUtils.Sin( t ) * MathUtils.Sin( p );
            float z = radius * MathUtils.Cos( p );

            return center + new Vector3f( x, y, z );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.DirectX.D3D10.VertexFormats;
using libcgt.core;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;
using SlimDX;

namespace libcgt.DirectX.D3D10
{
    public static class PrimitiveGeometryGenerator
    {
        public static VertexPosition3fNormal3fTexture2fColor4ub[] GenerateFull( Box3f box, Vector4ub color )
        {
            var dx = box.DX;
            var dy = box.DY;
            var dz = box.DZ;
            
            return new[]
            {
                // back
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V100, -dz, new Vector2f( 0, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V000, -dz, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V110, -dz, new Vector2f( 0, 0 ), color ),

                new VertexPosition3fNormal3fTexture2fColor4ub( box.V110, -dz, new Vector2f( 0, 0 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V000, -dz, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V010, -dz, new Vector2f( 1, 0 ), color ),

                // front
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V001, dz, new Vector2f( 0, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V101, dz, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V011, dz, new Vector2f( 0, 0 ), color ),

                new VertexPosition3fNormal3fTexture2fColor4ub( box.V011, dz, new Vector2f( 0, 0 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V101, dz, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V111, dz, new Vector2f( 1, 0 ), color ),

                // left
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V000, -dx, new Vector2f( 0, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V001, -dx, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V010, -dx, new Vector2f( 0, 0 ), color ),

                new VertexPosition3fNormal3fTexture2fColor4ub( box.V010, -dx, new Vector2f( 0, 0 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V001, -dx, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V011, -dx, new Vector2f( 1, 0 ), color ),

                // right
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V101, dx, new Vector2f( 0, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V100, dx, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V111, dx, new Vector2f( 0, 0 ), color ),

                new VertexPosition3fNormal3fTexture2fColor4ub( box.V111, dx, new Vector2f( 0, 0 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V100, dx, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V110, dx, new Vector2f( 1, 0 ), color ),

                // bottom
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V000, -dy, new Vector2f( 0, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V100, -dy, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V001, -dy, new Vector2f( 0, 0 ), color ),
                                                                                       
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V001, -dy, new Vector2f( 0, 0 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V100, -dy, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V101, -dy, new Vector2f( 1, 0 ), color ),

                // top
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V011, dy, new Vector2f( 0, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V111, dy, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V010, dy, new Vector2f( 0, 0 ), color ),
                                                                                       
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V010, dy, new Vector2f( 0, 0 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V111, dy, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( box.V110, dy, new Vector2f( 1, 0 ), color )
            };
        }

        // HACK: this should not be there
        // both boxes are full, this one just has its vertices inverted
        public static VertexPosition3fNormal3fTexture2fColor4ub[] Generate( Box3f box, Vector4ub color )
        {
            var vertices = new List< VertexPosition3fNormal3fTexture2fColor4ub >( 36 );

            var origin = box.V000;
            var dx = box.DX;
            var dy = box.DY;
            var dz = box.DZ;

            // bottom            
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz, Vector3f.Up, new Vector2f( 0, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz + dx, Vector3f.Up, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin, Vector3f.Up, Vector2f.Zero, color ) );

            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin, Vector3f.Up, Vector2f.Zero, color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz + dx, Vector3f.Up, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx, Vector3f.Up, new Vector2f( 1, 0 ), color ) );

            // top
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dy, -Vector3f.Up, new Vector2f( 0, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy, -Vector3f.Up, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dy + dz, -Vector3f.Up, Vector2f.Zero, color ) );

            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dy + dz, -Vector3f.Up, Vector2f.Zero, color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy, -Vector3f.Up, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy + dz, -Vector3f.Up, new Vector2f( 1, 0 ), color ) );

            // left
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz, Vector3f.Right, new Vector2f( 0, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin, Vector3f.Right, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz + dy, Vector3f.Right, Vector2f.Zero, color ) );

            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz + dy, Vector3f.Right, Vector2f.Zero, color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin, Vector3f.Right, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dy, Vector3f.Right, new Vector2f( 1, 0 ), color ) );

            // right
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx, -Vector3f.Right, new Vector2f( 0, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dz, -Vector3f.Right, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy, -Vector3f.Right, Vector2f.Zero, color ) );

            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy, -Vector3f.Right, Vector2f.Zero, color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dz, -Vector3f.Right, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy + dz, -Vector3f.Right, new Vector2f( 1, 0 ), color ) );

            // front
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dz, Vector3f.Forward, new Vector2f( 0, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz, Vector3f.Forward, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy + dz, Vector3f.Forward, Vector2f.Zero, color ) );

            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy + dz, Vector3f.Forward, Vector2f.Zero, color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dz, Vector3f.Forward, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dy + dz, Vector3f.Forward, new Vector2f( 1, 0 ), color ) );

            // back
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin, -Vector3f.Forward, new Vector2f( 0, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx, -Vector3f.Forward, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dy, -Vector3f.Forward, Vector2f.Zero, color ) );

            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dy, -Vector3f.Forward, Vector2f.Zero, color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx, -Vector3f.Forward, new Vector2f( 1, 1 ), color ) );
            vertices.Add( new VertexPosition3fNormal3fTexture2fColor4ub( origin + dx + dy, -Vector3f.Forward, new Vector2f( 1, 0 ), color ) );

            return vertices.ToArray();
        }

        public static VertexPosition3fNormal3fTexture2f[] Generate( Rectangle3f rect )
        {
            var normal = rect.Normal;

            return new[]
            {
                new VertexPosition3fNormal3fTexture2f( rect.V00, normal, new Vector2f( 0, 1 ) ),
                new VertexPosition3fNormal3fTexture2f( rect.V10, normal, new Vector2f( 1, 1 ) ),
                new VertexPosition3fNormal3fTexture2f( rect.V01, normal, new Vector2f( 0, 0 ) ),

                new VertexPosition3fNormal3fTexture2f( rect.V01, normal, new Vector2f( 0, 0 ) ),
                new VertexPosition3fNormal3fTexture2f( rect.V10, normal, new Vector2f( 1, 1 ) ),
                new VertexPosition3fNormal3fTexture2f( rect.V11, normal, new Vector2f( 1, 0 ) )
            };
        }

        public static VertexPosition3fNormal3fTexture2fColor4ub[] Generate( Rectangle3f rect, Vector4ub color )
        {
            var normal = rect.Normal;

            return new[]
            {
                new VertexPosition3fNormal3fTexture2fColor4ub( rect.V00, normal, new Vector2f( 0, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( rect.V10, normal, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( rect.V01, normal, new Vector2f( 0, 0 ), color ),

                new VertexPosition3fNormal3fTexture2fColor4ub( rect.V01, normal, new Vector2f( 0, 0 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( rect.V10, normal, new Vector2f( 1, 1 ), color ),
                new VertexPosition3fNormal3fTexture2fColor4ub( rect.V11, normal, new Vector2f( 1, 0 ), color )
            };
        }

        public static VertexPosition3fColor4ub[] Generate( OldCamera camera, Vector4ub color )
        {
            var center = camera.Position;
  
            var rect = camera.GetAlignedRectAt( camera.ZNear );
            var zn00 = rect.V00;
            var zn10 = rect.V10;
            var zn01 = rect.V01;
            var zn11 = rect.V11;

            return new[]
            {
                // zNear quad
                new VertexPosition3fColor4ub( zn00, color ),
                new VertexPosition3fColor4ub( zn10, color ),

                new VertexPosition3fColor4ub( zn10, color ),
                new VertexPosition3fColor4ub( zn11, color ),

                new VertexPosition3fColor4ub( zn11, color ),
                new VertexPosition3fColor4ub( zn01, color ),

                new VertexPosition3fColor4ub( zn01, color ),
                new VertexPosition3fColor4ub( zn00, color ),

                // outgoing rays
                new VertexPosition3fColor4ub( center, color ),
                new VertexPosition3fColor4ub( zn00, color ),

                new VertexPosition3fColor4ub( center, color ),
                new VertexPosition3fColor4ub( zn10, color ),

                new VertexPosition3fColor4ub( center, color ),
                new VertexPosition3fColor4ub( zn01, color ),

                new VertexPosition3fColor4ub( center, color ),
                new VertexPosition3fColor4ub( zn11, color )
            };
        }

        /// <summary>
        /// Returns a triangle list
        /// </summary>
        /// <param name="light"></param>
        /// <param name="z"></param>
        /// <param name="nSamples"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static VertexPosition3fColor4ub[] Generate( SpotLight light, float z, int nSamples, Vector4ub color )
        {
            var circle = light.GetAlignedCircleAt( z );
            var output = new VertexPosition3fColor4ub[ 3 * nSamples ];

            var apex = new VertexPosition3fColor4ub( light.Position, color );

            for( int i = 0; i < nSamples - 1; ++i )
            {
                output[ 3 * i ] = apex;
                
                float t0 = ( float ) ( 2 * Math.PI ) * Arithmetic.Fraction( i, nSamples );

                float t1;
                if( i == nSamples - 1 )
                {
                    t1 = 0;
                }
                else
                {
                    t1 = ( float ) ( 2 * Math.PI ) * Arithmetic.Fraction( i + 1, nSamples );
                }                

                var p0 = circle.Sample( t0 );
                var p1 = circle.Sample( t1 );
                
                output[ 3 * i + 1 ] = new VertexPosition3fColor4ub( p0, color );
                output[ 3 * i + 2 ] = new VertexPosition3fColor4ub( p1, color );
            }

            return output;
        }

        // TODO: return a rectangle3f, just like OldCamera        
        public static void GetAlignedQuadAt( SpotLight light, float z, out Vector3f bottomLeft, out Vector3f bottomRight,
            out Vector3f topLeft, out Vector3f topRight )             
        {
            float fTop = ( float ) ( z * Math.Tan( 0.5f * light.FieldOfView ) );
            float fBottom = -fTop;
            float fRight = fTop;
            float fLeft = -fRight;

            var eye = light.Position;
            var forward = light.Direction;
            var up = light.Up;

            var worldRight = Vector3f.Cross( forward, up );
            topLeft = eye + fTop * up + fLeft * worldRight + z * forward;
            topRight = eye + fTop * up + fRight * worldRight + z * forward;
            bottomLeft = eye + fBottom * up + fLeft * worldRight + z * forward;
            bottomRight = eye + fBottom * up + fRight * worldRight + z * forward;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.DirectX.D3D10;
using libcgt.DirectX.D3D10.VertexFormats;
using libcgt.core.Vecmath;
using SlimDX.Direct3D10;

namespace libcgt.Visualizer
{
    public class Visualizer
    {
        private ItemTree rootItemTree;
        
        private D3D10Wrapper d3d;
        private Effect effect;
        
        private EffectPass opaquePass;
        private InputLayout opaquePassLayout;

        private EffectPass alphaPass;
        private InputLayout alphaPassLayout;

        private EffectPass additivePass;
        private InputLayout additivePassLayout;
        
        private EffectPass opaqueTexturePass;
        private InputLayout opaqueTexturePassLayout;

        private EffectPass alphaTexturePass;
        private InputLayout alphaTexturePassLayout;

        private VisualizerStreamState streamState;
        private DynamicVertexBuffer textureVertexBuffer;

        public Visualizer( D3D10Wrapper d3d, EffectManager effectManager )
        {
            rootItemTree = new ItemTree( "root" );

            this.d3d = d3d;
            
            streamState = new VisualizerStreamState();
            textureVertexBuffer = new DynamicVertexBuffer( VertexPosition4fColor4f.SizeInBytes, 6 );
            var stream = textureVertexBuffer.MapForWriteDiscard();
            stream.Write( new Vector4f( 0, 0, 0, 1 ) );
            stream.Write( new Vector4f( 0, 1, 0, 1 ) );
            stream.Write( new Vector4f( 1, 0, 0, 1 ) );
            stream.Write( new Vector4f( 1, 1, 0, 1 ) );
            stream.Write( new Vector4f( 0, 1, 0, 1 ) );
            stream.Write( new Vector4f( 0, 0, 0, 1 ) );

            stream.Write( new Vector4f( 0, 1, 0, 1 ) );
            stream.Write( new Vector4f( 0, 0, 0, 1 ) );
            stream.Write( new Vector4f( 1, 0, 0, 1 ) );
            stream.Write( new Vector4f( 1, 1, 0, 1 ) );
            stream.Write( new Vector4f( 1, 1, 0, 1 ) );
            stream.Write( new Vector4f( 1, 0, 0, 1 ) );
            textureVertexBuffer.Unmap();

            effect = effectManager[ "visualizer.fx" ];

            opaquePass = effectManager[ "visualizer.fx", "renderOpaque", "p0" ];
            opaquePassLayout = new InputLayout( d3d.Device, opaquePass.Description.Signature, VertexPosition4fColor4f.InputElements );

            alphaPass = effectManager[ "visualizer.fx", "renderAlpha", "p0" ];
            alphaPassLayout = new InputLayout( d3d.Device, alphaPass.Description.Signature, VertexPosition4fColor4f.InputElements );

            additivePass = effectManager[ "visualizer.fx", "renderAdditive", "p0" ];
            additivePassLayout = new InputLayout( d3d.Device, additivePass.Description.Signature, VertexPosition4fColor4f.InputElements );
            
            opaqueTexturePass = effect.GetTechniqueByName( "renderTexOpaque" ).GetPassByName( "p0" );
            opaqueTexturePassLayout = new InputLayout( d3d.Device, opaqueTexturePass.Description.Signature, VertexPosition4fTexture4f.InputElements );

            alphaTexturePass = effect.GetTechniqueByName( "renderTexAlpha" ).GetPassByName( "p0" );
            alphaTexturePassLayout = new InputLayout( d3d.Device, alphaTexturePass.Description.Signature, VertexPosition4fTexture4f.InputElements );
        }

        public void Add( IVisualizerItem item )
        {
            rootItemTree.Add( item );
        }

        public void Add( IEnumerable< IVisualizerItem > items )
        {
            rootItemTree.Add( items );
        }

        public void Add( string treePath, IVisualizerItem item )
        {
            FindSubtreeByPath( treePath ).Add( item );
        }

        public void Add( string treePath, IEnumerable< IVisualizerItem > items )
        {
            FindSubtreeByPath( treePath ).Add( items );
        }

        public void CreateSubTree( string parentPath, string treeName )
        {
            CreateSubTree( parentPath, treeName, true );
        }

        public void CreateSubTree( string parentPath, string treeName, bool active )
        {
            FindSubtreeByPath( parentPath ).CreateSubtree( treeName, active );
        }

        public void ActivateSubtree( string treePath )
        {
            FindSubtreeByPath( treePath ).Active = true;
        }

        public void DeactivateSubtree( string treePath )
        {
            FindSubtreeByPath( treePath ).Active = true;
        }

        public void Clear()
        {
            rootItemTree.RecursiveClear();
        }

        public void ClearSubtree( string treePath )
        {
            FindSubtreeByPath( treePath ).RecursiveClear();
        }

        public ItemTree FindSubtreeByPath( string path )
        {
            var tokens = path.Split( new[] { '.' }, StringSplitOptions.RemoveEmptyEntries );
            if( tokens[ 0 ] != "root" )
            {
                throw new ArgumentException( "first token must be \"root\"" );
            }

            var currentTree = rootItemTree;
            for( int i = 1; i < tokens.Length; ++i )
            {
                currentTree = currentTree.Subtrees[ tokens[ i ] ];
            }

            return currentTree;
        }
        
        public ItemTree this[ string path ]
        {
            get
            {
                return FindSubtreeByPath( path );
            }
        }

        public void RenderAlphaTexture( ShaderResourceView textureSRV, Rect2f rect, Matrix4f pvw, bool yPointsUp )
        {
            RenderTexture( textureSRV, rect, pvw, false, yPointsUp );
        }

        public void RenderOpaqueTexture( ShaderResourceView textureSRV, Rect2f rect, Matrix4f pvw, bool yPointsUp )
        {
            RenderTexture( textureSRV, rect, pvw, true, yPointsUp );
        }

        /// <summary>
        /// Renders a texture "textureSRV" into a rectangle rect.
        /// Rect is always specified such that x points right and y points up,
        /// but can be modified by pvw.
        /// 
        /// opaque specifies whether to use the opaque or alpha pass.
        /// 
        /// yPointsUp specifies that in the *input image* textureSRV, whether y points up
        /// If false, then [0,0] is the top left corner, else it's the bottom left corner
        /// </summary>
        /// <param name="textureSRV"></param>
        /// <param name="rect"></param>
        /// <param name="pvw"></param>
        /// <param name="opaque"></param>
        /// <param name="yPointsUp"></param>
        private void RenderTexture( ShaderResourceView textureSRV, Rect2f rect, Matrix4f pvw, bool opaque, bool yPointsUp )
        {
            // TODO: split: passing in opaque is stupid

            var viewportMatrix = Matrix4f.Identity;
            var textureMatrix = Matrix4f.Identity;

            viewportMatrix.m00 = rect.Width;
            viewportMatrix.m11 = rect.Height;
            viewportMatrix.m03 = rect.Origin.x;
            viewportMatrix.m13 = rect.Origin.y;

            pvw = pvw * viewportMatrix;

            if( yPointsUp )
            {
                textureMatrix.m11 = -1;
                textureMatrix.m13 = 1;
            }            

            effect.GetVariableByName( "pvw" ).AsMatrix().SetMatrix( pvw );
            effect.GetVariableByName( "textureMatrix" ).AsMatrix().SetMatrix( textureMatrix );
            effect.GetVariableByName( "tex" ).AsResource().SetResource( textureSRV );

            d3d.Device.InputAssembler.SetVertexBuffers( 0, textureVertexBuffer.DefaultBinding );
            d3d.Device.InputAssembler.SetPrimitiveTopology( PrimitiveTopology.TriangleList );

            if( opaque )
            {
                d3d.Device.InputAssembler.SetInputLayout( opaqueTexturePassLayout );
                opaqueTexturePass.Apply();
            }
            else
            {
                d3d.Device.InputAssembler.SetInputLayout( alphaTexturePassLayout );
                alphaTexturePass.Apply();
            }            
            
            d3d.Device.Draw( 6, 0 );
        }

        public void Render( Matrix4f pvw )
        {
            effect.GetVariableByName( "pvw" ).AsMatrix().SetMatrix( pvw );            

            streamState.Begin();

            foreach( BlendType pass in Enum.GetValues( typeof( BlendType ) ) )            
            {
                ApplyPass( pass );
                RenderPass( pass );
            }

            streamState.End();
        }

        private void ApplyPass( BlendType blendType )
        {
            switch( blendType )
            {
                case BlendType.Opaque:
                    opaquePass.Apply();
                    d3d.Device.InputAssembler.SetInputLayout( opaquePassLayout );
                    break;

                case BlendType.Additive:
                    additivePass.Apply();
                    d3d.Device.InputAssembler.SetInputLayout( additivePassLayout );
                    break;

                case BlendType.Alpha:
                    alphaPass.Apply();
                    d3d.Device.InputAssembler.SetInputLayout( alphaPassLayout );
                    break;                

                default:
                    break;
            }
        }

        private void RenderPass( BlendType blendType )
        {
            var prevItemType = ItemType.None;

            foreach( var item in rootItemTree.ItemsInSubTree() )
            {
                if( blendType != item.BlendType )
                {
                    continue;
                }

                var currentItemType = item.ItemType;

                // if the current item is different than the previous item
                if( currentItemType != prevItemType )
                {
                    // and it's not the first item type
                    // then render it and reset the stream
                    if( prevItemType != ItemType.None )
                    {
                        Render( prevItemType );
                    }
                }

                // if it doesn't fit
                // then render it and reset the stream
                if( !streamState.CanAppendItem( item ) )
                {
                    Render( prevItemType );                    
                }
                
                // then add it to the stream
                streamState.AppendItem( item );
                
                prevItemType = currentItemType;             
            }

            if( streamState.NumVerticesInCurrentStream > 0 )
            {
                Render( prevItemType );
            }
        }

        private void Render( ItemType itemType )
        {
            streamState.BeginRender();

            // set topology based on item type
            switch( itemType )
            {
                case ItemType.Point:
                    d3d.Device.InputAssembler.SetPrimitiveTopology( PrimitiveTopology.PointList );
                    break;

                case ItemType.Line:
                    d3d.Device.InputAssembler.SetPrimitiveTopology( PrimitiveTopology.LineList );
                    break;

                case ItemType.Triangle:
                    d3d.Device.InputAssembler.SetPrimitiveTopology( PrimitiveTopology.TriangleList );
                    break;
            }

            // bind vertex buffer and draw
            d3d.Device.InputAssembler.SetVertexBuffers( 0, streamState.CurrentVertexBuffer.DefaultBinding );
            d3d.Device.Draw( streamState.NumVerticesInCurrentStream, 0 );

            streamState.EndRender();
        }        
    }
}

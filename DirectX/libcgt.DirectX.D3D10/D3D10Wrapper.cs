using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;
using SlimDX;
using SlimDX.DXGI;
using SlimDX.Direct3D10;
using SlimDX.Direct3D10_1;

namespace libcgt.DirectX.D3D10
{
    // Singleton
    public class D3D10Wrapper
    {
        // StackManager
        // can't pop off the bottom one

        public Stack< Matrix4f > ModelMatrixStack { get; private set; }

        // TODO: allow the user to disable automatic viewport stack management?        
        public Stack< Viewport > ViewportStack { get; private set; }
        
        // TODO: stack of back buffers

        public static D3D10Wrapper Instance { get; private set; }
        public SlimDX.Direct3D10_1.Device1 Device { get; private set; }

        private Factory factory;

        private ModeDescription modeDescription;
        private SwapChainDescription swapChainDescription;
        private SwapChain swapChain;
        
        private DepthStencilTarget depthStencilBuffer;

        private RenderTargetView renderTargetView;
        private Texture2D colorBufferTexture;
        private Surface colorBufferSurface;

        public Texture2D ColorBufferAsTexture
        {
            get
            {
                return colorBufferTexture;
            }
        }

        public Surface ColorBufferAsSurface
        {
            get
            {
                return colorBufferSurface;
            }
        }

        public RenderTargetView ColorBufferAsRenderTargetView
        {
            get
            {
                return renderTargetView;
            }
        }

        public DepthStencilTarget DepthStencilBuffer
        {
            get
            {
                return depthStencilBuffer;
            }
        }

        // TODO: pass in debug as a parameter...
        public D3D10Wrapper( IntPtr hWnd, int width, int height )            
        {
            if( Instance != null )
            {
                throw new ApplicationException( "Attempted to create another D3D wrapper while one already exists." );
            }
            Instance = this;

            factory = new Factory();
#if DEBUG
            Device = new SlimDX.Direct3D10_1.Device1( DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_1 );
#else
            Device = new SlimDX.Direct3D10_1.Device1( DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_1 );
#endif
            CreateSwapChain( hWnd, width, height );

            CreateDepthStencilBuffers( width, height );            
            Device.OutputMerger.SetTargets( depthStencilBuffer.DepthStencilView, renderTargetView );

            ModelMatrixStack = new Stack< Matrix4f >();            

            ViewportStack = new Stack< Viewport >();
            var viewport = new Viewport( 0, 0, width, height, 0, 1 );
            PushViewport( viewport );
        }

        /// <summary>
        /// Handles a resize event by:
        /// * resizing the window render target (and swap chain)
        /// * resizing the window depth and stencil buffers
        /// * resetting the viewport stack to just one element sized to fit the window
        /// 
        /// Typically, this would be call by the window resize handler
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void HandleResize( int width, int height )
        {
            ResizeSwapchain( width, height );
            ResizeDepthStencilBuffer( width, height );

            // TODO: clear the stack
            RestoreBackBuffer();

            // resize the viewport
            ViewportStack.Clear();
            var viewport = new Viewport( 0, 0, width, height, 0, 1 );
            PushViewport( viewport );
        }

        public Vector2i TopViewportSize()
        {
            var vp = TopViewport();
            return new Vector2i( vp.Width, vp.Height );
        }

        public Viewport TopViewport()
        {
            return ViewportStack.Peek();
        }

        public void PushViewport( Viewport viewport )
        {
            ViewportStack.Push( viewport );
            Device.Rasterizer.SetViewports( viewport );
        }

        public void PopViewport()
        {
            ViewportStack.Pop();
            Device.Rasterizer.SetViewports( ViewportStack.Peek() );
        }

        private void ResizeSwapchain( int width, int height )
        {
            modeDescription.Width = width;
            modeDescription.Height = height;
            swapChainDescription.ModeDescription = modeDescription;

            renderTargetView.Dispose();
            colorBufferTexture.Dispose();
            colorBufferSurface.Dispose();

            swapChain.ResizeBuffers( 1, width, height, Format.R8G8B8A8_UNorm, SwapChainFlags.None );
            CreateColorBufferDerivedObjects();
        }

        private void ResizeDepthStencilBuffer( int width, int height )
        {
            depthStencilBuffer.Dispose();
            CreateDepthStencilBuffers( width, height );
        }

        public void Present()
        {
            // TODO: figure out what syncInterval means
            Present( 1 );
        }

        public void Present( int syncInterval )
        {
            swapChain.Present( syncInterval, PresentFlags.None );
        }

        public void ClearBuffers()
        {
            ClearColorBuffer();
            ClearDepthBuffer();
        }

        public void ClearBuffers( Vector4f color )
        {
            ClearBuffers( color, 1 );
        }

        public void ClearBuffers( Vector4f color, float depth )
        {
            ClearColorBuffer( color );
            ClearDepthBuffer( depth );
        }

        /// <summary>
        /// Clears the window's color buffer to transparent black (0,0,0,0)
        /// </summary>
        public void ClearColorBuffer()
        {
            ClearColorBuffer( renderTargetView );
        }

        /// <summary>
        /// Clears the window's color buffer to "color"
        /// </summary>
        /// <param name="color"></param>
        public void ClearColorBuffer( Vector4f color )
        {
            ClearColorBuffer( renderTargetView, color );
        }

        /// <summary>
        /// Clears a render target view to transparent black
        /// </summary>
        /// <param name="view"></param>
        public void ClearColorBuffer( RenderTargetView view )
        {
            ClearColorBuffer( view, Color4f.TransparentBlack );
        }

        /// <summary>
        /// Clears a render target view to "color"
        /// </summary>
        /// <param name="view"></param>
        /// <param name="color"></param>
        public void ClearColorBuffer( RenderTargetView view, Vector4f color )
        {
            Device.ClearRenderTargetView( view, color.ToD3DColor4() );
        }

        /// <summary>
        /// Clears the depth buffer to 1
        /// </summary>
        public void ClearDepthBuffer()
        {
            ClearDepthBuffer( 1 );
        }

        /// <summary>
        /// Clears the window depth buffer to "depth"
        /// </summary>
        /// <param name="depth"></param>
        public void ClearDepthBuffer( float depth )
        {
            ClearDepthBuffer( depthStencilBuffer.DepthStencilView, depth );
        }        

        /// <summary>
        /// Clears a DepthStencilView to 1
        /// </summary>
        /// <param name="view"></param>
        public void ClearDepthBuffer( DepthStencilView view )
        {
            ClearDepthBuffer( view, 1 );
        }

        /// <summary>
        /// Clears a DepthStencilView to "depth"
        /// </summary>
        /// <param name="view"></param>
        /// <param name="depth"></param>
        public void ClearDepthBuffer( DepthStencilView view, float depth )
        {
            Device.ClearDepthStencilView( view, DepthStencilClearFlags.Depth, depth, 0 );
        }
        
        // TODO: push/pop?
        /// <summary>
        /// Resets the output merger to target the screen depth and color buffers.
        /// </summary>
        public void RestoreBackBuffer()
        {
            Device.OutputMerger.SetTargets( depthStencilBuffer.DepthStencilView, renderTargetView );
        }

        public void SaveBackBufferColorTXT( string filename )
        {
            using( var backBuffer = Texture2D.FromSwapChain< Texture2D >( swapChain, 0 ) )
            {
                backBuffer.SaveTXT( filename );
            }
        }

        public void SaveBackBufferColorPNG( string filename )
        {
            using( var backBuffer = Texture2D.FromSwapChain< Texture2D >( swapChain, 0 ) )
            {
                backBuffer.SavePNG( filename );
            }
        }

        private void CreateSwapChain( IntPtr hWnd, int width, int height )
        {
            modeDescription = new ModeDescription( width, height, new Rational( 60, 1 ), Format.R8G8B8A8_UNorm )
            {
                Scaling = DisplayModeScaling.Centered,
                ScanlineOrdering = DisplayModeScanlineOrdering.Progressive
            };

            swapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                ModeDescription = modeDescription,
                Usage = Usage.RenderTargetOutput,                
                OutputHandle = hWnd,
                SampleDescription = new SampleDescription( 1, 0 ),
                IsWindowed = true
            };
            
            swapChain = new SwapChain( factory, Device, swapChainDescription );
            CreateColorBufferDerivedObjects();
        }

        private void CreateDepthStencilBuffers( int width, int height )
        {
            depthStencilBuffer = DepthStencilTarget.CreateDepth( width, height );
        }

        private void CreateColorBufferDerivedObjects()
        {
            colorBufferSurface = Surface.FromSwapChain( swapChain, 0 );
            colorBufferTexture = SlimDX.Direct3D10.Resource.FromSwapChain< Texture2D >( swapChain, 0 );
            renderTargetView = new RenderTargetView( Device, colorBufferTexture );
        }
    }
}

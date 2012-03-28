using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace libcgt.DirectX.D3D10
{
    public class D3D10Control : UserControl
    {
        public D3D10Wrapper D3D { get; private set; }

        // Attach a delegate to the Resize event
        // to receive notifications.
        // After resizing, Paint is automatically called.
        public event Action< int, int, D3D10Wrapper > ResizeD3D;
        public event Action< D3D10Wrapper > DrawD3D;

        /// <summary>
        /// Call this delegate to cause a repaint
        /// </summary>
        public Action RefreshDelegate;

        public D3D10Control()
        {
            D3D = new D3D10Wrapper( Handle, Width, Height );
            RefreshDelegate = new Action( Refresh );
        }

        /// <summary>
        /// Override this method if you want to handle resize calls and
        /// don't want to attach a delegate.
        /// The default implementation raises the ResizeD3D event.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected virtual void OnResizeD3D( int width, int height )
        {
            if( ResizeD3D != null )
            {
                ResizeD3D( Width, Height, D3D );
            }
        }

        /// <summary>
        /// Override this method if you want to draw and not attach a delegate
        /// The default implementatnio raises the delegate DrawD3D.
        /// </summary>
        protected virtual void OnDrawD3D()
        {
            if( DrawD3D != null )
            {
                DrawD3D( D3D );
            }
        }        

        protected override void OnResize( EventArgs e )
        {
            D3D.HandleResize( Width, Height );

            OnResizeD3D( Width, Height );
            
            Refresh();
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            // Don't paint unless I'm visible
            // This fixes the WPF interop flickering
            if( Visible )   
            {            
                OnDrawD3D();
                D3D.Present();
            }
        }

        protected override void OnPaintBackground( PaintEventArgs e )
        {
            // Don't paint background unless I'm actually invisible
            // This fixes the WPF interop flickering
            if( !Visible )
            {
                base.OnPaintBackground( e );
            }
        }
    }
}

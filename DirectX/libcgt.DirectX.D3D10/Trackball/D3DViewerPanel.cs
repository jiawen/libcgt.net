#if false

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using libcgt.Trackball;
using libcgt.core.Vecmath;
using SlimDX;

namespace libcgt.DirectX.D3D10.Trackball
{
    public class D3DViewerPanel : D3D10Control
    {
        // Camera
        private Camera camera;
        private bool cameraIsEdited;
        private float previousCameraZClippingCoefficient;
        // int previousPathId_; // Double key press recognition

        // manipulated frame
        private libcgt.Trackball.ManipulatedFrame manipulatedFrame;
        private bool manipulatedFrameIsACamera;

        private bool updateOK;

        public D3DViewerPanel() : base()
        {
            updateOK = false;

            // TODO: this is stupid
            this.camera = new Camera();
            this.Camera = this.camera;

            manipulatedFrame = null;
            manipulatedFrameIsACamera = false;

            SceneRadius = 1.0f;
            ShowEntireScene();

            updateOK = true;
        }

        public float SceneRadius
        {
            get
            {
                return camera.SceneRadius;
            }
            set
            {
                camera.SceneRadius = value;
            }
        }

        public Vector3f SceneCenter
        {
            get
            {
                return camera.SceneCenter;
            }
            set
            {
                camera.SceneCenter = value;
            }
        }

        public void SetSceneBoundingBox( Vector3f min, Vector3f max )
        {
            camera.SetSceneBoundingBox( min, max );
        }

        public void ShowEntireScene()
        {
            Camera.ShowEntireScene();

            if( updateOK )
            {
                Repaint();
            }
        }

        public Camera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                if( value == null )
                {
                    return;
                }

                value.SceneRadius = SceneRadius;
                value.SceneCenter = SceneCenter;
                value.ScreenSize = new Vector2i( Width, Height );

                // disconnect current camera from this viewer
                Camera.Frame.Manipulated -= Repaint;
                Camera.Frame.Spun -= Repaint;

                // conect camera frame to this viewer
                value.Frame.Manipulated += Repaint;
                value.Frame.Spun += Repaint;
                
                //TODO: 
                //connectAllCameraKFIInterpolatedSignals(false);
                camera = value;
                //connectAllCameraKFIInterpolatedSignals();

                previousCameraZClippingCoefficient = camera.ZClippingCoefficient;
            }
        }

        public libcgt.Trackball.ManipulatedFrame ManipulatedFrame
        {
            get
            {
                return manipulatedFrame;
            }
            set
            {
                if( ManipulatedFrame != null )
                {
                    if( ManipulatedFrame != Camera.Frame )
                    {
                        ManipulatedFrame.Manipulated -= Repaint;
                        ManipulatedFrame.Spun -= Repaint;
                    }
                }

                this.manipulatedFrame = value;

                this.manipulatedFrameIsACamera =
                    (
                        ( this.ManipulatedFrame != this.Camera.Frame ) &&
                        ( this.ManipulatedFrame is ManipulatedCameraFrame )
                    );

                if( this.ManipulatedFrame != null )
                {
                    // Prevent multiple connections, that would result in useless display updates
                    if( this.ManipulatedFrame != this.Camera.Frame )
                    {
                        this.ManipulatedFrame.Manipulated += Repaint;
                        this.ManipulatedFrame.Spun += Repaint;
                    }
                }
            }
        }

        protected virtual void Draw()
        {

        }

        protected override void DrawD3D()
        {
            updateOK = false;

            Draw();

            updateOK = true;
            
            // TODO:
            // emit drawFinished( true );
        }

        protected override void ResizeD3D( int width, int height )
        {
            Camera.ScreenSize = new Vector2i( width, height );
        }

        protected override void OnMouseDown( MouseEventArgs e )
        {
            base.OnMouseDown( e );

            // See QGLViewer.cpp:1470            
            if( e.Button == MouseButtons.Left )
            {
                Camera.Frame.StartAction( MouseAction.ROTATE );
                Camera.Frame.HandleMouseDown( e.X, e.Y );
            }
            else if( e.Button == MouseButtons.Right )
            {
                Camera.Frame.StartAction( MouseAction.TRANSLATE );
                Camera.Frame.HandleMouseDown( e.X, e.Y );
            }
            else if( e.Button == MouseButtons.Middle )
            {
                Camera.Frame.StartAction( MouseAction.ZOOM );
                Camera.Frame.HandleMouseDown( e.X, e.Y );
            }
        }

        protected override void OnMouseMove( MouseEventArgs e )
        {
            base.OnMouseMove( e );

            // if( Camera.Frame.IsManipulated )
            if( e.Button == MouseButtons.Left )
            {
                Camera.Frame.HandleMouseMoved( e.X, e.Y, Camera );
            }
            else if( e.Button == MouseButtons.Right )
            {
                Camera.Frame.HandleMouseMoved( e.X, e.Y, Camera );
            }
            else if( e.Button == MouseButtons.Middle )
            {
                Camera.Frame.HandleMouseMoved( e.X, e.Y, Camera );
            }
        }

        protected override void OnMouseUp( MouseEventArgs e )
        {
            base.OnMouseUp( e );

            Camera.Frame.HandleMouseUp( e.X, e.Y, Camera );
        }

        protected override void OnMouseWheel( MouseEventArgs e )
        {
            Console.WriteLine( "OnMouseWheel" );
            base.OnMouseWheel( e );

            Camera.Frame.StartAction( MouseAction.ZOOM );
            Camera.Frame.HandleMouseWheel( e.Delta, Camera );
        }

        protected override void OnMouseDoubleClick( MouseEventArgs e )
        {
            // base.OnMouseDoubleClick( e );

            Camera.Frame.AlignWithFrame( null, true );
            
            // TODO: hack:
            Repaint();
        }
    }
}

#endif
using System;
using libcgt.core.Vecmath;

namespace libcgt.core.Trackball
{
    public class ManipulatedFrame : Frame
    {
        // Events        
        public event EmptyEventHandler Manipulated;
        public event EmptyEventHandler Spun;

        // sensitivity
        public float RotationSensitivity { get; set; }
        public float TranslationSensitivity { get; set; }
        public float SpinningSensitivity { get; set; }
        public float WheelSensitivity { get; set; }

        // mouse speed and spinning
        private int lastMoveTime;
        private float mouseSpeed;
        private int delay;
        public Quat4f SpinningQuaternion { get; set; }
        
        // Whether the SCREEN_TRANS direction (horizontal or vertical) is fixed or not.
        private bool directionIsFixed;

        protected MouseAction action;

        // TODO: vector2i
        protected int prevX;
        protected int prevY;
        protected int pressedX;
        protected int pressedY;

        public ManipulatedFrame()
        {
            action = MouseAction.NO_MOUSE_ACTION;
            RotationSensitivity = 1.0f;
            TranslationSensitivity = 1.0f;
            SpinningSensitivity = 0.3f;
            WheelSensitivity = 1.0f;

            // previousConstraint = false;
        }

        // TODO: withConstraint = true
        // TODO: in qglviewer, this is a friend class qglviewer thing
        public virtual void StartAction( MouseAction action )
        {
            this.action = action;

            // TODO:
            //if( withConstraint )
            //    previousConstraint_ = NULL;
            //else
            //{
            //    previousConstraint_ = constraint();
            //    setConstraint( NULL );
            //}

            switch( this.action )
            {
                case MouseAction.ROTATE:
                    // fall through (allowed in C# if there's no code)

                case MouseAction.SCREEN_ROTATE:
                    mouseSpeed = 0.0f;
                    break;

                case MouseAction.SCREEN_TRANSLATE:
                    directionIsFixed = false;
                    break;

                default:
                    break;
            }
        }

        public virtual void HandleMouseDown( int screenX, int screenY )
        {
            prevX = screenX;
            prevY = screenY;
            pressedX = screenX;
            pressedY = screenY;
        }

        public virtual void HandleMouseMoved( int screenX, int screenY, Camera camera )
        {
            switch( this.action )
            {
                case MouseAction.TRANSLATE:
                    {
                        throw new NotImplementedException();
                    }

                case MouseAction.ROTATE:
                    {
                        Vector3f trans = camera.ProjectedCoordinatesOf( Position );
                        Quat4f rot = DeformedBallQuaternion( screenX, screenY, trans.x, trans.y, camera );
                        trans = new Vector3f( -rot.x, -rot.y, -rot.z );
                        trans = camera.Frame.Orientation.Rotate( trans );
                        trans = TransformOf( trans );
                        rot.x = trans.x;
                        rot.y = trans.y;    
                        rot.z = trans.z;
                        ComputeMouseSpeed( screenX, screenY );
                        SpinningQuaternion = rot;
                        Spin();
                        
                        break;
                    }

                case MouseAction.NO_MOUSE_ACTION:
                    // Possible when the ManipulatedFrame is a MouseGrabber. This method is then called without startAction
                    // because of mouseTracking.
                    break;
            }

            if( this.action != MouseAction.NO_MOUSE_ACTION )
            {
                prevX = screenX;
                prevY = screenY;

                EmitManipulated();
            }
        }

        public virtual void HandleMouseUp( int screenX, int screenY, Camera camera )
        {
            // TODO
            //if (previousConstraint_)
            //    setConstraint(previousConstraint_);

            if( ( ( action == MouseAction.ROTATE ) || ( action == MouseAction.SCREEN_ROTATE ) ) &&
                ( mouseSpeed >= SpinningSensitivity ) )
            {
                Spin();                
            }

            action = MouseAction.NO_MOUSE_ACTION;
        }

        public virtual void HandleMouseWheel( int delta, Camera camera )
        {
            if( this.action == MouseAction.ZOOM )
            {
                float wheelSensitivityCoef = 8e-4f;
                Vector3f trans = new Vector3f( 0.0f, 0.0f, -delta * WheelSensitivity * wheelSensitivityCoef * ( camera.Position - Position ).Norm() );
                trans = camera.Frame.Orientation.Rotate( trans );

                if( ReferenceFrame != null )
                {
                    trans = ReferenceFrame.TransformOf( trans );                    
                }
                
                Translate( ref trans );
                EmitManipulated();
            }

            // TODO:
            // if( previousConstraint_ )
            // setConstraint( previousConstraint_ );

            this.action = MouseAction.NO_MOUSE_ACTION;
        }

        protected Quat4f DeformedBallQuaternion( int x, int y, float cx, float cy, Camera camera )
        {
            // Points on the deformed ball
            float px = RotationSensitivity * ( prevX - cx ) / camera.ScreenSize.x;
            float py = RotationSensitivity * ( cy - prevY ) / camera.ScreenSize.y;
            float dx = RotationSensitivity * ( x - cx ) / camera.ScreenSize.x;
            float dy = RotationSensitivity * ( cy - y ) / camera.ScreenSize.y;

            Vector3f p1 = new Vector3f( px, py, ProjectOnBall( px, py ) );
            Vector3f p2 = new Vector3f( dx, dy, ProjectOnBall( dx, dy ) );
  
            // Approximation of rotation angle
            // Should be divided by the projectOnBall size, but it is 1.0
            Vector3f axis = Vector3f.Cross( p2, p1 );
            float angle = ( float )( 2.0 * Math.Asin( Math.Sqrt( axis.NormSquared() / p1.NormSquared() / p2.NormSquared() ) ) );
            return new Quat4f( axis, angle );
        }

        protected void ComputeMouseSpeed( int screenX, int screenY )
        {
            int dx = screenX - prevX;
            int dy = screenY - prevY;

            float dist = ( float )( Math.Sqrt( dx * dx + dy * dy ) );
            mouseSpeed = dist;

            int now = Environment.TickCount;
            delay = now - lastMoveTime;
            lastMoveTime = now;

            if( delay == 0 )
            {
                // Less than a millisecond: assume delay = 1ms
                mouseSpeed = dist;
            }
            else
            {
                mouseSpeed = dist / delay;
            }
        }

        protected virtual void Spin()
        {
            Rotate( SpinningQuaternion );
        }

        protected void EmitSpun()
        {
            if( Spun != null )
            {
                Spun();
            }
        }

        protected void EmitManipulated()
        {
            if( Manipulated != null )
            {
                Manipulated();
            }
        }

        private void SpinUpdate()
        {
            Spin();
            EmitSpun();
        }

        // Just wraps around SpinUpdate()
        private void spinningTimer_Tick( object sender, EventArgs e )
        {
            SpinUpdate();
        }

        private static float ProjectOnBall( float x, float y )
        {
            float size = 1.0f;
            float size2 = size * size;
            float sizeLimit = size2 * 0.5f;

            float d = x * x + y * y;
            if( d < sizeLimit )
            {
                return ( float )( Math.Sqrt( size2 - d ) );
            }
            else
            {
                return ( float )( sizeLimit / Math.Sqrt( d ) );
            }
        }
    }
}

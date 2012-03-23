using System;
using libcgt.core.Vecmath;

namespace libcgt.core.Trackball
{
    public class ManipulatedCameraFrame : ManipulatedFrame
    {
        public float FlySpeed { get; set; }
        public Vector3f RevolveAroundPoint { get; set; }

        private float driveSpeed;
        public Vector3f FlyUpVector { get; set; }


        public ManipulatedCameraFrame()
        {
            FlyUpVector = Vector3f.Up;

            // removeFromMouseGrabberPool();
            // connect(&flyTimer_, SIGNAL(timeout()), SLOT(flyUpdate()));
        }
        
        private void FlyUpdate( object sender, EventArgs e )
        {
            FlyUpdate();
        }
        
        private void FlyUpdate()
        {
            Vector3f flyDisp = Vector3f.Zero;
            switch( action )
            {
                case MouseAction.MOVE_FORWARD:
                    flyDisp.z = -FlySpeed;
                    Translate( LocalInverseTransformOf( flyDisp ) );
                    break;
                case MouseAction.MOVE_BACKWARD:
                    flyDisp.z = FlySpeed;
                    Translate( LocalInverseTransformOf( flyDisp ) );
                    break;
                case MouseAction.DRIVE:
                    flyDisp.z = FlySpeed * driveSpeed;
                    Translate( LocalInverseTransformOf( flyDisp ) );
                    break;
                default:
                    break;
            }

            // TODO:
            // Needs to be out of the switch since ZOOM/fastDraw()/wheelEvent use this callback to trigger a final draw().
            // #CONNECTION# wheelEvent.
            // emit manipulated();
        }

        public void UpdateFlyUpVector()
        {
            FlyUpVector = InverseTransformOf( new Vector3f( 0, 1, 0 ) );
        }

        protected override void Spin()
        {
            var q = SpinningQuaternion;
            RotateAroundPoint( ref q, RevolveAroundPoint );
        }

        // TODO: withConstraint = true
        // TODO: in qglviewer, this is a friend class qglviewer thing
        public override void StartAction( MouseAction action )
        {
            base.StartAction( action );

            switch( this.action )
            {
                case MouseAction.MOVE_FORWARD:
                case MouseAction.MOVE_BACKWARD:
                case MouseAction.DRIVE:
                    /*
                     * #if QT_VERSION >= 0x040000
      flyTimer_.setSingleShot(false);
#endif
      flyTimer_.start(10);
                     * */
                    break;
                default:
                    break;
            }
        }

        public override void HandleMouseMoved( int screenX, int screenY, Camera camera )
        {
            switch( action )
            {
                case MouseAction.TRANSLATE:
                {
                    int dx = prevX - screenX;
                    int dy = prevY - screenY;
                    Vector3f trans = new Vector3f( dx, -dy, 0.0f );
                    
                    // scale to fit the screen mouse displacement
                    switch( camera.CameraType )
                    {
                        case CameraType.PERSPECTIVE:
                            
                            double fovTerm = 2.0 * Math.Tan( 0.5 * camera.FieldOfView );
                            double scale = Math.Abs( ( camera.Frame.CoordinatesOf( RevolveAroundPoint ) ).z ) / camera.ScreenSize.y;
                            trans *= ( float )( fovTerm * scale );
                            break;

                        case CameraType.ORTHOGRAPHIC:

                            Vector2f wh = camera.GetOrthoWidthHeight();
                            trans.x *= 2.0f * wh.x / camera.ScreenSize.x;
                            trans.y *= 2.0f * wh.y / camera.ScreenSize.y;
                            break;
                    }

                    Translate( InverseTransformOf( TranslationSensitivity * trans ) );
                    break;
                }

                case MouseAction.MOVE_FORWARD:
                {
                    var rot = PitchYawQuaternion( screenX, screenY, camera );
                    Rotate( ref rot );

                    // QGLViewer has the following:
                    // actual translation is made in flyUpdate().
	                //translate(inverseTransformOf(Vec(0.0, 0.0, flySpeed())));

                    // but since we're not using qglviewer:
                    Translate( InverseTransformOf( new Vector3f( 0, 0, -FlySpeed ) ) );
                    break;
                }

                case MouseAction.MOVE_BACKWARD:
                {
                    var rot = PitchYawQuaternion( screenX, screenY, camera );
                    Rotate( ref rot );

                    // QGLViewer has the following:
                    // actual translation is made in flyUpdate().
	                //translate(inverseTransformOf(Vec(0.0, 0.0, flySpeed())));

                    // but since we're not using qglviewer:
                    Translate( InverseTransformOf( new Vector3f( 0, 0, FlySpeed ) ) );
                    break;
                }

                case MouseAction.DRIVE:
                {
                    var rot = TurnQuaternion( screenX, camera );
                    Rotate( ref rot );

                    // actual translation is made in flyUpdate().
                    driveSpeed = 0.01f * ( screenY - pressedY );
                    FlyUpdate();
                    break;
                }

                case MouseAction.ZOOM:
                {
                    float coef = Math.Max
                    (
                        Math.Abs( camera.Frame.CoordinatesOf( camera.RevolveAroundPoint ).z ),
                        0.2f * camera.SceneRadius
                    );

                    Vector3f trans = new Vector3f( 0.0f, 0.0f, -coef * ( screenY - prevY ) / camera.ScreenSize.y );
		            Translate( InverseTransformOf( trans ) );
		            break;
                }

                case MouseAction.ROTATE:
                {
                    Vector3f trans = camera.ProjectedCoordinatesOf( RevolveAroundPoint );
                    Quat4f rot = DeformedBallQuaternion( screenX, screenY, trans.x, trans.y, camera );
                    ComputeMouseSpeed( screenX, screenY );
                    SpinningQuaternion = rot;
                    Spin();

                    break;
                }

                case MouseAction.MOVE_FORWARD_BACKWARD:
                {
                    float delta = FlySpeed * ( prevY - screenY );
                    Translate( InverseTransformOf( new Vector3f( 0, 0, delta ) ) );
                    break;
                }
            }

            if( action != MouseAction.NO_MOUSE_ACTION )
            {
                prevX = screenX;
                prevY = screenY;

                if( this.action != MouseAction.ZOOM_ON_REGION )
                {
                    // ZOOM_ON_REGION should not emit manipulated().
                    // prevPos_ is used to draw rectangle feedback.
                    EmitManipulated();
                }
            }
        }

        public override void HandleMouseUp( int screenX, int screenY, Camera camera )
        {
            if( ( action == MouseAction.MOVE_FORWARD ) ||
                ( action == MouseAction.MOVE_BACKWARD ) ||
                ( action == MouseAction.DRIVE ) )
            {
                // flyTimer.Stop();
            }

            if( action == MouseAction.ZOOM_ON_REGION )
            {
                throw new NotImplementedException();
                // camera->fitScreenRegion(QRect(pressPos_, event->pos()));
            }

            base.HandleMouseUp( screenX, screenY, camera );
        }

        public override void HandleMouseWheel( int delta, Camera camera )
        {
            switch( this.action )
            {
                case MouseAction.ZOOM:
                    {
                        float wheelSensitivityCoef = 8e-4f;
                        float coef = Math.Max
                            (
                                Math.Abs( ( camera.Frame.CoordinatesOf( camera.RevolveAroundPoint ) ).z ),
                                0.2f * camera.SceneRadius
                            );

                        Vector3f trans = new Vector3f( 0.0f, 0.0f, coef * delta * WheelSensitivity * wheelSensitivityCoef );
                        Translate( InverseTransformOf( trans ) );
                        EmitManipulated();
                        
                        break;
                    }

                case MouseAction.MOVE_FORWARD:
                case MouseAction.MOVE_BACKWARD:
                    {
                        Translate( InverseTransformOf( new Vector3f( 0.0f, 0.0f, 0.2f * FlySpeed * delta ) ) );
                        EmitManipulated();

                        break;
                    }
            
                default:
                    break;
            }

            // TODO:
            // if (previousConstraint_)
            // setConstraint(previousConstraint_);

            // TODO: get rid of GL
            // The wheel triggers a fastDraw. A final updateGL is needed after the last wheel event to
            // polish the rendering using draw(). Since the last wheel event does not say its name, we use
            // the flyTimer_ to trigger flyUpdate(), which emits manipulated. Two wheel events
            // separated by more than this delay milliseconds will trigger a draw().
            // const int finalDrawAfterWheelEventDelay = 400;
            
            // TODO: 
            // flyTimer_.setSingleShot(true);
            // flyTimer_.start(finalDrawAfterWheelEventDelay);

            // This could also be done *before* manipulated is emitted, so that isManipulated() returns false.
            // But then fastDraw would not be used with wheel.
            // Detecting the last wheel event and forcing a final draw() is done using the timer_.
            this.action = MouseAction.NO_MOUSE_ACTION;
        }

        // Returns a Quaternion that is a rotation around current camera Y, proportional to the horizontal mouse position
        private Quat4f TurnQuaternion( int x, Camera camera )
        {
            return new Quat4f( new Vector3f( 0, 1, 0 ), RotationSensitivity * ( prevX - x ) / camera.ScreenSize.x );
        }

        // Returns a Quaternion that is the composition of two rotations, inferred from the
        // mouse pitch (X axis) and yaw (flyUpVector() axis)
        private Quat4f PitchYawQuaternion( int x, int y, Camera camera )
        {
            var rotX = new Quat4f( new Vector3f( 1, 0, 0 ), RotationSensitivity * ( prevY - y ) / camera.ScreenSize.y );
            var rotY = new Quat4f( TransformOf( FlyUpVector ), RotationSensitivity * ( prevX - x ) / camera.ScreenSize.x );
            
            return rotY * rotX;
        }
    }
}

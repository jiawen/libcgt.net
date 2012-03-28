
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using libcgt.core;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;
using SlimDX.XInput;

namespace libcgt.DirectX.D3D10.FPSControls
{
    //        invX,  invY, translationPerPixel, radisnsPerPixel, fovRadiansPerMouseClick
    //: this( false, true, new Vector3f( 0.001f, 0.001f, 0.001f ), 0.05f, 5 ) // gautham
    //: this( false, true, new Vector3f( 0.01f, 0.01f, 0.01f ), 0.05f, 5 ) // church
    //: this( false, true, new Vector3f( 0.02f, 0.02f, 0.02f ), 0.075f, 5 ) // copier
    //: this( false, true, new Vector3f( 0.05f, 0.05f, 0.05f ), 0.1f, 5 ) // copier

    [Serializable]
    public class FPSMouseParameters
    {
        public bool InvertX { get; set; }
        public bool InvertY { get; set; }
        public Vector3f TranslationPerPixel { get; set; }
        public float RadiansPerPixel { get; set; }
        public float FovRadiansPerMouseWheelDelta { get; set; }

        public FPSMouseParameters()
        {
            InvertX = false;
            InvertY = true;

            TranslationPerPixel = new Vector3f( 0.05f, 0.05f, 0.05f );
            RadiansPerPixel = Arithmetic.DegreesToRadians( 0.25f );

            FovRadiansPerMouseWheelDelta = Arithmetic.DegreesToRadians( 0.01f );
        }
    }

    [Serializable]
    public class FPSKeyboardParameters
    {
        public float TranslationPerKeyPress;

        public FPSKeyboardParameters()
        {
            TranslationPerKeyPress = 0.25f;
        }
    }

    [Serializable]
    public class FPSXboxGamepadParameters
    {
        public bool InvertX { get; set; }
        public bool InvertY { get; set; }

        public float TranslationPerTick { get; set; }

        public float YawRadiansPerTick { get; set; }
        public float PitchRadiansPerTick { get; set; }

        public float FoVRadiansPerTick { get; set; }

        public FPSXboxGamepadParameters()
        {
            InvertX = false;
            InvertY = true;

            TranslationPerTick = 1e-6f;
            YawRadiansPerTick = Arithmetic.DegreesToRadians( -1.0f / 32000.0f );
            PitchRadiansPerTick = Arithmetic.DegreesToRadians( 1.0f / 32000.0f );

            FoVRadiansPerTick = 0.05f / 255;
        }
    }

    [Serializable]
    public class FPSControls
    {        
        public OldCamera Camera { get; set; }

        public FPSMouseParameters MouseParameters { get; set; }
        public FPSKeyboardParameters KeyboardParameters { get; set; }
        public FPSXboxGamepadParameters XboxGamepadParameters { get; set; }
        
        public Vector3f UpVector
        {
            get
            {
                return groundPlaneToWorld.GetColumn( 1 );
            }
            set
            {
                Vector3f x;
                Vector3f z;
                GeometryUtils.Basis( value, out x, out z );
                groundPlaneToWorld = new Matrix3f( x, value, z );
                worldToGroundPlane = groundPlaneToWorld.Inverse();
            }
        }

        private bool mouseIsDown;
        private Vector2i mouseDownXY;

        private Matrix3f groundPlaneToWorld;
        private Matrix3f worldToGroundPlane;

        // TODO: force passing in a camera
        // TODO: throw exception if camera is null
        public FPSControls()
            : this( new FPSMouseParameters(), new FPSKeyboardParameters(), new FPSXboxGamepadParameters(), null )
        {
            
        }

        public FPSControls( FPSMouseParameters mouseParameters,
            FPSKeyboardParameters keyboardParameters,
            FPSXboxGamepadParameters xboxGamepadParameters,
            OldCamera camera )
        {
            MouseParameters = mouseParameters;
            KeyboardParameters = keyboardParameters;
            XboxGamepadParameters = xboxGamepadParameters;

            UpVector = Vector3f.Up;

            Camera = camera;
            mouseIsDown = false;
        }

#if false
        // TODO:
        /// <summary>
        /// Finds the theta and phi to best fit the camera
        /// </summary>
        /// <param name="camera"></param>
        public void FitThetaPhi( OldCamera camera )
        {
            var cameraForward = camera.ViewDirection;
            var cameraUp = camera.UpVector;

            // compute theta:
            // find theta such that the up vector
            // when rotated about groundPlaneUp lies in the 
            // (ground plane forward)-(ground plane up) plane

            // which is just the angle theta it makes projected onto the ground plane
            
            // TODO: define a ground plane point as well?  make it an affine basis...

            // first: take the incoming up vector in world coordinates
            // and transform it into the ground plane basis

            // var gpToWorld = new Matrix3f( GroundPlaneRight, GroundPlaneUp, -GroundPlaneForward );
            var gpToWorld = new Matrix3f( -GroundPlaneForward, GroundPlaneRight, GroundPlaneUp );
            var worldToGP = gpToWorld.Inverse();

            // camera up vector in local coordinates
            var cameraUpGP = worldToGP * cameraUp;

            // compute theta
            var cameraUpGPSpherical = GeometryUtils.RectangularToSpherical( cameraUpGP );

            Theta = cameraUpGPSpherical.y;
            // Phi = MathUtils.PI - cameraUpGPSpherical.z;
            Phi = 0;
        }
#endif

        public void HandleKeyDown( object sender, System.Windows.Forms.KeyEventArgs e )
        {
            switch( e.KeyCode )
            {
                case Keys.W:
                {
                    ApplyTranslation( 0, 0, -KeyboardParameters.TranslationPerKeyPress );
                    break;
                }
                case Keys.A:
                {
                    ApplyTranslation( -KeyboardParameters.TranslationPerKeyPress, 0, 0 );                    
                    break;
                }
                case Keys.S:
                {
                    ApplyTranslation( 0, 0, KeyboardParameters.TranslationPerKeyPress );
                    break;
                }
                case Keys.D:
                {
                    ApplyTranslation( KeyboardParameters.TranslationPerKeyPress, 0, 0 );
                    break;
                }                
                case Keys.R:
                {
                    ApplyTranslation( 0, KeyboardParameters.TranslationPerKeyPress, 0 );
                    break;
                }
                case Keys.F:
                {
                    ApplyTranslation( 0, -KeyboardParameters.TranslationPerKeyPress, 0 );
                    break;
                }
            }
        }

#if false
        public void HandleKeyPress( object sender, System.Windows.Forms.KeyPressEventArgs e )
        {
            switch( e.KeyChar )
            {
                case 'w':
                {
                    Camera.Position += KeyboardParameters.TranslationPerKeyPress * ProjectedForward;
                    break;
                }
                case 's':
                {
                    Camera.Position -= KeyboardParameters.TranslationPerKeyPress * ProjectedForward;
                    break;
                }
                case 'd':
                {
                    Camera.Position += KeyboardParameters.TranslationPerKeyPress * ProjectedRight;
                    break;
                }
                case 'a':
                {
                    Camera.Position -= KeyboardParameters.TranslationPerKeyPress * ProjectedRight;
                    break;
                }
            }
        }
#endif

        public void HandleMouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            mouseIsDown = true;
            mouseDownXY = new Vector2i( e.X, e.Y );
        }

        public void HandleMouseMove( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            if( mouseIsDown )
            {
                var xy = new Vector2i( e.X, e.Y );
                var delta = xy - mouseDownXY;
                
                if( Camera != null )
                {
                    if( e.Button == MouseButtons.Left )
                    {
                        ComputeMouseRotation( e, delta );
                    }
                    else if( e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle )
                    {
                        ComputeMouseTranslation( e, delta );
                    }
                }

                mouseDownXY = xy;
            }

            if( Camera != null )
            {
                //Console.WriteLine( "mouse moved, pvw = \n" + Camera.ProjectionViewMatrix );
            }
        }

        private void ComputeMouseRotation( System.Windows.Forms.MouseEventArgs e, Vector2f deltaXY )
        {
            float yaw = deltaXY.x * MouseParameters.RadiansPerPixel;
            float pitch = -deltaXY.y * MouseParameters.RadiansPerPixel;

            if( MouseParameters.InvertX )
            {
                yaw = -yaw;
            }
            if( MouseParameters.InvertY )
            {
                pitch = -pitch;
            }

            ApplyRotation( yaw, pitch );
        }

        private void ComputeMouseTranslation( System.Windows.Forms.MouseEventArgs e, Vector2f deltaXY )
        {
            if( e.Button == MouseButtons.Right )
            {
                float moveX = deltaXY.x * MouseParameters.TranslationPerPixel.x;
                float moveY = -deltaXY.y * MouseParameters.TranslationPerPixel.y;

                if( MouseParameters.InvertX )
                {
                    moveX = -moveX;
                }
                if( MouseParameters.InvertY )
                {
                    moveY = -moveY;
                }
                                
                ApplyTranslation( moveX, 0, moveY );
            }
            else if( e.Button == MouseButtons.Middle )
            {
                float moveY = -deltaXY.y * MouseParameters.TranslationPerPixel.z;
                                
                ApplyTranslation( 0, moveY, 0 );
            }            
        }

        public void HandleMouseUp( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            mouseIsDown = false;            
        }

        public void HandleMouseWheel( object sender, System.Windows.Forms.MouseEventArgs e )
        {
            if( Camera != null )
            {
                // delta is positive for wheel up, negative for wheel down
                // so flip it for fov                

                float dFov = MouseParameters.FovRadiansPerMouseWheelDelta * e.Delta;
                float fov = Camera.FieldOfView - dFov;
                fov = Arithmetic.Clamp( fov, Arithmetic.DegreesToRadians( 1 ), Arithmetic.DegreesToRadians( 175 ) );
                Camera.FieldOfView = fov;
            }
        }

#if false
        public void HandleXboxController( Controller xboxController )
        {
            if( xboxController.IsConnected && Camera != null )
            {
                var gamepad = xboxController.GetState().Gamepad;
                ComputeXboxTranslation( gamepad );
                ComputeXboxRotation( gamepad );
                ComputeXboxFoV( gamepad );

                /*
                var vib = new Vibration();

                if( Math.Abs( lx ) > Gamepad.GamepadLeftThumbDeadZone ||
                    Math.Abs( ly ) > Gamepad.GamepadLeftThumbDeadZone )
                {
                    var l = Math.Max( Math.Abs( lx ), Math.Abs( ly ) );
                    vib.LeftMotorSpeed = ( ushort )Math.Max( l, 0 );
                }
                else
                {
                    vib.LeftMotorSpeed = 0;
                }

                if( Math.Abs( rx ) > Gamepad.GamepadRightThumbDeadZone ||
                    Math.Abs( ry ) > Gamepad.GamepadRightThumbDeadZone )
                {
                    var r = Math.Max( Math.Abs( rx ), Math.Abs( ry ) );
                    vib.RightMotorSpeed = ( ushort )Math.Max( r, 0 );
                }
                else
                {                    
                    vib.RightMotorSpeed = 0;                    
                }
                xboxController.SetVibration( vib );
                */
            }
        }

        private void ComputeXboxTranslation( Gamepad gamepad )
        {
            int lx = gamepad.LeftThumbX;
            int ly = gamepad.LeftThumbY;            

            float moveX = 0;
            float moveY = 0;
            float moveZ = 0;

            // left stick: move
            if( Math.Abs( lx ) > Gamepad.GamepadLeftThumbDeadZone )
            {
                moveX = lx * XboxGamepadParameters.TranslationPerTick;
            }
            if( Math.Abs( ly ) > Gamepad.GamepadLeftThumbDeadZone )
            {
                moveZ = ly * XboxGamepadParameters.TranslationPerTick;
            }

            // TODO: make move up/down configurable
            // dpad: up/down
            if( ( gamepad.Buttons & GamepadButtonFlags.DPadUp ) != 0 )
            {
                moveY = 0.01f;
            }
            if( ( gamepad.Buttons & GamepadButtonFlags.DPadDown ) != 0 )
            {
                moveY = -0.01f;
            }

            var translation = moveX * ProjectedRight + moveY * GroundPlaneUp + moveZ * ProjectedForward;
            ApplyTranslation( translation );
        }

        private void ComputeXboxRotation( Gamepad gamepad )
        {
            float dTheta = 0;
            float dPhi = 0;
            int rx = gamepad.RightThumbX;
            int ry = gamepad.RightThumbY;

            // right stick: rotate
            if( Math.Abs( rx ) > Gamepad.GamepadRightThumbDeadZone )
            {
                dTheta = rx * XboxGamepadParameters.YawRadiansPerTick;
            }
            if( Math.Abs( ry ) > Gamepad.GamepadRightThumbDeadZone )
            {
                dPhi = ry * XboxGamepadParameters.PitchRadiansPerTick;
            }

            if( XboxGamepadParameters.InvertX )
            {
                dTheta = -dTheta;
            }
            if( XboxGamepadParameters.InvertY )
            {
                dPhi = -dPhi;
            }

            Theta += dTheta;
            Phi += dPhi;
            
            ApplyRotation( Theta, Phi );
        }
#endif

        private void ComputeXboxFoV( Gamepad gamepad )
        {
            if( Camera == null )
            {
                return;
            }

            byte lt = gamepad.LeftTrigger;
            byte rt = gamepad.RightTrigger;

            // do nothing if both triggers are held down
            if( lt > 0 && rt > 0 )
            {
                return;
            }
            
            float fov = Camera.FieldOfView;            

            // left trigger: zoom out
            if( lt > 0 )
            {
                fov += lt * XboxGamepadParameters.FoVRadiansPerTick;
            }
            // right trigger: zoom in
            else
            {
                fov -= rt * XboxGamepadParameters.FoVRadiansPerTick;  
            }

            fov = fov.Clamp( Arithmetic.DegreesToRadians( 1 ), Arithmetic.DegreesToRadians( 179 ) );
            Camera.FieldOfView = fov;
        }

        private void ApplyTranslation( float dx, float dy, float dz )
        {
            var x = Camera.RightVector;
            var z = -Camera.ViewDirection;
            Camera.Position += dx * x + dy * UpVector + dz * z;
        }

        private void ApplyRotation( float yaw, float pitch )
        {
            /*
            var rot = Matrix4f.RotateAxis( GroundPlaneUp, theta ) * Matrix4f.RotateAxis( GroundPlaneRight, phi );
            var forward = rot.Transform( GroundPlaneForward );
            var up = rot.Transform( GroundPlaneUp );
            
            Camera.SetOrientation( forward, up );
            */

            var worldToCamera = Camera.ViewMatrix.GetSubmatrix3x3( 0, 0 );
            var cameraToWorld = Camera.InverseViewMatrix.GetSubmatrix3x3( 0, 0 );

            // pitch around the local x axis
            var pitchMatrix = Matrix3f.RotateX( pitch );

            var x = Camera.RightVector;
            var y = Camera.UpVector;
            var z = Camera.ViewDirection;

            y = cameraToWorld * pitchMatrix * worldToCamera * y;
            z = cameraToWorld * pitchMatrix * worldToCamera * z;

            // yaw around the world up vector

            var yawMatrix = groundPlaneToWorld * Matrix3f.RotateY( yaw ) * worldToGroundPlane;

            // x = yawMatrix * x;
            y = yawMatrix * y;
            z = yawMatrix * z;

            Camera.SetOrientation( z, y );
        }

        /*
        private Quat4f PitchYawQuaternion( float radiansX, float radiansY )
        {
            var rotX = new Quat4f( Vector3f.Right, radiansY );

            var yAxis = camera.Frame.TransformOf( FlyUpVector );
            var rotY = new Quat4f( yAxis, radiansX );
            
            return rotY * rotX;
        }
        */
    }
}

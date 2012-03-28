using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using libcgt.core;
using libcgt.core.Vecmath;

namespace libcgt.DirectX.D3D10.Trackball
{
    public class TrackballControls
    {
        // sensitivity
        public float RotationSensitivity { get; set; }
        public float TranslationSensitivity { get; set; }
        public float WheelSensitivity { get; set; }

        public Vector3f SceneCenter
        {
            get
            {
                return sceneCenter;
            }
            set
            {
                sceneCenter = value;
                RevolveAroundPoint = value;
            }
        }        

        public float SceneRadius
        {
            get
            {
                return sceneRadius;
            }
            set
            {
                if( value <= 0 )
                {
                    throw new ArgumentException( "Scene radius must be > 0." );
                }
                else
                {
                    sceneRadius = value;                    
                }
            }
        }

        public float FlySpeed
        {
            get
            {
                return FlySpeedCoefficient * SceneRadius;
            }
        }

        public Vector2i ScreenSize { get; set; }

        public OldCamera Camera { get; set; }
        public float ZNearCoefficient { get; set; }
        public float ZClippingCoefficient { get; set; }
        public float FlySpeedCoefficient { get; set; }
        public bool AutoAdjustNearFar { get; set; }

        private Vector3f sceneCenter;
        private float sceneRadius;

        private int prevX;
        private int prevY;

        private Vector3f RevolveAroundPoint;

        public TrackballControls( OldCamera camera, Vector2i screenSize )
            : this( camera, screenSize, true )
        {
            
        }

        public TrackballControls( OldCamera camera, Vector2i screenSize, bool autoAdjustNearFar )
        {
            Camera = camera;
            ScreenSize = screenSize;

            SceneCenter = Vector3f.Zero;
            SceneRadius = 1.0f;

            ZNearCoefficient = 0.005f;
            ZClippingCoefficient = MathUtils.Sqrt( 3.0f );
            FlySpeedCoefficient = 0.01f;
            AutoAdjustNearFar = autoAdjustNearFar;

            RotationSensitivity = 1.0f;
            TranslationSensitivity = 1.0f;
        }

        public void ShowEntireScene()
        {
            FitSphere( SceneCenter, SceneRadius );
        }

        public void SetSceneBoundingBox( Vector3f min, Vector3f max )
        {
            SceneCenter = 0.5f * ( min + max );
            SceneRadius = 0.5f * ( max - min ).Norm();
        }

        // Moves the camera to see the scene
        public void FitSphere( Vector3f sceneCenter, float sceneRadius )
        {            
            var yView = sceneRadius / MathUtils.Sin( 0.5f * Camera.FieldOfView );
            var xView = sceneRadius / MathUtils.Sin( 0.5f * Camera.HorizontalFieldOfView );
            float distance = Math.Max( xView, yView );

            Camera.Position = new Vector3f( sceneCenter - distance * Camera.ViewDirection );
            UpdateCameraNearFar();
        }

        // Moves the camera to see the scene
        public void FitBoundingBox( Vector3f min, Vector3f max )
        {
            var sceneCenter = 0.5f * ( min + max );
            var sceneRadius = 0.5f * ( max - min ).Norm();
            FitSphere( sceneCenter, sceneRadius );
        }
        
        public void SetFieldOfViewToFitScene( Vector3f sceneCenter, float sceneRadius )
        {
            if( DistanceToSceneCenter() > MathUtils.Sqrt( 2.0f ) * sceneRadius )
            {
                Camera.FieldOfView = 2.0f * MathUtils.Asin( sceneRadius / DistanceToSceneCenter() );
            }
            else
            {
                Camera.FieldOfView = MathUtils.HALF_PI;
            }
        }

        public void OnMouseDown( MouseEventArgs e )
        {            
            prevX = e.X;
            prevY = e.Y;
        }
        
        public void OnMouseMove( MouseEventArgs e )
        {
            int screenX = e.X;
            int screenY = e.Y;
            int dx = prevX - screenX;
            int dy = prevY - screenY;

            if( e.Button == MouseButtons.Left )
            {
                var trans = Camera.Project( RevolveAroundPoint ).XY;
                var q = DeformedBallQuaternion( screenX, screenY, trans );

                Camera.Frame.RotateAroundPoint( q, RevolveAroundPoint );
                UpdateCameraNearFar();
            }
            else if( e.Button == MouseButtons.Right )
            {                
                // scale to fit the screen mouse displacement
                float fovTerm = 2.0f * MathUtils.Tan( 0.5f * Camera.FieldOfView );
                float scale = Math.Abs( ( Camera.Frame.CoordinatesOf( RevolveAroundPoint ) ).z ) / ScreenSize.y;
                var localTranslation = fovTerm * scale * new Vector3f( dx, -dy, 0.0f );
                var globalTranslation = Camera.Frame.InverseTransformOf( TranslationSensitivity * localTranslation );
                
                Camera.Frame.Translate( globalTranslation );
                // RevolveAroundPoint += globalTranslation;

                UpdateCameraNearFar();
            }
            else if( e.Button == MouseButtons.Middle )
            {
                Camera.Frame.Translate( Camera.Frame.InverseTransformOf( new Vector3f( 0, 0, -FlySpeed * dy ) ) );
                UpdateCameraNearFar();
            }

            prevX = screenX;
            prevY = screenY;
        }

        public void OnMouseUp( MouseEventArgs e )
        {
            prevX = -1;
            prevY = -1;
        }

        private float ComputeZNear()
        {
            float z = DistanceToSceneCenter() - ZClippingCoefficient * SceneRadius;

            // Prevents negative or null zNear values.
            float zMin = ZNearCoefficient * ZClippingCoefficient * SceneRadius;
            if( z < zMin )
            {
                z = zMin;
            }

            return z;
        }

        private float ComputeZFar()
        {
            return DistanceToSceneCenter() + ZClippingCoefficient * SceneRadius;
        }

        private float DistanceToSceneCenter()
        {
            return Math.Abs( Camera.Orientation.InverseRotate( SceneCenter - Camera.Position ).z );
        }

        private void UpdateCameraNearFar()
        {
            if( AutoAdjustNearFar )
            {
                Camera.ZNear = ComputeZNear();
                Camera.ZFar = ComputeZFar();
            }
        }

        private Quat4f DeformedBallQuaternion( int x, int y, Vector2f center )
        {
            // Points on the deformed ball
            float px = RotationSensitivity * ( prevX - center.x ) / ScreenSize.x;
            float py = RotationSensitivity * ( center.y - prevY ) / ScreenSize.y;
            float dx = RotationSensitivity * ( x - center.x ) / ScreenSize.x;
            float dy = RotationSensitivity * ( center.y - y ) / ScreenSize.y;

            var p1 = new Vector3f( px, py, ProjectOnBall( px, py ) );
            var p2 = new Vector3f( dx, dy, ProjectOnBall( dx, dy ) );
  
            // Approximation of rotation angle
            // Should be divided by the projectOnBall size, but it is 1.0
            var axis = Vector3f.Cross( p2, p1 );
            float angle = 2.0f * MathUtils.Asin( MathUtils.Sqrt( axis.NormSquared() / p1.NormSquared() / p2.NormSquared() ) );
            return new Quat4f( axis, angle );
        }

        private static float ProjectOnBall( float x, float y )
        {
            const float size = 1.0f;
            const float size2 = size * size;
            const float sizeLimit = size2 * 0.5f;

            float d = x * x + y * y;
            if( d < sizeLimit )
            {
                return MathUtils.Sqrt( size2 - d );
            }
            else
            {
                return sizeLimit / MathUtils.Sqrt( d );
            }
        }
    }
}

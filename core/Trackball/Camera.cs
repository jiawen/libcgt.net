using System;
using libcgt.core.Vecmath;

namespace libcgt.core.Trackball
{
    public enum CameraType
    {
        PERSPECTIVE,
        ORTHOGRAPHIC
    }

    public class Camera
    {
        private CameraType cameraType;
        private ManipulatedCameraFrame frame;
        private float orthoCoeff;        

        // camera parameters

        private Vector3f sceneCenter; // in world coordinates
        private float sceneRadius; // in world units
        private Vector2i screenSize;

        // stored matrices
        private Matrix4f projectionMatrix;
        private Matrix4f inverseProjectionMatrix;
        private Matrix4f viewMatrix;
        private Matrix4f inverseViewMatrix;

        public Camera()            
        {
            FieldOfView = ( float ) ( Math.PI / 4.0 );

            // interpolationKeyFrames = new KeyFrameInterpolator
            Frame = new ManipulatedCameraFrame();

            SceneRadius = 1.0f;
            orthoCoeff = ( float ) ( Math.Tan( FieldOfView / 2.0 ) );
            SceneCenter = Vector3f.Zero;

            CameraType = CameraType.PERSPECTIVE;

            ZNearCoefficient = 0.005f;
            ZClippingCoefficient = ( float ) ( Math.Sqrt( 3.0 ) );

            // dummy values
            screenSize = new Vector2i( 400, 400 );

            viewMatrix = Matrix4f.Identity;
            projectionMatrix = Matrix4f.Zero;

            ComputeProjectionMatrix();
        }

        // TODO: get rid of ComputeProjectionMatrix()

        public Camera( Vector3f position, Vector3f forward, Vector3f up,
            float fovYDegrees,
            int screenWidth, int screenHeight )
        {
            FieldOfView = Arithmetic.DegreesToRadians( fovYDegrees );

            // interpolationKeyFrames = new KeyFrameInterpolator
            Frame = new ManipulatedCameraFrame();
            
            SceneRadius = 1.0f;
            orthoCoeff = ( float ) ( Math.Tan( FieldOfView / 2.0 ) );
            SceneCenter = Vector3f.Zero;

            CameraType = CameraType.PERSPECTIVE;

            ZNearCoefficient = 0.005f;
            ZClippingCoefficient = ( float ) ( Math.Sqrt( 3.0 ) );

            // dummy values
            screenSize = new Vector2i( screenWidth, screenHeight );

            Position = position;
            UpVector = up;
            ViewDirection = forward;

            ComputeViewMatrix();
            ComputeProjectionMatrix();
        }

        public Camera( Camera camera )
        {
            // #CONNECTION# Camera constructor
            // interpolationKfi_ = new KeyFrameInterpolator;
            
            // Requires the interpolationKfi_
            Frame = new ManipulatedCameraFrame();

            viewMatrix = Matrix4f.Identity;
            projectionMatrix = Matrix4f.Zero;

            Set( camera );
        }

        /// <summary>
        /// Field of view, in radians
        /// </summary>
        public float FieldOfView { get; set; }        

        /// <summary>
        /// Returns the coefficient which is used to set zNear() when the Camera is inside the sphere
        /// defined by sceneCenter() and zClippingCoefficient() * sceneRadius().
        /// 
        /// In that case, the zNear() value is set to zNearCoefficient() * zClippingCoefficient() *
        /// sceneRadius(). See the zNear() documentation for details.
        /// 
        /// Default value is 0.005, which is appropriate for most applications. In case you need a high
        /// dynamic ZBuffer precision, you can increase this value (~0.1). A lower value will prevent
        /// clipping of very close objects at the expense of a worst Z precision.
        /// 
        /// Only meaningful when Camera type is Camera::PERSPECTIVE. */
        /// </summary>
        public float ZNearCoefficient { get; set; }

        /// <summary>
        /// Returns the coefficient used to position the near and far clipping planes.
        /// 
        /// The near (resp. far) clipping plane is positioned at a distance equal to zClippingCoefficient() *
        /// sceneRadius() in front of (resp. behind) the sceneCenter(). This garantees an optimal use of
        /// the z-buffer range and minimizes aliasing. See the zNear() and zFar() documentations.
        /// 
        /// Default value is square root of 3.0 (so that a cube of size sceneRadius() is not clipped).
        /// 
        /// However, since the sceneRadius() is used for other purposes (see showEntireScene(), flySpeed(),
        /// ...) and you may want to change this value to define more precisely the location of the clipping
        /// planes. See also zNearCoefficient().
        /// 
        /// For a total control on clipping planes' positions, an other option is to overload the zNear()
        /// and zFar() methods. See the <a href="../examples/OldCamera.html">OldCamera example</a>.
        /// 
        /// \attention When QGLViewer::cameraPathAreEdited(), this value is set to 5.0 so that the Camera
        /// paths are not clipped. The previous zClippingCoefficient() value is restored back when you leave
        /// this mode.
        /// </summary>
        public float ZClippingCoefficient { get; set; }

        public ManipulatedCameraFrame Frame
        {
            get
            {
                return frame;
            }
            set
            {
                if( value == null )
                {
                    throw new ArgumentException( "Manipulated camera frame cannot be null." );
                }

                frame = value;
                // TODO:
                // interpolationKfi_->setFrame(frame());
            }
        }

        public Vector3f Position
        {
            get
            {
                return Frame.Position;
            }
            set
            {
                Frame.Position = value;
            }
        }

        public Vector3f ViewDirection
        {
            get
            {
                return Frame.InverseTransformOf( new Vector3f( 0, 0, -1 ) );
            }
            set
            {
                if( value.NormSquared() < 1e-10 )
                {
                    return;
                }
                
                var xAxis = Vector3f.Cross( value, UpVector );
                if( xAxis.NormSquared() < 1e-10 )
                {
                    // target is aligned with upVector, this means a rotation around X axis
                    // X axis is then unchanged, let's keep it !
                    xAxis = Frame.InverseTransformOf( new Vector3f( 1, 0, 0 ) );
                }

                var q = Quat4f.FromRotatedBasis( xAxis, Vector3f.Cross( xAxis, value ), -value );
                
                // TODO: should be constrained: Frame.SetOrientationWithConstraint( q );
                Frame.Orientation = q;
            }
        }

        public Vector3f UpVector
        {
            get
            {
                return Frame.InverseTransformOf( Vector3f.Up );
            }
            set
            {
                SetUpVector( value, true );
            }
        }

        public Vector3f RightVector
        {
            get
            {
                return Frame.InverseTransformOf( new Vector3f( 1, 0, 0 ) );
            }
        }

        public Quat4f Orientation
        {
            get
            {
                return Frame.Orientation;
            }
            set
            {
                Frame.Orientation = value;
                Frame.UpdateFlyUpVector();
            }
        }

        /// <summary>
        /// Sets the orientation() of the Camera using polar coordinates.
        /// \p theta rotates the Camera around its Y axis, and \e then \p phi rotates it around its X axis.
        /// The polar coordinates are defined in the world coordinates system: \p theta = \p phi = 0 means
        /// that the Camera is directed towards the world Z axis. Both angles are expressed in radians.
        /// 
        /// See also setUpVector(). The position() of the Camera is unchanged, you may want to call showEntireScene()
        /// after this method to move the Camera.
        /// 
        /// This method can be useful to create Quicktime VR panoramic sequences, see the
        /// QGLViewer::saveSnapshot() documentation for details. */
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="phi"></param>
        public void SetOrientation( float theta, float phi )
        {
            var axis = new Vector3f( 0, 1, 0 );
            var rot1 = new Quat4f( axis, theta );
            axis = new Vector3f( ( float )( -Math.Cos( theta ) ), 0, ( float )( Math.Sin( theta ) ) );
            var rot2 = new Quat4f( axis, phi );
            Orientation = rot1 * rot2;
        }

        /// <summary>
        /// Rotates the Camera so that its upVector() becomes \p up (defined in the world coordinate system).
        /// The Camera is rotated around an axis orthogonal to \p up and to the current upVector() direction.
        /// Use this method in order to define the Camera horizontal plane.
        /// 
        /// When \p noMove is set to \c false, the orientation modification is compensated by a translation, so
        /// that the revolveAroundPoint() stays projected at the same position on screen. This is especially
        /// useful when the Camera is an observer of the scene (default mouse binding).
        /// 
        /// When \p noMove is \c true (default), the Camera position() is left unchanged, which is an intuitive
        /// behavior when the Camera is in a walkthrough fly mode (see the QGLViewer::MOVE_FORWARD and
        /// QGLViewer::MOVE_BACKWARD QGLViewer::MouseAction).
        /// </summary>
        /// <param name="up"></param>
        /// <param name="noMove"></param>
        public void SetUpVector( Vector3f up, bool noMove )
        {
            var q = new Quat4f( new Vector3f( 0, 1, 0 ), Frame.TransformOf( up ) );

            if( !noMove )
            {
                Frame.Position = RevolveAroundPoint - ( Frame.Orientation * q ).Rotate( Frame.CoordinatesOf( RevolveAroundPoint ) );
            }

            Frame.Rotate( q );  

            // Useful in fly mode to keep the horizontal direction.
            Frame.UpdateFlyUpVector();
        }

        /* TODO
         * void setFromModelViewMatrix(const GLdouble* const modelViewMatrix);
    void setFromProjectionMatrix(const float matrix[12]);
         * */

        public float HorizontalFieldOfView
        {
            get
            {
                return ( float ) ( 2.0 * Math.Atan( Math.Tan( 0.5 * FieldOfView ) * AspectRatio ) );
            }
            set
            {
                FieldOfView = ( float ) ( 2.0 * Math.Atan( Math.Tan( 0.5 * value ) / AspectRatio ) );
            }
        }

        public CameraType CameraType
        {
            get
            {
                return cameraType;
            }
            set
            {
                // make ORTHOGRAPHIC frustum fit PERSPECTIVE (at least in plane normal to viewDirection(), passing
                // through RAP) Done only when CHANGING type since orthoCoef_ may have been changed with a
                // setRevolveAroundPoint() in the meantime.
                if( ( value == CameraType.ORTHOGRAPHIC ) && ( cameraType == CameraType.PERSPECTIVE ) )
                {
                    orthoCoeff = ( float ) Math.Tan( 0.5 * FieldOfView );
                }
                cameraType = value;
            }
        }

        public virtual float ZNear
        {
            get
            {
                float z = DistanceToSceneCenter - ZClippingCoefficient * SceneRadius;

                // Prevents negative or null zNear values.
                float zMin = ZNearCoefficient * ZClippingCoefficient * SceneRadius;
                if( z < zMin )
                {
                    switch( CameraType )
                    {
                        case CameraType.PERSPECTIVE:
                            z = zMin;
                            break;

                        case CameraType.ORTHOGRAPHIC:
                            // HACK
                            // z = 0.0f;
                            z = -1000.0f;
                            break;
                    }
                }

                return z;
            }
            set
            {
                throw new NotSupportedException( "Cannot set zNear for default camera" );
            }
        }

        public virtual float ZFar
        {
            get
            {
                return DistanceToSceneCenter + ZClippingCoefficient * SceneRadius;
            }
            set
            {
                throw new NotSupportedException( "Cannot set zNear for default camera" );
            }
        }

        public Rect2f Viewport
        {
            get
            {                
                return new Rect2f( 0, 0, screenSize.x, screenSize.y );

                // TODO: QGLViewer uses this weird style of viewport:
                // This method is mainly used in conjunction with \c gluProject, which requires such a viewport.
                // Returned values are (0, screenHeight(), screenWidth(), - screenHeight()), so that the origin is
                // located in the \e upper left corner of the window (Qt style coordinate system). */
                // viewport[0] = 0;
                // viewport[1] = screenHeight();
                // viewport[2] = screenWidth();
                // viewport[3] = -screenHeight();
            }
        }

        public float AspectRatio
        {
            get
            {
                return Arithmetic.DivideIntsToFloat( screenSize.x, screenSize.y );
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
                    Frame.FlySpeed = 0.01f * SceneRadius;
                }
            }
        }

        public Vector3f SceneCenter
        {
            get
            {
                return sceneCenter;
            }
            set
            {
                sceneCenter = value;
                RevolveAroundPoint = sceneCenter;
            }
        }

        public Vector3f RevolveAroundPoint
        {
            get
            {
                return Frame.RevolveAroundPoint;
            }
            set
            {
                float prevDist = ( Math.Abs( CameraCoordinatesOf( RevolveAroundPoint ).z ) );
                Frame.RevolveAroundPoint = value;

                // orthoCoef_ is used to compensate for changes of the revolveAroundPoint, so that the image does
                // not change when the revolveAroundPoint is changed in ORTHOGRAPHIC mode.
                float newDist = Math.Abs( CameraCoordinatesOf( RevolveAroundPoint ).z );

                // Prevents division by zero when rap is set to camera position
                if( ( prevDist > 1e-9 ) && ( newDist > 1e-9 ) )
                {
                    orthoCoeff *= prevDist / newDist;
                }
            }
        }

        public Vector2i ScreenSize
        {
            get
            {
                return screenSize;
            }
            set
            {
                screenSize.x = ( value.x > 0 ) ? value.x : 1;
                screenSize.y = ( value.y > 0 ) ? value.y : 1;
            }
        }

        public float DistanceToSceneCenter
        {
            get
            {
                return Math.Abs( ( Frame.CoordinatesOf( SceneCenter ) ).z );
            }
        }

        public Matrix4f ProjectionMatrix
        {
            get
            {
                ComputeProjectionMatrix();
                return projectionMatrix;
            }
        }

        public Matrix4f InverseProjectionMatrix
        {
            get
            {
                ComputeProjectionMatrix();
                return inverseProjectionMatrix;
            }
        }

        public Matrix4f ViewMatrix
        {
            get
            {
                ComputeViewMatrix();
                return viewMatrix;
            }
        }

        public Matrix4f InverseViewMatrix
        {
            get
            {
                ComputeViewMatrix();
                return inverseViewMatrix;
            }
        }

        public Matrix4f ProjectionViewMatrix
        {
            get
            {
                return ProjectionMatrix * ViewMatrix;
            }
        }        

        public void SetSceneBoundingBox( Vector3f min, Vector3f max )
        {
            SceneCenter = 0.5f * ( min + max );
            sceneRadius = 0.5f * ( max - min ).Norm();
        }

        public void SetFOVToFitScene()
        {
            if( DistanceToSceneCenter > Math.Sqrt( 2.0 ) * SceneRadius )
            {
                FieldOfView = ( float ) ( 2.0 * Math.Asin( SceneRadius / DistanceToSceneCenter ) );
            }
            else
            {
                FieldOfView = ( float ) ( Math.PI / 2.0 );
            }
        }

        /// <summary>
        /// World to camera
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f CameraCoordinatesOf( Vector3f src )
        {
            return Frame.CoordinatesOf( src );
        }

        /// <summary>
        /// Camera to world
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f WorldCoordinatesOf( Vector3f src )
        {
            return frame.InverseCoordinatesOf( src );
        }

        /// <summary>
        /// World to screen (pixel)
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f ProjectedCoordinatesOf( Vector3f src )
        {
            return ProjectedCoordinatesOf( src, null );
        }

        /// <summary>
        /// World to screen (pixel)
        /// </summary>
        /// <param name="src"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Vector3f ProjectedCoordinatesOf( Vector3f src, Frame frame )
        {
            Vector3f tmp;
            if( frame != null )
            {
                tmp = frame.InverseCoordinatesOf( src );
            }
            else
            {
                tmp = src;
            }

            Vector4f clip = ProjectionViewMatrix * new Vector4f( tmp, 1.0f );
            Vector3f ndc = clip.Homogenized();
            float sx = ScreenSize.x * 0.5f * ( ndc.x + 1.0f );
            float sy = ScreenSize.y * 0.5f * ( ndc.y + 1.0f );
            // OpenGL
            // float sz = 0.5f * ( ndc.Z + 1.0f );
            // D3D
            float sz = ndc.z;
            // float w = 1;

            return new Vector3f( sx, sy, sz );
        }        

        /// <summary>
        /// Screen (pixel) to world
        /// </summary>
        /// <param name="src"></param>        
        /// <returns></returns>
        public Vector3f UnprojectedCoordinatesOf( Vector3f src )
        {
            return UnprojectedCoordinatesOf( src, null );
        }

        /// <summary>
        /// Screen (pixel) to world
        /// </summary>
        /// <param name="src"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Vector3f UnprojectedCoordinatesOf( Vector3f src, Frame frame )
        {
            float x = 2 * src.x / ScreenSize.x - 1;
            float y = 2 * src.y / ScreenSize.y - 1;
            float z = src.z;
           
            var clip = new Vector4f( x, y, z, 1 );
            var eye = InverseProjectionMatrix * clip;
            var world = ( InverseViewMatrix * eye ).Homogenized();

            if( frame != null )
            {
                return Frame.CoordinatesOf( world );
            }
            else
            {
                return world;
            }
        }        

        /// <summary>
        /// Converts a pixel into a ray for object intersection.
        /// pixel (0,0) is the top left corner
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        public void ConvertClickToLine( Vector2i pixel, out Vector3f origin, out Vector3f direction )
        {
            origin = Vector3f.Zero;
            direction = Vector3f.Zero;

            switch( CameraType )
            {
                case CameraType.PERSPECTIVE:

                    origin = Position;
                    direction = new Vector3f
                        (
                        ( float )
                        ( ( ( 2.0 * pixel.x / ScreenSize.x ) - 1.0 ) * Math.Tan( 0.5 * FieldOfView ) * AspectRatio ),
                        ( float )
                        ( ( ( 2.0 * ( ScreenSize.y - pixel.y ) / ScreenSize.y ) - 1.0 ) * Math.Tan( 0.5 * FieldOfView ) ),
                        -1.0f
                        );
                    direction = WorldCoordinatesOf( direction ) - origin;
                    direction.Normalize();
                    break;

                case CameraType.ORTHOGRAPHIC:

                    Vector2f wh = GetOrthoWidthHeight();
                    origin = new Vector3f
                        (
                        ( float ) ( ( 2.0 * pixel.x / ScreenSize.x - 1.0 ) * wh.x ),
                        ( float ) ( -( 2.0 * pixel.y / ScreenSize.y - 1.0 ) * wh.y ),
                        0.0f
                        );
                    origin = WorldCoordinatesOf( origin );
                    direction = ViewDirection;
                    break;
            }
        }

        /// <summary>
        /// Returns the \p halfWidth and \p halfHeight of the Camera orthographic frustum.
        /// 
        /// These values are only valid and used when the Camera is of type() Camera::ORTHOGRAPHIC. They are
        /// expressed in OpenGL units and are used by loadProjectionMatrix() to define the projection matrix
        /// using:
        /// \code
        /// glOrtho( -halfWidth, halfWidth, -halfHeight, halfHeight, zNear(), zFar() )
        /// \endcode
        /// 
        /// These values are proportional to the Camera (z projected) distance to the revolveAroundPoint().
        /// When zooming on the object, the Camera is translated forward \e and its frustum is narrowed, making
        /// the object appear bigger on screen, as intuitively expected.
        /// 
        /// Overload this method to change this behavior if desired, as is done in the 
        /// <a href="../examples/OldCamera.html">OldCamera example</a>. */
        /// </summary>
        /// <returns></returns>
        public virtual Vector2f GetOrthoWidthHeight()
        {
            float dist = orthoCoeff * Math.Abs( CameraCoordinatesOf( RevolveAroundPoint ).z );
            float halfWidth = dist * ( ( AspectRatio < 1.0f ) ? 1.0f : AspectRatio );
            float halfHeight = dist * ( ( AspectRatio < 1.0f ) ? 1.0f / AspectRatio : 1.0f );

            return new Vector2f( halfWidth, halfHeight );
        }

        public void ComputeProjectionMatrix()
        {
            switch( CameraType )
            {
                case CameraType.PERSPECTIVE:
                    {                        
                        projectionMatrix = Matrix4f.PerspectiveD3D( FieldOfView, AspectRatio, ZNear, ZFar );

                        break;
                    }
                case CameraType.ORTHOGRAPHIC:
                    {
                        Vector2f wh = GetOrthoWidthHeight();
                        float w = wh.x;
                        float h = wh.y;
                        projectionMatrix = Matrix4f.OrthoD3D( w, h, ZNear, ZFar );

                        break;
                    }
            }

            inverseProjectionMatrix = projectionMatrix.Inverse();
        }

        public void ComputeViewMatrix()
        {
            var q = Frame.Orientation;

            float q00 = 2.0f * q[ 0 ] * q[ 0 ];
            float q11 = 2.0f * q[ 1 ] * q[ 1 ];
            float q22 = 2.0f * q[ 2 ] * q[ 2 ];

            float q01 = 2.0f * q[ 0 ] * q[ 1 ];
            float q02 = 2.0f * q[ 0 ] * q[ 2 ];
            float q03 = 2.0f * q[ 0 ] * q[ 3 ];

            float q12 = 2.0f * q[ 1 ] * q[ 2 ];
            float q13 = 2.0f * q[ 1 ] * q[ 3 ];

            float q23 = 2.0f * q[ 2 ] * q[ 3 ];

            viewMatrix[ 0, 0 ] = 1.0f - q11 - q22;
            viewMatrix[ 1, 0 ] = q01 - q23;
            viewMatrix[ 2, 0 ] = q02 + q13;
            viewMatrix[ 3, 0 ] = 0.0f;

            viewMatrix[ 0, 1 ] = q01 + q23;
            viewMatrix[ 1, 1 ] = 1.0f - q22 - q00;
            viewMatrix[ 2, 1 ] = q12 - q03;
            viewMatrix[ 3, 1 ] = 0.0f;

            viewMatrix[ 0, 2 ] = q02 - q13;
            viewMatrix[ 1, 2 ] = q12 + q03;
            viewMatrix[ 2, 2 ] = 1.0f - q11 - q00;
            viewMatrix[ 3, 2 ] = 0.0f;

            var t = q.InverseRotate( Frame.Position );

            viewMatrix[ 0, 3 ] = -t.x;
            viewMatrix[ 1, 3 ] = -t.y;
            viewMatrix[ 2, 3 ] = -t.z;
            viewMatrix[ 3, 3 ] = 1.0f;

            inverseViewMatrix = viewMatrix.Inverse();
        }

        public void ShowEntireScene()
        {
            FitSphere( SceneCenter, SceneRadius );
        }

        public void FitSphere( Vector3f center, float radius )
        {
            float distance = 0.0f;

            switch( CameraType )
            {
                case CameraType.PERSPECTIVE:
                    {
                        var yView = ( float ) ( radius / Math.Sin( 0.5 * FieldOfView ) );
                        var xView = ( float ) ( radius / Math.Sin( 0.5 * HorizontalFieldOfView ) );
                        distance = Math.Max( xView, yView );
                        break;
                    }

                case CameraType.ORTHOGRAPHIC:
                    {
                        distance = Vector3f.Dot( center - RevolveAroundPoint, ViewDirection ) + ( radius / orthoCoeff );
                        break;
                    }
            }

            var newPosition = new Vector3f( center - distance * ViewDirection );

            Frame.Position = newPosition;

            // TODO:
            // frame()->setPositionWithConstraint(newPos);
        }

        // TODO: Rect2i
        // public void FitScreeRegion( 

        //////////////////////////////////////////// jiawen ////////////////////////////////////////////
        
        public void Move( Vector3f xyz )
        {
            Move( xyz.x, xyz.y, xyz.z );
        }

        public void Move( float x, float y, float z )
        {
            Position += x * RightVector + y * UpVector - z * ViewDirection;

            ComputeViewMatrix();
        }

        /// <summary>
        /// Screen (pixel) to world
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Vector3f UnprojectedCoordinatesOf( Vector2f xy, float z )
        {
            return UnprojectedCoordinatesOf( new Vector3f( xy, z ), null );
        }

        public virtual void Set( Camera camera )
        {
            screenSize = camera.ScreenSize;
            FieldOfView = camera.FieldOfView;
            SceneRadius = camera.SceneRadius;
            SceneCenter = camera.SceneCenter;
            ZNearCoefficient = camera.ZNearCoefficient;
            ZClippingCoefficient = camera.ZClippingCoefficient;
            CameraType = camera.CameraType;

            orthoCoeff = camera.orthoCoeff;

            // frame_ and interpolationKfi_ pointers are not shared.
            Frame.ReferenceFrame = null;
            Frame.Position = camera.Position;
            Frame.Orientation = camera.Orientation;
            //interpolationKfi_->resetInterpolation();

            // kfi_ = camera.kfi_;
            
            ComputeProjectionMatrix();
            ComputeViewMatrix();
        }

        // TODO: unify with ConvertClickToLine
        // TODO: doesn't do ortho camera
        public Vector3f PixelToRay( Vector2f screenXY )
        {
            var pointOnNearPlane = UnprojectedCoordinatesOf( new Vector3f( screenXY, 0 ) );
            return ( pointOnNearPlane - Position ).Normalized();
        }

        public Matrix4f ScreenToWorld()
        {
            var inverseViewport = new Matrix4f
            (
                2.0f / ScreenSize.x, 0, 0, -1,
                0, 2.0f / ScreenSize.y, 0, -1,
                0, 0, 1, 0,
                0, 0, 0, 1
            );

            return InverseViewMatrix * InverseProjectionMatrix * inverseViewport;
        }

        public void GetFrustum( out Vector3f topLeft, out Vector3f topRight,
            out Vector3f bottomLeft, out Vector3f bottomRight )
        {
            float fTop = ( float ) ( ZNear * Math.Tan( Arithmetic.DegreesToRadians( 0.5f * FieldOfView ) ) );
            float fBottom = -fTop;
            float fRight = AspectRatio * fTop;
            float fLeft = -fRight;

            var worldRight = Vector3f.Cross( ViewDirection, UpVector );
            topLeft = Position + fTop * UpVector + fLeft * worldRight + ZNear * ViewDirection;
            topRight = Position + fTop * UpVector + fRight * worldRight + ZNear * ViewDirection;
            bottomLeft = Position + fBottom * UpVector + fLeft * worldRight + ZNear * ViewDirection;
            bottomRight = Position + fBottom * UpVector + fRight * worldRight + ZNear * ViewDirection;
        }

        public void Pitch( float radians )
        {
            var m = Matrix4f.RotateX( radians );

            var rotatedUpCamera = ( m * Vector4f.UpH ).XYZ;
            var rotatedForwardCamera = ( m * Vector4f.ForwardH ).XYZ;

            var upWorld = WorldCoordinatesOf( rotatedUpCamera );
            var forwardWorld = WorldCoordinatesOf( rotatedForwardCamera );

            UpVector = upWorld - Position;
            ViewDirection = forwardWorld - Position;

            ComputeViewMatrix();
        }

        public void Yaw( float radians )
        {
            var m = Matrix4f.RotateY( radians );

            var rotatedRightCamera = ( m * Vector4f.RightH ).XYZ;
            var rotatedForwardCamera = ( m * Vector4f.ForwardH ).XYZ;

            var rightWorld = WorldCoordinatesOf( rotatedRightCamera );
            var forwardWorld = WorldCoordinatesOf( rotatedForwardCamera );

            var upWorld = Vector3f.Cross( rightWorld - Position, forwardWorld - Position );

            UpVector = upWorld;
            ViewDirection = forwardWorld - Position;

            ComputeViewMatrix();
        }

        public void GetJittered( Vector2f dxdy, float zFocus,
            out Matrix4f jitteredView, out Matrix4f jitteredProjection )
        {
            // var camera = new Camera( this );
            var newEye = Position + dxdy.x * RightVector + dxdy.y * UpVector;
            jitteredView = Matrix4f.LookAt( newEye, newEye + ViewDirection, UpVector );

            // compute new projection matrix
            float halfThetaY = 0.5f * FieldOfView;
            float halfThetaX = 0.5f * HorizontalFieldOfView;

            float topFocus = zFocus * MathUtils.Tan( halfThetaY );
            float newTopFocus = topFocus - dxdy.y;
            float newTop = ZNear * newTopFocus / zFocus;

            float bottomFocus = zFocus * MathUtils.Tan( halfThetaY );
            float newBottomFocus = bottomFocus + dxdy.y;
            float newBottom = -ZNear * newBottomFocus / zFocus;

            float leftFocus = zFocus * MathUtils.Tan( halfThetaX );
            float newLeftFocus = leftFocus + dxdy.x;
            float newLeft = -ZNear * newLeftFocus / zFocus;

            float rightFocus = zFocus * MathUtils.Tan( halfThetaX );
            float newRightFocus = rightFocus - dxdy.x;
            float newRight = ZNear * newRightFocus / zFocus;

            jitteredProjection = Matrix4f.PerspectiveOffCenterD3D( newLeft, newRight, newBottom, newTop, ZNear, ZFar );
        }
    }
}
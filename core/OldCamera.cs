using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    // TODO: D3D notes
    // The projection matrix is set UpVector such that
    // once you homogenize the clip space coordinates
    // at zNear, ndc.z = 0, and
    // at zFar, ndc.z = 1
    // i.e. the projection matrix flips the z (clip space is left handed)

    // TODO: general camera class
    // subclass it with perspective, skewed perspective, ortho, and skewed ortho classes
    // that lets you specify a smaller set of parameters

    [Serializable]
    public class OldCamera : IEquatable< OldCamera >
    {
        /// <summary>
        /// Field of view in the y direction, in radians
        /// </summary>
        public float FieldOfView
        {
            get
            {
                return fieldOfView;
            }
            set
            {
                fieldOfView = value;
                projectionMatrixIsDirty = true;
            }
        }

        private float fieldOfView;
        
        /// <summary>
        /// Aspect ratio of the projected image, width / height
        /// </summary>
        public float AspectRatio
        {
            get
            {
                return Arithmetic.DivideIntsToFloat( ScreenSize.x, ScreenSize.y );
            }
        }

        public float ZNear
        {
            get
            {
                return zNear;
            }
            set
            {
                zNear = value;
                projectionMatrixIsDirty = true;
            }
        }

        private float zNear;

        public float ZFar
        {
            get
            {
                return zFar;
            }
            set
            {
                zFar = value;
                projectionMatrixIsDirty = true;
            }
        }

        private float zFar;

        public Vector2i ScreenSize
        {
            get
            {
                return screenSize;
            }
            set
            {
                screenSize = value;
                projectionMatrixIsDirty = true;
                viewportMatrixIsDirty = true;
            }
        }

        private Vector2i screenSize;

        private Matrix4f viewMatrix;
        private Matrix4f inverseViewMatrix;
        private bool viewMatrixIsDirty = true;

        private Matrix4f projectionMatrix;
        private Matrix4f inverseProjectionMatrix;
        private bool projectionMatrixIsDirty = true;

        private Matrix4f viewportMatrix;
        private Matrix4f inverseViewportMatrix;
        private bool viewportMatrixIsDirty = true;

        private Matrix4f projectionViewMatrix;
        private Matrix4f inverseProjectionViewMatrix;

        public AffineFrame Frame { get; set; }

        public OldCamera()
            : this( 400, 400 )
        {
            
        }

        public OldCamera( int screenWidth, int screenHeight )
            : this( new Vector3f( 0, 0, 5 ), Vector3f.Forward, Vector3f.Up,
            Arithmetic.DegreesToRadians( 50 ),
            1, 100,
            screenWidth, screenHeight )
        {
            
        }

        public OldCamera( Vector3f position, Vector3f viewDirection, Vector3f upVector,
                          float fovYRadians, float zNear, float zFar,
                          int screenWidth, int screenHeight ) :
            this( position, Quat4f.FromRotatedBasis( Vector3f.Cross( viewDirection, upVector ), upVector, -viewDirection ),
            fovYRadians, zNear, zFar, screenWidth, screenHeight )
        {
            
        }

        public OldCamera( Vector3f position, Quat4f orientation,
            float fovYRadians, float zNear, float zFar,
            int screenWidth, int screenHeight )
        {
            fieldOfView = fovYRadians;
            this.zNear = zNear;
            this.zFar = zFar;

            screenSize = new Vector2i( screenWidth, screenHeight );

            Frame = new AffineFrame( position, orientation );
        }

        /// <summary>
        /// For loading photosynth datasets
        /// TODO: make another for boujou: focal length and sensor size in meters
        /// </summary>
        /// <param name="view"></param>
        /// <param name="focalLengthPixels"></param>
        /// <param name="imageSizePixels"></param>
        public OldCamera( Matrix4f view, float focalLengthPixels, Vector2i imageSizePixels,
            float zNear, float zFar )
        {
            var iView = view.Inverse();
            var position = ( iView * Vector4f.ZeroH ).XYZ;
            var viewDirection = ( iView * Vector4f.ForwardH ).XYZ - position;
            var upVector = ( iView * Vector4f.UpH ).XYZ - position;

            fieldOfView = 2 * ( float ) ( Math.Atan2( 0.5f * imageSizePixels.y, focalLengthPixels ) );
            this.zNear = zNear;
            this.zFar = zFar;

            screenSize = imageSizePixels;
            
            var q = Quat4f.FromRotatedBasis( Vector3f.Cross( viewDirection, upVector ), upVector, -viewDirection );
            Frame = new AffineFrame( position, q );
        }

        public OldCamera( OldCamera camera )
        {
            Set( camera );
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

        public Quat4f Orientation
        {
            get
            {
                return Frame.Orientation;
            }
            set
            {
                Frame.Orientation = value;
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
                
                Orientation = q;
            }
        }

        public Vector3f UpVector
        {
            get
            {
                return Frame.InverseTransformOf( Vector3f.Up );
            }
        }

        public Vector3f RightVector
        {
            get
            {
                return Frame.InverseTransformOf( new Vector3f( 1, 0, 0 ) );
            }
        }

        public void SetOrientation( Vector3f viewDirection, Vector3f upVector )
        {
            var rightVector = Vector3f.Cross( viewDirection, upVector );
            var q = Quat4f.FromRotatedBasis( rightVector, upVector, -viewDirection );
            Frame.Orientation = q;
        }
        
        public float HorizontalFieldOfView
        {
            get
            {
                return ComputeHorizontalFieldOfView( FieldOfView, screenSize.x, screenSize.y );
            }
            set
            {
                FieldOfView = ComputeVerticalFieldOfView( value, screenSize.x, screenSize.y );
            }
        }

        public void Set( OldCamera camera )
        {
            fieldOfView = camera.FieldOfView;

            zNear = camera.ZNear;
            zFar = camera.ZFar;

            screenSize = camera.ScreenSize;
            Frame = new AffineFrame( camera.Frame );
            
            viewMatrixIsDirty = true;
            projectionMatrixIsDirty = true;
            viewportMatrixIsDirty = true;
        }

        /// <summary>
        /// Projects a point xyz from world space to screen (pixel) space.        
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public Vector4f Project( Vector3f xyz )
        {
            return Project( new Vector4f( xyz, 1 ) );
        }

        /// <summary>
        /// Projects a point xyzw from world space to screen (pixel) space.
        /// y points up.
        /// </summary>
        /// <param name="xyzw"></param>
        /// <returns></returns>
        public Vector4f Project( Vector4f xyzw )
        {
            var position = ViewMatrix * xyzw;
            var clip = ProjectionMatrix * position;
            var ndc = clip.Homogenized();

            float x = ScreenSize.x * 0.5f * ( ndc.x + 1 );
            float y = ScreenSize.y * 0.5f * ( ndc.y + 1 );
            // OpenGL has ndc.Z from -1 to 1
            // but screen z from 0 to 1
            // float z = 0.5f * ( ndc.Z + 1.0f );
            // D3D
            float z = ndc.z;
            float w = 1;

            return new Vector4f( x, y, z, w );
        }

        /// <summary>
        /// Projects a point xyzw from world space to screen space
        /// as a floating point sample location.
        /// y points up.
        /// </summary>
        /// <param name="xyzw"></param>
        /// <returns></returns>
        public Vector2f ProjectToSample( Vector3f xyz )
        {
            var screenXYZW = Project( xyz );
            return screenXYZW.XY;
        }

        /// <summary>
        /// Projects a point xyzw from world space to screen space
        /// as an integer pixel (nearest neighbor)
        /// y points up.
        /// </summary>
        /// <param name="xyzw"></param>
        /// <returns></returns>
        public Vector2i ProjectToPixel( Vector3f xyz )
        {
            var screenXYZW = Project( xyz );
            return screenXYZW.XY.FloorToInt(); // yes, FloorToInt to get the nearest neighbor
        }

        /// <summary>
        /// Unprojects a point from screen space back into world space
        /// screenXY is between [0, width/height]
        /// screenZ is between 0 and 1, with 0 at the near plane and 1 at the far plane        
        /// </summary>
        /// <param name="screenXYZ"></param>
        /// <returns></returns>
        public Vector3f Unproject( Vector4f xyzw )
        {
            float x = 2 * xyzw.x / ScreenSize.x - 1;
            float y = 2 * xyzw.y / ScreenSize.y - 1;
            float z = xyzw.z;
            float w = xyzw.w;

            var clip = new Vector4f( x, y, z, w );
            var eye = InverseProjectionMatrix * clip;
            var world = InverseViewMatrix * eye;

            return world.Homogenized();
        }

        /// <summary>
        /// Unprojects a point from screen space back into world space
        /// screenXY is between [0, width/height]
        /// screenZ is between 0 and 1, with 0 at the near plane and 1 at the far plane
        /// </summary>
        /// <param name="screenXYZ"></param>
        /// <returns></returns>
        public Vector3f Unproject( Vector3f screenXYZ )
        {
            return Unproject( new Vector4f( screenXYZ, 1 ) );
        }

        /// <summary>
        /// Unprojects a point from screen space back into world space
        /// screenXY is between [0, width/height]
        /// screenZ is between 0 and 1, with 0 at the near plane and 1 at the far plane
        /// </summary>
        /// <param name="screenXY"></param>
        /// <param name="screenZ"></param>
        /// <returns></returns>
        public Vector3f Unproject( Vector2f screenXY, float screenZ )
        {
            return Unproject( new Vector4f( screenXY.x, screenXY.y, screenZ, 1 ) );
        }
        
        /// <summary>
        /// x points right, y points up
        /// </summary>
        /// <param name="screenXY"></param>
        /// <returns></returns>
        public Vector3f PixelToRay( int x, int y )
        {
            var screenXY = new Vector2f( x + 0.5f, y + 0.5f );
            var pointOnNearPlane = Unproject( screenXY, 0 );
            return ( pointOnNearPlane - Position ).Normalized();
        }

        /// <summary>
        /// x points right, y points UpVector
        /// </summary>
        /// <param name="screenXY"></param>
        /// <returns></returns>
        public Vector3f PixelToRay( Vector2f screenXY )
        {
            var pointOnNearPlane = Unproject( screenXY, 0 );
            return ( pointOnNearPlane - Position ).Normalized();
        }

        public void Pitch( float radians )
        {
            var m = Matrix4f.RotateX( radians );

            var rotatedUpOldCamera = ( m * Vector4f.UpH ).XYZ;
            var rotatedForwardOldCamera = ( m * Vector4f.ForwardH ).XYZ;

            var upWorld = CameraToWorld( rotatedUpOldCamera );
            var forwardWorld = CameraToWorld( rotatedForwardOldCamera );

            var ViewDirection = forwardWorld - Position;
            var upVector = upWorld - Position;
            SetOrientation( ViewDirection, upVector );

            ComputeViewMatrix();
        }

        public void Yaw( float radians )
        {
            var m = Matrix4f.RotateY( radians );

            var rotatedRightOldCamera = ( m * Vector4f.RightH ).XYZ;
            var rotatedForwardOldCamera = ( m * Vector4f.ForwardH ).XYZ;

            var rightWorld = CameraToWorld( rotatedRightOldCamera );
            var forwardWorld = CameraToWorld( rotatedForwardOldCamera );

            var upWorld = Vector3f.Cross( rightWorld - Position, forwardWorld - Position );

            var ViewDirection = forwardWorld - Position;
            var upVector = upWorld;
            SetOrientation( ViewDirection, upVector );

            ComputeViewMatrix();
        }

        /// <summary>
        /// Moves the camera, in its own coordinate system.
        /// I.e., if xyz = (0, 0, 1), the camera will move *backward*
        /// </summary>
        /// <param name="xyz"></param>
        public void Move( Vector3f xyz )
        {
            Move( xyz.x, xyz.y, xyz.z );
        }

        /// <summary>
        /// Moves the camera, in its own coordinate system.
        /// I.e., if xyz = (0, 0, 1), the camera will move *backward*
        /// </summary>
        /// <param name="xyz"></param>
        public void Move( float x, float y, float z )
        {
            //var deltaWorld = ( inverseViewMatrix * ( new Vector4f( xyz + Position, 1 ) ) ).XYZ;
            // Position += deltaWorld;

            Position += x * RightVector + y * UpVector - z * ViewDirection;

            ComputeViewMatrix();
        }

        public Matrix4f ViewMatrix
        {
            get
            {
                if( viewMatrixIsDirty )
                {
                    ComputeViewMatrix();
                }
                return viewMatrix;
            }
        }

        public Matrix4f InverseViewMatrix
        {
            get
            {
                if( viewMatrixIsDirty )
                {
                    ComputeViewMatrix();
                }
                return inverseViewMatrix;
            }
        }

        public Matrix4f ProjectionMatrix
        {
            get
            {
                if( projectionMatrixIsDirty )
                {
                    ComputeProjectionMatrix();
                }
                return projectionMatrix;
            }
        }

        public Matrix4f InverseProjectionMatrix
        {
            get
            {
                if( projectionMatrixIsDirty )
                {
                    ComputeProjectionMatrix();
                }
                return inverseProjectionMatrix;
            }
        }

        public Matrix4f ProjectionViewMatrix
        {
            get
            {
                if( projectionMatrixIsDirty || viewMatrixIsDirty )
                {
                    projectionViewMatrix = ProjectionMatrix * ViewMatrix;
                    inverseProjectionViewMatrix = projectionViewMatrix.Inverse();
                }
                return projectionViewMatrix;
            }
        }

        public Matrix4f ViewportMatrix
        {
            get
            {
                if( viewMatrixIsDirty )
                {
                    ComputeViewportMatrix();
                }
                return viewportMatrix;
            }
        }

        public Matrix4f InverseViewportMatrix
        {
            get
            {
                if( viewMatrixIsDirty )
                {
                    ComputeViewportMatrix();
                }
                return inverseViewportMatrix;
            }
        }

        public Matrix4f ScreenToWorld()
        {
            return InverseViewMatrix * InverseProjectionMatrix * InverseViewportMatrix;
        }        

        public bool IsVisible( Vector3f p )
        {
            // get a rectangle 1 unit away
            var rect = GetAlignedRectAt( 1 );
            
            // construct 4 planes
            var eye = Position;
            var leftPlane = new Plane3f( eye, rect.V00, rect.V01 );
            var topPlane = new Plane3f( eye, rect.V01, rect.V11 );
            var rightPlane = new Plane3f( eye, rect.V11, rect.V10 );
            var bottomPlane = new Plane3f( eye, rect.V10, rect.V00 );

            float d0 = leftPlane.Distance( p );
            float d1 = topPlane.Distance( p );
            float d2 = rightPlane.Distance( p );
            float d3 = bottomPlane.Distance( p );

            return d0 > 0 && d1 > 0 && d2 > 0 && d3 > 0;
        }

        // TODO: get a set of world space clipping planes?

        /// <summary>
        /// Returns a camera-aligned rectangle z units in front of the camera.        
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public Rectangle3f GetAlignedRectAt( float z )
        {
            float fTop = ( float ) ( z * Math.Tan( 0.5f * FieldOfView ) );
            float fBottom = -fTop;
            float fRight = AspectRatio * fTop;
            float fLeft = -fRight;

            var worldRight = Vector3f.Cross( ViewDirection, UpVector );
            var topLeft = Position + fTop * UpVector + fLeft * worldRight + z * ViewDirection;            
            var bottomLeft = Position + fBottom * UpVector + fLeft * worldRight + z * ViewDirection;
            var bottomRight = Position + fBottom * UpVector + fRight * worldRight + z * ViewDirection;

            return new Rectangle3f( bottomLeft, bottomRight - bottomLeft, topLeft - bottomLeft );
        }

        public Plane3f GetAlignedPlaneAt( float z )
        {
            var rect = GetAlignedRectAt( z );
            return new Plane3f( rect.V00, rect.V10, rect.V01 );
        }

        /// <summary>
        /// Returns whether or not p is within the *field of view* of this camera
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool WithinFieldOfView( Vector3f p )
        {
            // TODO: nz = 0 maps to the near plane, not the eye right?
            // actually test the planes

            var pNDC = ProjectionViewMatrix.TransformHomogenized( p );
            float nx = pNDC.x;
            float ny = pNDC.y;
            float nz = pNDC.z;

            return nx > -1 && nx < 1 && ny > -1 && ny < 1 && nz > 0;
        }

        /// <summary>
        /// Returns whether or not p is within the *field of view* of this camera
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool WithinViewFrustum( Vector3f p )
        {
            // TODO: nz = 0 maps to the near plane, not the eye right?
            // actually test the planes

            // TODO: triangle vs view frustum test

            var pNDC = ProjectionViewMatrix.TransformHomogenized( p );
            float nx = pNDC.x;
            float ny = pNDC.y;
            float nz = pNDC.z;

            return nx > -1 && nx < 1 && ny > -1 && ny < 1 && nz > 0 && nz < 1;
        }

        // TODO: tuple< bool, float >, or a record type?
        /// <summary>
        /// Returns the intersection between a ray and this came frustum
        /// </summary>
        /// <param name="rayOrigin"></param>
        /// <param name="rayDirection"></param>
        /// <param name="frustumDistance"></param>
        /// <returns></returns>
        public RayIntersectionRecord FrustumIntersectRay( Vector3f rayOrigin, Vector3f rayDirection, float frustumDistance )            
        {
            // grab the frustum triangles
            var rect = GetAlignedRectAt( frustumDistance );

            var hits = new RayIntersectionRecord[4];
            hits[ 0 ] = TriangleIntersection.IntersectRay( rayOrigin, rayDirection, Position, rect.V00, rect.V10 );
            hits[ 1 ] = TriangleIntersection.IntersectRay( rayOrigin, rayDirection, Position, rect.V10, rect.V11 );
            hits[ 2 ] = TriangleIntersection.IntersectRay( rayOrigin, rayDirection, Position, rect.V11, rect.V01 );
            hits[ 3 ] = TriangleIntersection.IntersectRay( rayOrigin, rayDirection, Position, rect.V01, rect.V00 );

            var minHit = hits.Where( h => h.Intersected && h.t > 0 ).ArgMin( h => h.t );
            if( minHit == null )
            {
                return RayIntersectionRecord.None;
            }
            else
            {
                return minHit;
            }
        }

        /// <summary>
        /// Returns the 4 points of the quadrilateral of the projection of the camera frustum onto a plane
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3f[] ProjectFrustumOntoPlane( Plane3f p )
        {
            var rect = GetAlignedRectAt( ZNear );

            var records = new RayIntersectionRecord[ 4 ];
            records[ 0 ] = p.IntersectRay( Position, rect.V00 - Position );
            records[ 1 ] = p.IntersectRay( Position, rect.V10 - Position );
            records[ 2 ] = p.IntersectRay( Position, rect.V11 - Position );
            records[ 3 ] = p.IntersectRay( Position, rect.V01 - Position );

            if( records.All( r => r.Intersected && r.t > 0 ) )
            {
                return records.Select( r => r.p ).ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// zFocus is the (positive) distance from the Position to the plane in focus
        /// Useful for accumulation buffer depth of field rendering.
        /// </summary>
        /// <param name="dxdy"></param>
        /// <param name="zFocus"></param>
        /// <param name="jitteredView"></param>
        /// <param name="jitteredProjection"></param>
        public void GetJittered( Vector2f dxdy, float zFocus,
            out Matrix4f jitteredView, out Matrix4f jitteredProjection )
        {
            // var OldCamera = new OldCamera( this );
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

        public override string ToString()
        {
            var sb = new StringBuilder( "Perspective Camera:\n" );
            
            sb.Append( "===== Extrinsics =====\n" );
            sb.AppendFormat( "Position: {0}\n", Position );
            sb.AppendFormat( "ViewDirection: {0}\n", ViewDirection );
            sb.AppendFormat( "UpVector: {0}\n", UpVector );
            
            sb.Append( "===== Intrinsics =====\n" );
            sb.AppendFormat( "Vertical FoV: {0} deg, Horizontal FoV: {1} deg\n", Arithmetic.RadiansToDegrees( FieldOfView ), Arithmetic.RadiansToDegrees( HorizontalFieldOfView ) );
            sb.AppendFormat( "ZNear: {0}, ZFar: {1}\n", ZNear, ZFar );
            sb.AppendFormat( "Screen size: {0} x {1}, Aspect ratio: {2}", ScreenSize.x, ScreenSize.y, AspectRatio );

            return sb.ToString();
        }

        public override bool Equals( object obj )
        {
            if( obj is OldCamera )
            {
                return Equals( ( OldCamera ) obj );
            }

            return false;
        }

        public bool Equals( OldCamera o )
        {
            if( fieldOfView != o.fieldOfView )
            {
                return false;
            }

            if( zNear != o.zNear )
            {
                return false;
            }

            if( zFar != o.zFar )
            {
                return false;
            }

            if( !( screenSize.Equals( o.screenSize ) ) )
            {
                return false;
            }

            return Frame.Equals( o.Frame );
        }

        public override int GetHashCode()
        {
            int h0 = fieldOfView.GetHashCode();
            int h1 = zNear.GetHashCode();
            int h2 = zFar.GetHashCode();
            int h3 = screenSize.GetHashCode();
            int h4 = Frame.GetHashCode();

            return h0 ^ h1 ^ h2 ^ h3 ^ h4;
        }

        /// <summary>
        /// tan( ty / 2 ) = (h/2)/zNear
        /// tan( tx / 2 ) = (w/2)/zNear
        /// w/h = A, so tan( tx / 2 ) = A * (h/2) / zNear
        /// tan(tx/2) = A tan(ty/2)
        /// </summary>
        public static float ComputeHorizontalFieldOfView( float fovYRadians, float width, float height )
        {
            float aspect = width / height;
            return 2.0f * MathUtils.Atan( MathUtils.Tan( 0.5f * fovYRadians ) * aspect );
        }

        public static float ComputeVerticalFieldOfView( float fovXRadians, float width, float height )
        {
            float aspect = width / height;            
            return 2.0f * MathUtils.Atan( MathUtils.Tan( 0.5f * fovXRadians ) / aspect );
        }

        public static OldCamera Lerp( OldCamera c0, OldCamera c1, float t )
        {
            if( c0.screenSize != c1.screenSize )
            {
                throw new ArgumentException( "Cannot interpolate between cameras of different screen size" );
            }

            var screenSize = c0.screenSize;
            var fov = Arithmetic.Lerp( c0.fieldOfView, c1.fieldOfView, t );
            var zNear = Arithmetic.Lerp( c0.zNear, c1.zNear, t );
            var zFar = Arithmetic.Lerp( c0.zFar, c1.zFar, t );
            var position = Vector3f.Lerp( c0.Position, c1.Position, t );
            var orientation = Quat4f.Slerp( c0.Orientation, c1.Orientation, t );

            return new OldCamera( position, orientation, fov, zNear, zFar, screenSize.x, screenSize.y );
        }
        
        /// <summary>
        /// Catmull-Rom cubic interpolation
        /// </summary>
        /// <param name="c0"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="c3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static OldCamera CubicInterpolate( OldCamera c0, OldCamera c1, OldCamera c2, OldCamera c3, float t )
        {
            if( c0.screenSize != c1.screenSize ||
                c1.screenSize != c2.screenSize ||
                c2.screenSize != c3.screenSize )
            {
                throw new ArgumentException( "Cannot interpolate between cameras of different screen size" );
            }

            var screenSize = c0.screenSize;
            var fov = Arithmetic.CubicInterpolate( c0.fieldOfView, c1.fieldOfView, c2.fieldOfView, c3.fieldOfView, t );
            var zNear = Arithmetic.CubicInterpolate( c0.zNear, c1.zNear, c2.zNear, c3.zNear, t );
            var zFar = Arithmetic.CubicInterpolate( c0.zFar, c1.zFar, c2.zFar, c3.zFar, t );
            var position = Vector3f.CubicInterpolate( c0.Position, c1.Position, c2.Position, c3.Position, t );
            var orientation = Quat4f.CubicInterpolate( c0.Orientation, c1.Orientation, c2.Orientation, c3.Orientation, t );

            return new OldCamera( position, orientation, fov, zNear, zFar, screenSize.x, screenSize.y );
        }

        private void ComputeViewMatrix()
        {
            viewMatrix = Matrix4f.LookAt( Position, Position + ViewDirection, UpVector );
            inverseViewMatrix = viewMatrix.Inverse();
            
            // HACK: look at this later
            viewMatrixIsDirty = true;
        }

        private void ComputeProjectionMatrix()
        {
            projectionMatrix = Matrix4f.PerspectiveD3D( FieldOfView, AspectRatio, ZNear, ZFar );
            inverseProjectionMatrix = projectionMatrix.Inverse();

            // HACK: look at this later
            projectionMatrixIsDirty = true;
        }

        private void ComputeViewportMatrix()
        {
            viewportMatrix = Matrix4f.Viewport( ScreenSize.x, ScreenSize.y );
            inverseViewportMatrix = Matrix4f.InverseViewport( ScreenSize.x, ScreenSize.y );

            // HACK: look at this later
            viewportMatrixIsDirty = true;
        }
        
        public Vector3f WorldToCamera( Vector3f xyz )
        {
            return ( ViewMatrix * new Vector4f( xyz, 1 ) ).XYZ;
        }
        
        public Vector3f CameraToWorld( Vector3f xyz )
        {
            return ( InverseViewMatrix * new Vector4f( xyz, 1 ) ).XYZ;
        }
    }
}
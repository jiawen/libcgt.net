using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public enum UniversalManipulatorTransformType
    {
        None,
        Translation,
        Rotation,
        Scale
    }

    public class UniversalManipulatorTransform
    {
        public UniversalManipulatorTransformType transformType;
        public Matrix4f transform;

        public UniversalManipulatorTransform()
        {
            transformType = UniversalManipulatorTransformType.None;
            transform = Matrix4f.Identity;
        }
    }

    public class UniversalManipulatorWidget
    {
        private const float EPSILON = 0.1f;

        public Vector3f center;
        private int selectedObject = -1;
        private RayIntersectionRecord mouseDownIntersectionRecord;

        // TODO: properties
        public Ellipse3f RotateXEllipse { get; private set; }
        public Ellipse3f RotateYEllipse { get; private set; }
        public Ellipse3f RotateZEllipse { get; private set; }

        public Cone3f TranslateXCone { get; private set; }
        public Cone3f TranslateYCone { get; private set; }
        public Cone3f TranslateZCone { get; private set; }

        public Plane3f debugPlane;

        public UniversalManipulatorWidget( Vector3f center )
        {
            this.center = center;

            RotateXEllipse = new Ellipse3f( center, new Vector3f( 0, 0.5f, 0 ), new Vector3f( 0, 0, 0.5f ) );
            RotateYEllipse = new Ellipse3f( center, new Vector3f( 0.5f, 0, 0 ), new Vector3f( 0, 0, -0.5f ) );
            RotateZEllipse = new Ellipse3f( center, new Vector3f( 0, 0.5f, 0 ), new Vector3f( -0.5f, 0, 0 ) );

            TranslateXCone = new Cone3f( center + new Vector3f( 1, 0, 0 ), new Vector3f( 1, 0, 0 ), 0.05f, 0.2f );
            TranslateYCone = new Cone3f( center + new Vector3f( 0, 1, 0 ), new Vector3f( 0, 1, 0 ), 0.05f, 0.2f );
            TranslateZCone = new Cone3f( center + new Vector3f( 0, 0, 1 ), new Vector3f( 0, 0, 1 ), 0.05f, 0.2f );
        }

        public void HandleMouseDown( Vector3f rayOrigin, Vector3f rayDirection )
        {
            var rxir = RotateXEllipse.IntersectRay( rayOrigin, rayDirection, EPSILON );
            var ryir = RotateYEllipse.IntersectRay( rayOrigin, rayDirection, EPSILON );
            var rzir = RotateZEllipse.IntersectRay( rayOrigin, rayDirection, EPSILON );

            var txir = TranslateXCone.IntersectRay( rayOrigin, rayDirection );
            var tyir = TranslateYCone.IntersectRay( rayOrigin, rayDirection );
            var tzir = TranslateZCone.IntersectRay( rayOrigin, rayDirection );

            var irList = new List< RayIntersectionRecord >{ rxir, ryir, rzir, txir, tyir, tzir };
            float minT;
            int minValidIndex = irList.MinValidIndex( r => r.Intersected, r => r.t, out minT );
            if( minValidIndex != -1 )
            {
                selectedObject = minValidIndex;
                mouseDownIntersectionRecord = irList[ minValidIndex ];
            }
        }

        public UniversalManipulatorTransform HandleMouseMove( Vector3f rayOrigin, Vector3f rayDirection, Vector3f up )
        {
            var output = new UniversalManipulatorTransform();

            if( selectedObject == -1 )
            {
                return output;
            }

            // rotation
            if( selectedObject < 3 )
            {
                output.transformType = UniversalManipulatorTransformType.Rotation;

                var rxir = RotateXEllipse.IntersectRay( rayOrigin, rayDirection, EPSILON );
                var ryir = RotateYEllipse.IntersectRay( rayOrigin, rayDirection, EPSILON );
                var rzir = RotateZEllipse.IntersectRay( rayOrigin, rayDirection, EPSILON );

                var txir = TranslateXCone.IntersectRay( rayOrigin, rayDirection );
                var tyir = TranslateYCone.IntersectRay( rayOrigin, rayDirection );
                var tzir = TranslateZCone.IntersectRay( rayOrigin, rayDirection );

                var irList = new List< RayIntersectionRecord > { rxir, ryir, rzir, txir, tyir, tzir };
                
                if( irList[ selectedObject ].Intersected )
                {
                    output.transformType = UniversalManipulatorTransformType.Rotation;

                    var theta = ( irList[ selectedObject ] as RayEllipseIntersectionRecord ).theta;
                    float dTheta = theta - ( mouseDownIntersectionRecord as RayEllipseIntersectionRecord ).theta;

                    if( selectedObject == 0 )
                    {
                        output.transform = Matrix4f.RotateX( dTheta );
                    }
                    else if( selectedObject == 1 )
                    {
                        output.transform = Matrix4f.RotateY( dTheta );
                    }
                    else
                    {
                        output.transform = Matrix4f.RotateZ( dTheta );
                    }
                }

                mouseDownIntersectionRecord = irList[ selectedObject ];
                return output;
            }
            else
            {
                // translation:
                // define a plane between the click, the center, and the camera's up vector
                output.transformType = UniversalManipulatorTransformType.Translation;
                
                var pOld = mouseDownIntersectionRecord.p;
                // var planeX = ( pOld - center ).Normalized();
                Vector3f planeX;
                if( selectedObject == 3 )
                {
                    planeX = Vector3f.Right;
                }
                else if( selectedObject == 4 )
                {
                    planeX = Vector3f.Up;
                }
                else
                {
                    planeX = -Vector3f.Forward;
                }

                var planeNormal = Vector3f.Cross( planeX, up );
                var plane = new Plane3f( center, planeNormal );
                debugPlane = plane;
                
                // intersect the mouse down point with the plane
                var planeIR = plane.IntersectRay( rayOrigin, rayDirection );
                if( planeIR.Intersected )
                {
                    var pNew = planeIR.p;

                    // depending on which cone was picked, project the amount onto the appropriate vector
                    if( selectedObject == 3 )
                    {
                        float dx = pNew.x - pOld.x;
                        output.transform = Matrix4f.Translation( dx, 0, 0 );
                    }
                    else if( selectedObject == 4 )
                    {
                        float dy = pNew.y - pOld.y;
                        output.transform = Matrix4f.Translation( 0, dy, 0 );
                    }
                    else
                    {
                        float dz = pNew.z - pOld.z;
                        output.transform = Matrix4f.Translation( 0, 0, dz );
                    }

                    RotateXEllipse.Center = output.transform.TransformHomogenized( RotateXEllipse.Center );
                    RotateYEllipse.Center = output.transform.TransformHomogenized( RotateYEllipse.Center );
                    RotateZEllipse.Center = output.transform.TransformHomogenized( RotateZEllipse.Center );

                    TranslateXCone.Origin = output.transform.TransformHomogenized( TranslateXCone.Origin );
                    TranslateYCone.Origin = output.transform.TransformHomogenized( TranslateYCone.Origin );
                    TranslateZCone.Origin = output.transform.TransformHomogenized( TranslateZCone.Origin );

                    mouseDownIntersectionRecord = planeIR;
                    return output;
                }
            }
                    
            return output;
        }

        public void HandleMouseUp()
        {
            selectedObject = -1;
            mouseDownIntersectionRecord = null;
        }
    }
}

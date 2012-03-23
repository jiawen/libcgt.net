using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt;
using libcgt.core.Geometry;
using libcgt.core.Vecmath;

namespace libcgt.core
{    
    public class SpotLight : AbstractData
    {
        public Vector3f Position { get; set; }
        public Vector3f Target { get; set; }
        public Vector3f Up { get; set; }
        public Vector3f Color { get; set; }
        private float fieldOfView;

        public float FieldOfView
        {
            get
            {
                return fieldOfView;
            }
            set
            {
                fieldOfView = value;
                OnPropertyChanged( "FieldOfView" );
            }
        }

        public float Red
        {
            get
            {
                return Color.x;
            }
            set
            {
                var c = Color;
                c.x = value;
                Color = c;
                OnPropertyChanged( "Red" );
            }
        }

        public float Green
        {
            get
            {
                return Color.y;
            }
            set
            {
                var c = Color;
                c.y = value;
                Color = c;
                OnPropertyChanged( "Green" );
            }
        }

        public float Blue
        {
            get
            {
                return Color.z;
            }
            set
            {
                var c = Color;
                c.z = value;
                Color = c;
                OnPropertyChanged( "Blue" );
            }
        }

        // TODO: up is annoying
        // zNear and zFar should be adjusted to the scene

        /// <summary>
        /// fieldOfView in radians
        /// </summary>
        /// <param name="position"></param>
        /// <param name="target"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="color"></param>
        public SpotLight( Vector3f position, Vector3f target, float fieldOfView,
            Vector3f color )
        {
            Position = position;
            Target = target;            
            FieldOfView = fieldOfView;
            Color = color;

            // pick an up vector:
            // construct the plane
            // then pick any arbitrary point on it
            Up = ( ( new Plane3f( position, target - position ) ).PointOnPlane() - position ).Normalized();
        }

        public Vector3f Direction
        {
            get
            {
                return ( Target - Position ).Normalized();
            }
        }

        public Matrix4f ViewMatrix
        {
            get
            {
                return Matrix4f.LookAt( Position, Target, Up );
            }
        }

        public Matrix4f ProjectionMatrix
        {
            get
            {
                const float zNear = 0.1f;
                const float zFar = 1000.0f;
                return Matrix4f.PerspectiveD3D( FieldOfView, 1, zNear, zFar );
            }
        }

        public Matrix4f ProjectionViewMatrix
        {
            get
            {
                return ProjectionMatrix * ViewMatrix;
            }
        }

        public Ellipse3f GetAlignedCircleAt( float z )
        {
            float fTop = ( float ) ( z * Math.Tan( 0.5f * FieldOfView ) );
            float fRight = fTop;
            
            var right = Vector3f.Cross( Direction, Up );
            return new Ellipse3f( Position + z * Direction, fRight * right, fTop * Up );
        }
    }
}

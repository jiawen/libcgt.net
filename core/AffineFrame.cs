using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    /// <summary>
    /// An affine coordinate frame.  Translation occurs first, then rotated about the new origin.
    /// CoordinatesOf: transforms a point
    /// TransformOf: transforms a vector (applies the rotational part only)
    /// </summary>
    [Serializable]
    public class AffineFrame : IEquatable< AffineFrame >
    {
        public Vector3f Translation { get; set; }
        public Quat4f Rotation { get; set; }

        public AffineFrame()
        {
            Translation = Vector3f.Zero;
            Rotation = Quat4f.Identity;            
        }

        public AffineFrame( Vector3f translation, Quat4f rotation )
        {
            Translation = translation;
            Rotation = rotation;
        }

        public AffineFrame( AffineFrame frame )
        {
            Translation = frame.Translation;
            Rotation = frame.Rotation;
        }

        // TODO: translation and rotation are relative to the referenceFrame
        // Position and Orientation are absolute
        public Vector3f Position
        {
            get
            {
                return Translation;
            }
            set
            {
                Translation = value;
            }
        }

        // TODO: does the whole recursive thing
        public Quat4f Orientation
        {
            get
            {
                return Rotation;                
            }
            set
            {
                Rotation = value;
            }
        }

        /// <summary>
        /// World --> Frame
        /// </summary>
        public Matrix4f Matrix
        {
            get
            {
                var m = Rotation.GetMatrix();
            
                m[ 0, 3 ] = Translation.x;
                m[ 1, 3 ] = Translation.y;
                m[ 2, 3 ] = Translation.z;

                return m;
            }
            set
            {
                Translation = value.GetColumn( 3 ).Homogenized();
                var rotationMatrix = value.GetSubmatrix3x3( 0, 0 ) / value[ 3, 3 ];
                Rotation = Quat4f.FromRotationMatrix( rotationMatrix );
            }
        }

        public Matrix4f InverseMatrix
        {
           get
           {
               return Matrix.Inverse();
           }
        }

        public void Rotate( Quat4f q )
        {
            Rotation = ( Rotation * q ).Normalized();
        }

        public void RotateAroundPoint( Quat4f rotation, Vector3f point )
        {
            Rotation = ( Rotation * rotation ).Normalized();

            var q = new Quat4f( InverseTransformOf( rotation.Axis ), rotation.Angle );
            Vector3f trans = point + q.Rotate( Position - point ) - Translation;
            Translation += trans;
        }

        public void Translate( Vector3f t )
        {
            Translation += t;
        }

        public void Translate( float x, float y, float z )
        {
            Translate( new Vector3f( x, y, z ) );
        }

        /// <summary>
        /// World --> Frame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f CoordinatesOf( Vector3f src )
        {
            return LocalCoordinatesOf( src );
        }

        /// <summary>
        /// Frame --> World
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f InverseCoordinatesOf( Vector3f src )
        {
            return LocalInverseCoordinatesOf( src );
        }

        public Vector3f TransformOf( Vector3f src )
        {
            return LocalTransformOf( src );
        }

        public Vector3f InverseTransformOf( Vector3f src )
        {
            return LocalInverseTransformOf( src );
        }
        
        /// <summary>
        /// Converts coordinates in the reference frame to this frame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f LocalCoordinatesOf( Vector3f src )
        {
            return Rotation.InverseRotate( src - Translation );
        }

        /// <summary>
        /// Converts coordinates from this frame to the reference frame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f LocalInverseCoordinatesOf( Vector3f src )
        {
            return Rotation.Rotate( src ) + Translation;
        }

        /// <summary>
        /// Converts vectors in the reference frame to this frame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f LocalTransformOf( Vector3f src )
        {
            return Rotation.InverseRotate( src );
        }

        /// <summary>
        /// Converts vectors in this frame to the reference frame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f LocalInverseTransformOf( Vector3f src )
        {
            return Rotation.Rotate( src );
        }        

        public override bool Equals( object obj )
        {
            if( obj is AffineFrame )
            {
                return Equals( ( AffineFrame ) obj );
            }

            return false;
        }

        public bool Equals( AffineFrame o )
        {
            return ( Translation == o.Translation ) && ( Rotation == o.Rotation );
        }

        public override int GetHashCode()
        {
            return Translation.GetHashCode() ^ Rotation.GetHashCode();
        }
    }
}

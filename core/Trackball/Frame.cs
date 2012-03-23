using System;
using libcgt.core.Vecmath;

namespace libcgt.core.Trackball
{
    public class Frame
    {
        // TODO: emit modified on translation, rotation
        public Vector3f t_;
        public Quat4f q_;

        private Frame referenceFrame;

        public Frame()
        {
            t_ = Vector3f.Zero;
            q_ = Quat4f.Identity;
            referenceFrame = null;
        }

        public Frame( Vector3f translation, Quat4f rotation )
        {
            this.Translation = translation;
            this.Rotation = rotation;
            referenceFrame = null;
        }

        public Frame( Frame frame )
        {
            SetTranslationAndRotation( frame.Translation, frame.Rotation );
            // SetConstraint( frame.Constraint )
            ReferenceFrame = frame.ReferenceFrame;
        }

        public bool SettingAsReferenceFrameWillCreateALoop( Frame frame )
        {
            Frame f = frame;
            while( f != null )
            {
                if( f == this )
                {
                    return true;
                }
                f = f.ReferenceFrame;
            }
            return false;
        }

        public void SetTranslationAndRotation( Vector3f translation, Quat4f rotation )
        {
            this.t_ = translation;
            this.q_ = rotation;

            // TODO:
            // emit modified
        }

        public Vector3f Translation
        {
            get
            {
                return t_;
            }
            set
            {
                t_ = value;
            }
        }

        public Quat4f Rotation
        {
            get
            {
                return q_;
            }
            set
            {
                q_ = value;
            }
        }

        public Vector3f Position
        {
            get
            {
                return InverseCoordinatesOf( Vector3f.Zero );
            }
            set
            {
                if( ReferenceFrame != null )
                {
                    Translation = ReferenceFrame.CoordinatesOf( value );
                }
                else
                {
                    Translation = value;
                }
            }
        }

        public Quat4f Orientation
        {
            get
            {
                Quat4f res = Rotation;
                Frame fr = ReferenceFrame;
                while( fr != null )
                {
                    res = fr.Rotation * res;
                    fr = fr.ReferenceFrame;
                }
                return res;
            }
            set
            {
                if( ReferenceFrame != null )
                {
                    Rotation = ReferenceFrame.Orientation.Inverse() * value;                        
                }
                else
                {
                    Rotation = value;
                }  
            }
        }

        public Frame ReferenceFrame
        {
            get
            {
                return referenceFrame;
            }
            set
            {
                if( SettingAsReferenceFrameWillCreateALoop( value ) )
                {
                    throw new ArgumentException( "Setting value as referenceFrame would create a loop in Frame hierarchy" );
                }
                
                bool identical = ( referenceFrame == value );
                referenceFrame = value;

                // TODO:
                // if( !identical )
                // emit modified();
            }
        }

        public Matrix4f GetMatrix()
        {
            Matrix4f m = Rotation.GetMatrix();
            
            m[ 0, 3 ] = Translation.x;
            m[ 1, 3 ] = Translation.y;
            m[ 2, 3 ] = Translation.z;

            return m;
        }

        public Frame Inverse()
        {
            Frame inverseFrame = new Frame( -( Rotation.InverseRotate( Translation ) ), Rotation.Inverse() );
            inverseFrame.ReferenceFrame = referenceFrame;
            return inverseFrame;
        }

        public Matrix4f GetWorldMatrix()
        {
            if( referenceFrame == null )
            {
                return GetMatrix();
            }
            else
            {
                return GetMatrix() * referenceFrame.GetWorldMatrix();
            }
        }

        public void Translate( Vector3f t )
        {
            Vector3f v = t;
            Translate( ref v );
        }

        public void Translate( ref Vector3f t )
        {
            //if (constraint())
            //constraint()->constrainTranslation(t, this);
            
            this.Translation += t;
            // emit modified()
        }

        public void Translate( float x, float y, float z )
        {
            Vector3f v = new Vector3f( x, y, z );
            Translate( v );
        }

        public void Translate( ref float x, ref float y, ref float z )
        {
            Vector3f v = new Vector3f( x, y, z );
            Translate( ref v );
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public void Rotate( Quat4f q )
        {
            Quat4f qbis = q;
            Rotate( ref qbis );
        }

        public void Rotate( ref Quat4f q )
        {
            // if (constraint())
            // constraint()->constrainRotation(q, this);
            
            Rotation *= q;
            Rotation = Rotation.Normalized();

            // TODO: emit modified()
        }

        public void RotateAroundPoint( ref Quat4f rotation, Vector3f point )
        {
            // TODO: constrained
            // if (constraint())
            //     constraint()->constrainRotation(rotation, this);
            
            Rotation *= rotation;
            Rotation = Rotation.Normalized(); // Prevents numerical drift

            Quat4f q = new Quat4f( InverseTransformOf( rotation.Axis ), rotation.Angle );
            Vector3f trans = point + q.Rotate( Position - point ) - Translation;
  
            // if (constraint())
            // constraint()->constrainTranslation(trans, this);
            
            Translation += trans;
            
            // emit modified();
        }

        public void AlignWithFrame( Frame frame )
        {
            AlignWithFrame( frame, false );
        }

        public void AlignWithFrame( Frame frame, bool move )
        {
            // TODO: default_threshold
            AlignWithFrame( frame, move, 0.85f );
        }

        public void AlignWithFrame( Frame frame, bool move, float threshold )
        {
            Vector3f[,] directions = new Vector3f[ 2, 3 ];
            
            for( int d = 0; d < 3; ++d )
            {
                Vector3f dir = new Vector3f( ( d == 0 ) ? 1.0f : 0.0f, ( d == 1 ) ? 1.0f : 0.0f, ( d == 2 ) ? 1.0f : 0.0f );
                if( frame != null )
                {
                    directions[ 0, d ] = frame.InverseTransformOf( dir );
                }
                else
                {
                    directions[ 0, d ] = dir;
                }
                directions[ 1, d ] = InverseTransformOf( dir );
            }

            float maxProj = 0.0f;
            float proj;
            
            int[] index = new int[ 2 ];
            index[ 0 ] = 0;
            index[ 1 ] = 0;
            
            for( int i = 0; i < 3; ++i )
            {
                for( int j = 0; j < 3; ++j )
                {
                    proj = ( float )( Math.Abs( Vector3f.Dot( directions[ 0, i ], directions[ 1, j ] ) ) );
                    if( proj >= maxProj )
                    {
                        index[ 0 ] = i;
                        index[ 1 ] = j;
                        maxProj = proj;
                    }
                }
            }

            Frame old = new Frame( this );

            float coef = Vector3f.Dot( directions[ 0, index[ 0 ] ], directions[ 1, index[ 1 ] ] );
            if( Math.Abs( coef ) >= threshold )
            {
                Vector3f axis = Vector3f.Cross( directions[ 0, index[ 0 ] ], directions[ 1, index[ 1 ] ] );
                float angle = ( float )( Math.Asin( axis.Norm() ) );
                if( coef >= 0.0f )
                {
                    angle = -angle;
                }
                // setOrientation(Quaternion(axis, angle) * orientation()); // commented out in original code
                Rotate( Rotation.Inverse() * new Quat4f( axis, angle ) * Orientation );

                // Try to align an other axis direction
                int d = ( index[ 1 ] + 1 ) % 3;
                Vector3f dir = new Vector3f( ( d == 0 ) ? 1.0f : 0.0f, ( d == 1 )? 1.0f : 0.0f, ( d == 2 ) ? 1.0f : 0.0f );
                dir = InverseTransformOf( dir );

                float max = 0.0f;
                for (int i=0; i<3; ++i)
                {
                    float proj2 = Math.Abs( Vector3f.Dot( directions[ 0, i ], dir ) );
                    if( proj2 > max )
                    {
                        index[ 0 ] = i;
                        max = proj2;
                    }
                }
                
                if( max >= threshold )
                {
                    Vector3f axis2 = Vector3f.Cross( directions[ 0, index[ 0 ] ], dir);
                    float angle2 = ( float )( Math.Asin( axis2.Norm() ) );
                    if( Vector3f.Dot( directions[ 0, index[ 0 ] ], dir ) >= 0.0f )
                    {
                        angle2 = -angle2;
                    }
                    // setOrientation(Quaternion(axis, angle) * orientation()); // commented out in original source
                    Rotate( Rotation.Inverse() * new Quat4f( axis2, angle2 ) * Orientation );
                }
            }

            if( move )
            {
                Vector3f center = Vector3f.Zero;
                if( frame != null )
                {
                    center = frame.Position;
                }

                // setPosition(center - orientation().rotate(old.coordinatesOf(center))); // commented out in original source
                Vector3f v = old.CoordinatesOf( center );
                Vector3f foo = Orientation.Rotate( v );

                // Translate( center - Orientation.Rotate( old.CoordinatesOf( center ) ) - Translation );
            }
        }

        // world --> frame coordinates
        public Vector3f CoordinatesOf( Vector3f src )
        {
            if( referenceFrame != null )
            {
                return LocalCoordinatesOf( referenceFrame.CoordinatesOf( src ) );
            }
            else
            {
                return LocalCoordinatesOf( src );
            }
        }

        // frame --> world coordinates
        public Vector3f InverseCoordinatesOf( Vector3f src )
        {
            Frame fr = this;
            Vector3f res = src;
            while( fr != null )
            {
                res = fr.LocalInverseCoordinatesOf( res );
                fr = fr.ReferenceFrame;
            }
            return res;
        }

        // ReferenceFrame coordinates --> frame coordinates
        public Vector3f LocalCoordinatesOf( Vector3f src )
        {
            return Rotation.InverseRotate( src - Translation );
        }

        // frame --> ReferenceFrame coordinates
        public Vector3f LocalInverseCoordinatesOf( Vector3f src )
        {
            return Rotation.Rotate( src ) + Translation;
        }

        /// <summary>
        /// converts vectors from world to Frame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f TransformOf( Vector3f src )
        {
            if( ReferenceFrame != null )
            {
                return LocalTransformOf( ReferenceFrame.TransformOf( src ) );
            }
            else
            {
                return LocalTransformOf( src );
            }
        }

        /// <summary>
        /// converts vectors from Frame to world
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f InverseTransformOf( Vector3f src )
        {
            Frame fr = this;
            Vector3f res = src;
            while( fr != null )
            {
                res = fr.LocalInverseTransformOf( res );
                fr = fr.ReferenceFrame;
            }
            return res;
        }

        /// <summary>
        /// converts vectors from ReferenceFrame to Frame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f LocalTransformOf( Vector3f src )
        {
            return Rotation.InverseRotate( src );
        }

        /// <summary>
        /// converts vectors from Frame to ReferenceFrame
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public Vector3f LocalInverseTransformOf( Vector3f src )
        {
            return Rotation.Rotate( src );
        }
    }
}

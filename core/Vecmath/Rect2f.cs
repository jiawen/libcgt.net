using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable]
    public struct Rect2f
    {
        private Vector2f origin;
        private Vector2f size;

        public Rect2f( Vector2f origin, Vector2f size )
        {
            this.origin = origin;
            this.size = size;
        }

        public Rect2f( float x, float y, float width, float height )
        {
            origin = new Vector2f( x, y );
            size = new Vector2f( width, height );
        }
        
        public Rect2f( Vector2i size )
            : this( size.x, size.y )
        {

        }

        public Rect2f( Vector2f size )
            : this( size.x, size.y )
        {

        }

        public Rect2f( float width, float height )
            : this( 0, 0, width, height )
        {

        }        

        public Rect2f( Rect2i rect )
        {
            origin = new Vector2f( rect.Origin );
            size = new Vector2f( rect.Size );
        }

        public Rect2f( Rect2f copy )
        {
            this.origin = copy.origin;
            this.size = copy.size;
        }

        public Vector2f Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
            }
        }

        public Vector2f Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public Vector2f BottomRight
        {
            get
            {
                return Origin + new Vector2f( Width, 0 );
            }
        }

        public Vector2f TopLeft
        {
            get
            {
                return Origin + new Vector2f( 0, Height );
            }
        }

        public Vector2f TopRight
        {
            get
            {
                return Origin + Size;
            }
        }

        public float Width
        {
            get
            {
                return Size.x;
            }
        }

        public float Height
        {
            get
            {
                return Size.y;
            }
        }

        public float Area
        {
            get
            {
                return Width * Height;
            }
        }
        
        public Rect2f Dilated( float dx, float dy )
        {
            return new Rect2f( Origin - new Vector2f( dx, dy ), Size + new Vector2f( 2 * dx, 2 * dy ) );
        }

        public Rect2f United( Rect2f r )
        {
            Vector2f ro = r.Origin;
            Vector2f tr = TopRight;
            Vector2f rtr = r.TopRight;

            float ox = Math.Min( origin.x, ro.x );
            float oy = Math.Min( origin.y, ro.y );

            float trX = Math.Max( tr.x, rtr.x );
            float trY = Math.Max( tr.y, rtr.y );

            return new Rect2f( ox, oy, trX - ox, trY - oy );
        }

        public bool Contains( Vector2f p )
        {
            return
	        (
		        ( p.x > Origin.x ) &&
		        ( p.x < ( Origin.x + Width ) ) &&
		        ( p.y > Origin.y ) &&
		        ( p.y < ( Origin.y + Height ) )
	        );
        }

        /// <summary>
        /// Returns the smallest integer rectangle that contains this
        /// origin = floor( origin )
        /// size = ceil( size )
        /// </summary>
        /// <returns></returns>
        public Rect2i DilateToInt()
        {
            return new Rect2i( Origin.FloorToInt(), Size.CeilToInt() );
        }

        // TODO: normalized, empty()
        public static Rect2f Intersection( Rect2f r0, Rect2f r1 )
        {
            Rect2f tmp;
            tmp.origin = new Vector2f
            (
                Math.Max( r0.origin.x, r1.origin.x ),
                Math.Max( r0.origin.y, r1.origin.y )
            );
            tmp.size = new Vector2f
            (
                Math.Min( r0.origin.x + r0.Width, r1.origin.x + r1.Width ) - tmp.origin.x,
                Math.Min( r0.origin.y + r0.Height, r1.origin.y + r1.Height ) - tmp.origin.y
            );
            return tmp;
            
            /*
             * QRectF r1 = normalized();
             * QRectF r2 = r.normalized();    
             * return tmp.isEmpty() ? QRectF() : tmp;
            */
            
        }

        public Vector4f PackToVector4f()
        {
            return new Vector4f( Origin.x, Origin.y, TopRight.x, TopRight.y );
        }        

        /// <summary>
        /// given x_window, find x_image
		/// 1. convert x_window --> x_rect: x_rect = x_window - rectOriginX
		/// 2. scale x_rect to [0,1]: x_zo = x_rect / rectWidth
		/// 3. scale it back to image coordinates: x_image = x_zo * imageWidth
		/// x_image = imageWidth * ( x_window - rectOriginX ) / rectWidth
		///         = ( imageWidth / rectWidth ) * x_window + ( -imageWidth * rectOriginX / rectWidth )
		///
		/// same for y
		///
		/// write it as a 3x3 affine transformation
        /// </summary>
        /// <param name="image"></param>
        /// <param name="window"></param>
        /// <param name="windowToImage"></param>
        /// <returns></returns>
        public static Rect2f BestFitRectangle( Rect2f image, Rect2f window,
            out Matrix3f windowToImage )
        {
            float rectOriginX;
	        float rectOriginY;
	        float rectWidth;
	        float rectHeight;

            float imageAspectRatio = image.Width / image.Height;
            float windowAspectRatio = window.Width / window.Height;

	        // if the image is fatter than the window
	        // then fit it width-wise (rectWidth = window.width, and adjust the height)
	        if( imageAspectRatio > windowAspectRatio )
	        {
	            rectWidth = window.Width;
	            rectOriginX = window.Origin.x;

		        rectHeight = window.Width * image.Height / image.Width;
		        rectOriginY = window.Origin.y + 0.5f * ( window.Height - rectHeight );
	        }
	        // otherwise, the image is taller than the window
	        // so fit it height-wise (rectHeight = window.height)
	        else
	        {
		        rectWidth = window.Height * image.Width / image.Height;
		        rectOriginX = window.Origin.x + 0.5f * ( window.Width - rectWidth );

		        rectHeight = window.Height;
		        rectOriginY = window.Origin.y;
	        }

	        // compute the transformation from window to image coordinates
	        // given x_window, find x_image
	        // 1. convert x_window --> x_rect: x_rect = x_window - rectOriginX
	        // 2. scale x_rect to [0,1]: x_zo = x_rect / rectWidth
	        // 3. scale it back to image coordinates: x_image = x_zo * imageWidth
	        // x_image = imageWidth * ( x_window - rectOriginX ) / rectWidth
	        //         = ( imageWidth / rectWidth ) * x_window + ( -imageWidth * rectOriginX / rectWidth )
	        //
	        // same for y
	        //
	        // write it as a 3x3 affine transformation
	        //

	        windowToImage = new Matrix3f
	        (
		        image.Width / rectWidth,	0.0f,						-image.Width * rectOriginX / rectWidth,
		        0.0f,						image.Height / rectHeight,  -image.Height * rectOriginY / rectHeight,
		        0.0f,						0.0f,						1.0f
	        );

	        return new Rect2f( rectOriginX, rectOriginY, rectWidth, rectHeight );
        }

        public override string ToString()
        {
            return string.Format( "[ origin: {0}, topRight: {1} ]", origin, TopRight );
        }

        public override int GetHashCode()
        {
            return origin.GetHashCode() ^ size.GetHashCode();
        }

        public override bool Equals( object o )
        {
            if( o is Rect2f )
            {                
                var rhs = ( Rect2f )o;
                return( Equals( rhs ) );
            }
            return false;
        }

        public bool Equals( Rect2f other )
        {
            return ( origin.Equals( other.origin ) && size.Equals( other.size ) );
        }

        // implicit conversion to Rect2f
        public static implicit operator Rect2f( Rect2i rect )
        {
            return new Rect2f( rect.Origin, rect.Size );
        }

        // explicit conversion to Rect2i
        public static explicit operator Rect2i( Rect2f rect )
        {
            return new Rect2i( ( Vector2i )( rect.Origin ), ( Vector2i )( rect.Size ) );
        }
    }
}

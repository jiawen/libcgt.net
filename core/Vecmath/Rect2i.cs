using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core.Vecmath
{
    [Serializable]
    public struct Rect2i
    {
        private Vector2i origin;
        private Vector2i size;

        public Rect2i( Vector2i origin, Vector2i size )
        {
            this.origin = origin;
            this.size = size;
        }

        public Rect2i( int x, int y, int width, int height )
        {
            origin = new Vector2i( x, y );
            size = new Vector2i( width, height );
        }

        public Rect2i( Vector2i origin, int width, int height )
        {
            this.origin = origin;
            this.size = new Vector2i( width, height );
        }

        public Rect2i( int x, int y, Vector2i size )
        {
            origin = new Vector2i( x, y );
            this.size = size;
        }

        public Rect2i( int width, int height )
            : this( 0, 0, width, height )
        {

        }

        public Rect2i( Rect2i copy )
        {
            this.origin = copy.origin;
            this.size = copy.size;
        }

        public Vector2i Origin
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

        public Vector2i Size
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

        public Vector2i BottomRight
        {
            get
            {
                return Origin + new Vector2i( Width, 0 );
            }
        }

        public Vector2i TopLeft
        {
            get
            {
                return origin + new Vector2i( 0, Height );
            }
        }

        public Vector2i TopRight
        {
            get
            {
                return Origin + Size;
            }
        }

        public int Width
        {
            get
            {
                return Size.x;
            }
        }

        public int Height
        {
            get
            {
                return Size.y;
            }
        }

        public int Area
        {
            get
            {
                return Width * Height;
            }
        }

        public Rect2i Dilated( int dx, int dy )
        {
            return new Rect2i( Origin - new Vector2i( dx, dy ), Size + new Vector2i( 2 * dx, 2 * dy ) );
        }

        // TODO: normalized, empty()
        public static Rect2i Intersection( Rect2i r0, Rect2i r1 )
        {
            Rect2i tmp;
            tmp.origin = new Vector2i
            (
                Math.Max( r0.origin.x, r1.origin.x ),
                Math.Max( r0.origin.y, r1.origin.y )
            );
            tmp.size = new Vector2i
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

        /// <summary>
        /// Returns true if this rect strictly contains p
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains( Vector2i p )
        {
            return
	        (
		        ( p.x > Origin.x ) &&
		        ( p.x < ( Origin.x + Width ) ) &&
		        ( p.y > Origin.y ) &&
		        ( p.y < ( Origin.y + Height ) )
	        );
        }

        public Vector4f PackToVector4f()
        {
            return new Vector4f( Origin.x, Origin.y, TopRight.x, TopRight.y );
        }

        public override string ToString()
        {
            return string.Format( "Rect [ x0: {0} y0: {1} width: {2} height: {3} ]", origin.x, origin.y, Width, Height );
        }

        public override int GetHashCode()
        {
            return origin.GetHashCode() ^ size.GetHashCode();
        }

        public override bool Equals( object o )
        {
            if( o is Rect2i )
            {                
                var rhs = ( Rect2i )o;
                return( Equals( rhs ) );
            }
            return false;
        }

        public bool Equals( Rect2i other )
        {
            return ( origin.Equals( other.origin ) && size.Equals( other.size ) );
        }
    }
}

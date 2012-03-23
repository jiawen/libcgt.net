using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{    
    public struct IntInterval
    {
        public static IntInterval Empty = new IntInterval( 0, 0 );
        public static IntInterval Infinite = new IntInterval( 0, 0, true, true );
        
        public static IntInterval HalfOpenLeft( int max )
        {
            return new IntInterval( 0, max, true, false );
        }

        public static IntInterval HalfOpenRight( int min )
        {
            return new IntInterval( min, 0, false, true );
        }

        private int min;
        private int max;

        private bool openLeft;
        private bool openRight;        

        public IntInterval( int min, int max )
        {
            this.min = min;
            this.max = max;

            this.openLeft = false;
            this.openRight = false;
        }

        public IntInterval( int min, int max,
            bool openLeft, bool openRight )
        {
            this.openLeft = openLeft;
            this.openRight = openRight;

            if( openLeft && openRight )
            {
                this.min = 0;
                this.max = 0;
            }
            else
            {
                this.min = min;
                this.max = max;
            }            
        }

        public bool OpenLeft
        {
            get
            {
                return openLeft;
            }
        }

        public bool OpenRight
        {
            get
            {
                return openRight;
            }
        }

        public bool Finite
        {
            get
            {
                return !( openLeft || openRight );
            }
        }

        public int Size
        {
            get
            {
                return Finite ? max - min + 1 : -1;
            }
        }

        public int Min
        {
            get
            {
                if( openLeft )
                {
                    throw new ArgumentOutOfRangeException( "Cannot get the min of an infinite interval" );
                }
                return min;
            }
        }

        public int Max
        {
            get
            {
                if( openRight )
                {
                    throw new ArgumentOutOfRangeException( "Cannot get the max of an infinite interval" );
                }
                return max;
            }
        }

        public override string ToString()
        {
            return string.Format( "{0} {1}, {2} {3}",
                                  openLeft ? "(" : "]",
                                  openLeft ? "-Inf" : min.ToString(),
                                  openRight ? "Inf" : max.ToString(),
                                  openRight ? ")" : "]" );
        }

        public static IntInterval operator & ( IntInterval i0, IntInterval i1 )
        {
            return Intersect( i0, i1 );
        }

        public static IntInterval Intersect( IntInterval i0, IntInterval i1 )
        {
            bool openLeftNew = false;
            int minNew;

            if( i0.openLeft )
            {
                if( i1.OpenLeft )
                {
                    openLeftNew = true;
                    minNew = 0;
                }
                else
                {
                    minNew = i1.min;
                }
            }
            else
            {
                if( i1.openLeft )
                {
                    minNew = i0.min;
                }
                else
                {
                    minNew = Math.Max( i0.min, i1.min );
                }
            }

            bool openRightNew = false;
            int maxNew;

            if( i0.openRight )
            {
                if( i1.openRight )
                {
                    openRightNew = true;
                    maxNew = minNew;
                }
                else
                {
                    maxNew = i1.Max;
                }
            }
            else
            {
                if( i1.openRight )
                {
                    maxNew = i0.Max;
                }
                else
                {
                    maxNew = Math.Min( i0.Max, i1.Max );
                }
            }

            if( !( openLeftNew || openRightNew ) &&
                ( maxNew < minNew ) )
            {
                return Empty;
            }
            else
            {
                return new IntInterval( minNew, maxNew, openLeftNew, openRightNew );
            }
        }
    }
}

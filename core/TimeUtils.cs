using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public static class TimeUtils
    {
        /// <summary>
        /// Converts 100 nanosecond ticks into seconds
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static float TicksToSeconds( int ticks )
        {
            return ticks * 1e-7f;
        }

        public static int SecondsToTicks( float seconds )
        {
            return ( seconds * 1e7 ).RoundToInt();
        }

        public static int FrameRateToTicks( int framesPerSecond )
        {
            return SecondsToTicks( 1.0f / framesPerSecond );
        }

        public static TimeSpan FrameRateToTimeSpan( int framesPerSecond )
        {
            return new TimeSpan( FrameRateToTicks( framesPerSecond ) );
        }
    }
}

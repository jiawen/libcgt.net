using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libcgt;
using libcgt.core.ImageProcessing;
using libcgt.core.Vecmath;

namespace libcgt.core
{
    public class RandomAccessVideoView : AbstractData
    {
        private IVideo video;
        private int t;
        private bool pingPong;

        public RandomAccessVideoView( IVideo video )
        {
            this.video = video;
            pingPong = true;
        }

        public Image4ub GetCurrentFrame()
        {
            return video.GetFrame( CurrentFrame );
        }

        public int CurrentFrame
        {
            get
            {
                return t;
            }
            set
            {
                if( t != value )
                {
                    //Console.WriteLine( "Someone is calling SetCurrentFrame, tNew = {0}", value );

                    t = value;
                    OnPropertyChanged( "CurrentFrame" );
                    //OnPropertyChanged( "CurrentFraction" );
                    OnPropertyChanged( "CurrentFrameDouble" );
                }
            }
        }

        public double CurrentFrameDouble
        {
            get
            {
                return t;
            }
            set
            {
                //Console.WriteLine( "currentFrameDouble = {0}", value );

                int it = value.RoundToInt().Clamp( 0, NumFrames );
                //Console.WriteLine( "it = {0}", it );
                CurrentFrame = it;
                //OnPropertyChanged( "CurrentFrameDouble" );
            }
        }

        public Vector2i Size
        {
            get
            {
                return video.Size;
            }
        }

        public int NumFrames
        {
            get
            {
                return video.NumFrames;
            }
        }

        /// <summary>
        /// For convenience, returns NumFrames - 1
        /// </summary>
        public int LastFrame
        {
            get
            {
                return NumFrames - 1;
            }
        }

        /// <summary>
        /// For convenient data binding to sliders without converters
        /// </summary>
        public double NumFramesMinusOneDouble
        {
            get
            {
                return video.NumFrames - 1;
            }
        }

        public bool PingPong
        {
            get
            {
                return pingPong;
            }
            set
            {
                if( value != pingPong )
                {
                    pingPong = value;
                    OnPropertyChanged( "PingPong" );
                }                
            }
        }

        public float MillisecondsPerFrame
        {
            get
            {
                return video.MillisecondsPerFrame;
            }
        }
    }
}

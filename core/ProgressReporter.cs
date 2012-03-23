using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace libcgt.core
{
    public class ProgressReporter
    {
        private readonly string prefix;
        private readonly int nTasks;
        private readonly float reportRatePercent;

        private long totalMillisecondsElapsed;
        private Stopwatch stopwatch;
        private long previousTaskCompletedTime;

        private float nextReportedPercent;
        private int nTasksCompleted;

        public ProgressReporter( int nTasks ) : this( "Working:", nTasks, 1 )
        {
            
        }

        /// <summary>
        /// Construct a new ProgressReporter given a prefix string "prefix",
        /// a predetermined number of tasks, and a reportRate of 1%
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="nTasks"></param>
        public ProgressReporter( string prefix, int nTasks )
            : this( prefix, nTasks, 1 )
        {
            
        }

        /// <summary>
        /// Construct a new ProgressReporter given a prefix string "prefix",
        /// a predetermined number of tasks, and a reportRatePercent.
        /// 
        /// If reportRatePercent = 10, will only print in approximately 10% increments.
        /// Pass in a negative value for reportRatePercent to report on every task.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="nTasks"></param>
        /// <param name="reportRatePercent"></param>
        public ProgressReporter( string prefix, int nTasks, float reportRatePercent )
        {
            if( prefix.EndsWith( ":" ) )
            {
                this.prefix = prefix;
            }
            else
            {
                this.prefix = prefix + ":";
            }
            
            this.nTasks = nTasks;
            this.reportRatePercent = reportRatePercent;

            this.totalMillisecondsElapsed = 0;

            stopwatch = Stopwatch.StartNew();
            previousTaskCompletedTime = stopwatch.ElapsedMilliseconds;
        }

        public string NotifyAndGetProgressString()
        {
            NotifyTaskCompleted();
            return GetProgressString();
        }

        public void NotifyAndPrintProgressString()
        {
            NotifyTaskCompleted();
            if( reportRatePercent < 0 )
            {
                Console.WriteLine( GetProgressString() );
            }
            else if( IsComplete() || PercentComplete() > nextReportedPercent )
            {
                Console.WriteLine( GetProgressString() );
                nextReportedPercent += reportRatePercent;
            }
        }

        public void NotifyTaskCompleted()
        {
            long now = stopwatch.ElapsedMilliseconds;
            long millisecondsForTask = now - previousTaskCompletedTime;
            previousTaskCompletedTime = now;
            
            totalMillisecondsElapsed += millisecondsForTask;

            if( nTasksCompleted < nTasks )
            {
                ++nTasksCompleted;
            }
        }        

        public string GetProgressString()
        {
            if( NumTasksRemaining() <= 0 )
            {
                return string.Format( "{0} 100% [done!]", prefix );
            }
            else
            {
                string timeRemainingString;
                if( ApproximateMillisecondsRemaining() < 1000 )
                {
                    timeRemainingString = string.Format( "{0} ms", ApproximateMillisecondsRemaining().RoundToInt() );
                }
                else
                {
                    timeRemainingString = string.Format( "{0:F1} s", ApproximateMillisecondsRemaining() / 1000 );
                }

                string timeElapsedString;
                if( totalMillisecondsElapsed < 1000 )
                {
                    timeElapsedString = string.Format( "{0} ms", totalMillisecondsElapsed );
                }
                else
                {
                    timeElapsedString = string.Format( "{0:F1} s", totalMillisecondsElapsed / 1000.0f );
                }

                return string.Format
                (
                    "{0} {1:F3}% [{2} tasks left ({3}), elapsed: {4}]",
                    prefix,
                    PercentComplete(),
                    NumTasksRemaining(),
                    timeRemainingString,
                    timeElapsedString
                );
            }
        }

        public float PercentComplete()
        {
            return 100.0f * Arithmetic.DivideIntsToFloat( nTasksCompleted, nTasks );
        }

        public bool IsComplete()
        {
            return ( nTasksCompleted == nTasks );
        }

        public int NumTasksRemaining()
        {
            return nTasks - nTasksCompleted;
        }

        public double ApproximateMillisecondsRemaining()
        {
            return NumTasksRemaining() * AverageMillisecondsPerTask();
        }

        public double AverageMillisecondsPerTask()
        {
            return Arithmetic.DivideLongsToDouble( totalMillisecondsElapsed, nTasksCompleted );
        }
    }
}

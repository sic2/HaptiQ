using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HaptiQ_API
{
    /// <summary>
    /// BeepOutput defines a class used to emit audio beeps using the motherboard
    /// </summary>
    public class BeepOutput
    {
        /// <summary>
        /// Maximum duration of a beep in ms
        /// </summary>
        public const int MAX_DURATION = 1000;

        /// <summary>
        /// Minimum duration of a beep in ms
        /// </summary>
        public const int MIN_DURATION = 100;

        /// <summary>
        /// Plays a beep using the motherboard.
        /// Note: that this functionality is not supported by 64-bit Window Operating Systems
        /// </summary>
        /// <param name="duration"></param>
        public static void Beep(int duration)
        {
            int frequency = 800;
            if (duration < MIN_DURATION) duration = MIN_DURATION;
            if (duration > MAX_DURATION) duration = MAX_DURATION;
            new Thread(() => Console.Beep(frequency, duration)).Start();
            
        }
    }
}

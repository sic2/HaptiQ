using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Input_API;

namespace HaptiQ_API
{
    public class PressureGestureArgs : EventArgs
    {
        /// <summary>
        /// Id of the HaptiQ generating the event
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// Position of the HaptiQ generating the event
        /// </summary>
        public Point Position { get; private set; }

        /// <summary>
        /// PRESSURE_GESTURE_TYPE of the generated gesture
        /// </summary>
        public PRESSURE_GESTURE_TYPE GestureType { get; private set; }

        /// <summary>
        /// Create a wrapper for the pressure gesture arguments
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="gestureType"></param>
        public PressureGestureArgs(uint id, Point position, PRESSURE_GESTURE_TYPE gestureType)
        {
            ID = id;
            Position = position;
            GestureType = gestureType;
        }
    }
}

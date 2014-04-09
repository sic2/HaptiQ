using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Input_API;

namespace HaptiQ_API
{
    public class HaptiQPositionArgs : EventArgs
    {
        /// <summary>
        /// Id of the HaptiQ that generated the event
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// Position of the HaptiQ that generated the event
        /// </summary>
        public Point Position { get; private set; }

        /// <summary>
        /// Orientation of the HaptiQ that generated the event
        /// </summary>
        public double Orientation { get; private set; }

        /// <summary>
        /// Creates a wrapper for the arguments of the event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        public HaptiQPositionArgs(uint id, Point position, double orientation)
        {
            ID = id;
            Position = position;
            Orientation = orientation;
        }
    }
}

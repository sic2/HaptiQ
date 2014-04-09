using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Input_API
{
    public class InputArgs : EventArgs
    {
        /// <summary>
        /// InputIdentifier used to uniquely distinguish the device
        /// </summary>
        public InputIdentifier InputIdentifier { get; private set; }

        /// <summary>
        /// Position of the device
        /// </summary>
        public Point Position { get; private set; }

        // Orientation of the device
        public double Orientation { get; private set; }

        /// <summary>
        /// Creates a wrapper for the arguments of the event
        /// </summary>
        /// <param name="inputIdentifier"></param>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        public InputArgs(InputIdentifier inputIdentifier, Point position, double orientation)
        {
            InputIdentifier = inputIdentifier;
            Position = position;
            Orientation = orientation;
        }
    }
}

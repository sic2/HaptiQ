using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    public class ActuatorPositionArgs : EventArgs
    {
        /// <summary>
        /// Id of the HaptiQ that generated the event
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// Id of the actuator that generated the event
        /// </summary>
        public int ActuatorId { get; private set; }

        /// <summary>
        /// Position of the actuator that generated the event
        /// </summary>
        public double Position { get; private set; }

        /// <summary>
        /// Creates a wrapper for the arguments of the event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="actuatorId"></param>
        /// <param name="position"></param>
        public ActuatorPositionArgs(uint id, int actuatorId, double position)
        {
            ID = id;
            ActuatorId = actuatorId;
            Position = position;
        }
    }
}
